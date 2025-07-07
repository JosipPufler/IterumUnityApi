using System.IO;
using IterumApi.DTOs;
using IterumApi.Models;
using IterumApi.Repositories;
using IterumApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IterumApi.Controllers
{
    [Route("iterum/api/maps")]
    [ApiController]
    public class MapController : ControllerBase
    {
        private readonly MapRepo _mapRepo;

        public MapController(MapRepo maprepo)
        {
            _mapRepo = maprepo;
        }

        [Authorize]
        [HttpPost("save")]
        public async Task<ActionResult<MapDto>> CreateMap([FromBody] MapDto mapDto)
        {
            if (!long.TryParse(User.FindFirst("userId")?.Value, out long userId))
            {
                return Unauthorized();
            }

            try
            {
                Map map = await _mapRepo.CreateAsync(new Map(mapDto, userId));
                return Ok(new MapDto(map.Id, map.Name, map.Hexes, map.IsFlatTopped, mapDto.maxX, mapDto.maxY));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Map creation failed");
            }
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<ActionResult> UpdateMap([FromBody] MapDto mapDto)
        {
            if (!long.TryParse(User.FindFirst("userId")?.Value, out long userId))
            {
                return Unauthorized();
            }

            try
            {
                bool result = await _mapRepo.UpdateAsync(new Map(mapDto, userId));
                if (!result)
                {
                    return StatusCode(401);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Map update failed");
            }
        }

        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteMap(string id)
        {
            if (!long.TryParse(User.FindFirst("userId")?.Value, out long userId))
            {
                return Unauthorized();
            }

            try
            {
                bool result = await _mapRepo.DeleteAsync(id, userId);
                if (!result)
                {
                    return StatusCode(401);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Map deletion failed");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<MapDto>>> GetMaps()
        {
            if (!long.TryParse(User.FindFirst("userId")?.Value, out long userId))
            {
                return Unauthorized();
            }

            try
            {
                List<Map> maps = await _mapRepo.GetAllByUserIdAsync(userId);
                return Ok(maps.Select(x => new MapDto(x.Id, x.Name, x.Hexes, x.IsFlatTopped, x.MaxX, x.MaxY)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Map fetch failed");
            }
        }
    }
}
