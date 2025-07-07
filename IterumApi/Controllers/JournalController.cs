using System.Security.Claims;
using IterumApi.DTOs;
using IterumApi.Repositories;
using IterumApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IterumApi.Controllers
{
    [Route("iterum/api/[controller]")]
    [ApiController]
    public class JournalController : ControllerBase
    {
        char[] separators = { '.', '/' };
        private readonly IUserRepo _userRepo;
        private readonly IConfiguration _configuration;

        public JournalController(IUserRepo userRepo, IConfiguration configuration)
        {
            _userRepo = userRepo;
            _configuration = configuration;
        }

        [Authorize]
        [HttpPost("save")]
        public async Task<IActionResult> SaveNote([FromBody] JournalDto note)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (username == null)
            {
                return Unauthorized();
            }

            var stream = StreamServices.GenerateStreamFromString(note.Content);

            try
            {
                await BucketService.UploadStreamAsync(stream, $"{username}/journal/{note.Name}.txt");
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "File upload failed");
            }
        }

        [Authorize]
        [HttpGet("list/{name}")]
        public async Task<ActionResult<JournalDto>> GetJournalNote(string name)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            JournalDto? journal = await BucketService.DownloadJournalAsync(name, username);

            if (journal == null)
                return NotFound("Journal not found");

            return Ok(journal);
        }

        [Authorize]
        [HttpGet("preview/{*name}")]
        public async Task<ActionResult<JournalDto>> PreviewJournal([FromRoute] string name)
        {
            JournalDto? journal = await BucketService.DownloadJournalAsync(name);

            if (journal == null)
                return NotFound("Journal not found");

            return Ok(journal);
        }

        [Authorize]
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<string>>> ListUserNotes()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var userPrefix = $"{username}/journal";
            IEnumerable<string> files = await BucketService.ListFilesWithExtensionsAsync(userPrefix, [".txt"]);
            files = files.Select(x => {
                return x.Split(separators, StringSplitOptions.RemoveEmptyEntries).Reverse().ElementAt(1);
            });

            return Ok(files);
        }

        [Authorize]
        [HttpDelete("delete/{name}")]
        public async Task<IActionResult> DeleteJournalNote(string name)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var key = $"{username}/journal/{name}.txt";
            var success = await BucketService.DeleteFileAsync(key);

            if (!success)
                return StatusCode(500, "Failed to delete the note.");

            return Ok("Note deleted successfully.");
        }
    }
}
