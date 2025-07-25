using IterumApi.DTOs;
using IterumApi.Models;
using IterumApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IterumApi.Controllers
{
    [Route("iterum/api/characters")]
    [ApiController]
    public class CharacterController : ControllerBase
    {
        private readonly CharacterRepo _characterRepo;

        public CharacterController(CharacterRepo characterRepo)
        {
            _characterRepo = characterRepo;
        }

        [Authorize]
        [HttpPost("save")]
        public async Task<ActionResult<CharacterDto>> CreateCharacter([FromBody] CharacterDto characterDto)
        {
            if (!long.TryParse(User.FindFirst("userId")?.Value, out long userId))
            {
                return Unauthorized();
            }

            try
            {
                Character character = await _characterRepo.CreateAsync(new Character(characterDto, userId));
                return Ok(new CharacterDto(character.Id, character.Name, character.Level, character.IsPlayer, character.Data));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Character creation failed");
            }
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<ActionResult> UpdateMap([FromBody] CharacterDto characterDto)
        {
            if (!long.TryParse(User.FindFirst("userId")?.Value, out long userId))
            {
                return Unauthorized();
            }

            try
            {
                bool result = await _characterRepo.UpdateAsync(new Character(characterDto, userId));
                if (!result)
                {
                    return StatusCode(401);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Character update failed");
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
                bool result = await _characterRepo.DeleteAsync(id, userId);
                if (!result)
                {
                    return StatusCode(401);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Character deletion failed");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<CharacterDto>>> GetMaps()
        {
            if (!long.TryParse(User.FindFirst("userId")?.Value, out long userId))
            {
                return Unauthorized();
            }

            try
            {
                List<Character> maps = await _characterRepo.GetAllByUserIdAsync(userId);
                return Ok(maps.Select(character => new CharacterDto(character.Id, character.Name, character.Level, character.IsPlayer, character.Data)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Character fetch failed");
            }
        }
    }
}
