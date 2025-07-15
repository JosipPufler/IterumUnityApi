using IterumApi.DTOs;
using IterumApi.Models;
using IterumApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using Action = IterumApi.Models.Action;

namespace IterumApi.Controllers
{
    [Route("iterum/api/actions")]
    [ApiController]
    public class ActionController : ControllerBase
    {
        private readonly ActionRepo _actionRepo;

        public ActionController(ActionRepo actionRepo)
        {
            _actionRepo = actionRepo;
        }

        [Authorize]
        [HttpPost("save")]
        public async Task<ActionResult<ActionDto>> CreateAction([FromBody] ActionDto characterDto)
        {
            if (!long.TryParse(User.FindFirst("userId")?.Value, out long userId))
            {
                return Unauthorized();
            }

            try
            {
                Action action = await _actionRepo.CreateAsync(new Action(characterDto, userId));
                return Ok(new ActionDto(action.Id, action.Name, action.Description, action.ApCost, action.MpCost, action.Data));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Action creation failed");
            }
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<ActionResult> UpdateAction([FromBody] ActionDto actionDto)
        {
            if (!long.TryParse(User.FindFirst("userId")?.Value, out long userId))
            {
                return Unauthorized();
            }

            try
            {
                bool result = await _actionRepo.UpdateAsync(new Action(actionDto, userId));
                if (!result)
                {
                    return StatusCode(401);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Action update failed");
            }
        }

        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteAction(string id)
        {
            if (!long.TryParse(User.FindFirst("userId")?.Value, out long userId))
            {
                return Unauthorized();
            }

            try
            {
                bool result = await _actionRepo.DeleteAsync(id, userId);
                if (!result)
                {
                    return StatusCode(401);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Action deletion failed");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<ActionDto>>> GetActions()
        {
            if (!long.TryParse(User.FindFirst("userId")?.Value, out long userId))
            {
                return Unauthorized();
            }

            try
            {
                List<Action> maps = await _actionRepo.GetAllByUserIdAsync(userId);
                return Ok(maps.Select(action => new ActionDto(action.Id, action.Name, action.Description, action.ApCost, action.MpCost, action.Data)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Action fetch failed");
            }
        }
    }
}
