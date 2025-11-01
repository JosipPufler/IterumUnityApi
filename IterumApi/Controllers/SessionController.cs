using IterumApi.DTOs;
using IterumApi.Models;
using IterumApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading;

namespace IterumApi.Controllers
{
    [Route("iterum/api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        private readonly IConfiguration _config;
        private readonly Random _random = new();
        static readonly Dictionary<string, (SessionConnectionInfo info, Process process)> instances = [];

        private readonly Lock _lock = new();

        private const int MaxPort = 8000;
        private const int MinPort = 7000;
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const string pathToExecutable = @"C:\Builds\Server\HeadlessServer.exe";

        public SessionController(IUserRepo userRepo, IConfiguration config)
        {
            _userRepo = userRepo;
            _config = config;
        }

        public Task<int> GetFreePort()
        {
            return Task.Run(() =>
            {
                int newPort;
                do
                {
                    newPort = _random.Next(MinPort, MaxPort);
                } while (instances.Values.Any(x => x.info.ConnectionPort == newPort));
                return newPort;
            });
        }

        public Task<string> GetNewSessionCode()
        {
            return Task.Run(() =>
            {
                string newCode;
                do
                {
                    newCode = new string([.. Enumerable.Repeat(chars, 6).Select(s => s[_random.Next(s.Length)])]);
                } while (instances.Keys.Any(x => x == newCode));
                return newCode;
            });
        }

        public async Task<List<string>> GetUsernames(List<long> userIds)
        {
            List<string> result = [];
            foreach (long id in userIds)
            {
                string? username = await GetUsername(id);
                if (username != null)
                {
                    result.Add(username);
                }
            }
            return result;
        }

        public async Task<string?> GetUsername(long userId)
        {
            User? user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
            {
                return null;
            }
            return user.Username;
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<ActionResult<SessionDto>> StartSession()
        {
            if (!long.TryParse(User.FindFirst("userId")?.Value, out long userId))
            {
                return Unauthorized();
            }
            if (string.IsNullOrEmpty(User.FindFirst(ClaimTypes.Name)?.Value))
            {
                return Unauthorized();
            }
            string username = User.FindFirst(ClaimTypes.Name).Value;

            if (instances.Any(x => x.Value.info.HostId == userId))
            {
                KeyValuePair<string, (SessionConnectionInfo info, Process process)> existingSession = instances.FirstOrDefault(x => x.Value.info.HostId == userId);
                existingSession.Value.process.Kill();
                lock (_lock)
                    instances.Remove(existingSession.Key);
            }

            string sessionCode = await GetNewSessionCode();
            int port = await GetFreePort();
            string ip = "127.0.0.1";
            SessionConnectionInfo sessionConnectionInfo = new(sessionCode, port, ip, [userId], userId);

            Process process = Process.Start(new ProcessStartInfo
            {
                FileName = pathToExecutable,
                WorkingDirectory = Path.GetDirectoryName(pathToExecutable),
                Arguments = $"-batchmode -nographics -port {port} -sessionCode {sessionCode} -adminUsername {Environment.GetEnvironmentVariable("ITERUM_ADMIN_USERNAME")} -adminPassword {Environment.GetEnvironmentVariable("ITERUM_ADMIN_PASSWORD")}",
                UseShellExecute = false
            });

            if (process == null || process.HasExited)
            {
                return StatusCode(500, "Failed to start session server process.");
            }
            process.EnableRaisingEvents = true;
            process.Exited += (sender, e) =>
            {
                lock (_lock) { instances.Remove(sessionCode); }
            };

            lock (_lock)
                instances.Add(sessionCode, (sessionConnectionInfo, process));

            return Ok(new SessionDto(sessionCode, port, ip, [username], username));
        }

        [Authorize]
        [HttpPost("join/{sessionCode}")]
        public async Task<ActionResult<SessionDto>> JoinSession(string sessionCode)
        {
            if (!long.TryParse(User.FindFirst("userId")?.Value, out long userId))
            {
                return Unauthorized();
            }
            if (string.IsNullOrEmpty(User.FindFirst(ClaimTypes.Name)?.Value))
            {
                return Unauthorized();
            }
            string username = User.FindFirst(ClaimTypes.Name).Value;

            if (!instances.Any(x => x.Key == sessionCode))
            {
                return NotFound($"Session with code {sessionCode} not found");
            }
            KeyValuePair<string, (SessionConnectionInfo info, Process process)> keyValuePair = instances.First(x => x.Key == sessionCode);
            keyValuePair.Value.info.PlayerUserIds.Add(userId);

            SessionConnectionInfo connInfo = keyValuePair.Value.info;
            return Ok(new SessionDto(connInfo.SessionCode, connInfo.ConnectionPort, connInfo.ConnectionIp, await GetUsernames(connInfo.PlayerUserIds), await GetUsername(connInfo.HostId) ?? "unknownHost"));
        }

        [Authorize]
        [HttpPost("end")]
        public async Task<ActionResult> EndSession()
        {
            if (!long.TryParse(User.FindFirst("userId")?.Value, out long userId))
            {
                return Unauthorized();
            }
            if (instances.Any(x => x.Value.info.HostId == userId))
            {
                KeyValuePair<string, (SessionConnectionInfo info, Process process)> keyValuePair = instances.First(x => x.Value.info.HostId == userId);
                keyValuePair.Value.process.Kill();

                lock (_lock)
                    instances.Remove(keyValuePair.Key);
                return Ok();
            }
            return NotFound();
        }
    }
}
