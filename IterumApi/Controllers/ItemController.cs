using IterumApi.DTOs;
using IterumApi.Models;
using IterumApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IterumApi.Controllers
{
    [Route("/iterum/api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly ItemRepo _itemRepo;

        public ItemController(ItemRepo itemRepo)
        {
            _itemRepo = itemRepo;
        }

        [Authorize]
        [HttpPost("save")]
        public async Task<ActionResult<ItemDto>> CreateItem([FromBody] ItemDto itemDto)
        {
            if (!long.TryParse(User.FindFirst("userId")?.Value, out long userId))
            {
                return Unauthorized();
            }

            try
            {
                Item item = await _itemRepo.CreateAsync(new Item(itemDto, userId));
                return Ok(new ItemDto(item));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Item creation failed");
            }
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<ActionResult> UpdateItem([FromBody] ItemDto itemDto)
        {
            if (!long.TryParse(User.FindFirst("userId")?.Value, out long userId))
            {
                return Unauthorized();
            }

            try
            {
                bool result = await _itemRepo.UpdateAsync(new Item(itemDto, userId));
                if (!result)
                {
                    return StatusCode(401);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Item update failed");
            }
        }

        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteItem(string id)
        {
            if (!long.TryParse(User.FindFirst("userId")?.Value, out long userId))
            {
                return Unauthorized();
            }

            try
            {
                bool result = await _itemRepo.DeleteAsync(id, userId);
                if (!result)
                {
                    return StatusCode(401);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Item deletion failed");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<ActionDto>>> GetItems()
        {
            if (!long.TryParse(User.FindFirst("userId")?.Value, out long userId))
            {
                return Unauthorized();
            }

            try
            {
                List<Item> items = await _itemRepo.GetAllByUserIdAsync(userId);
                return Ok(items.Select(item => new ItemDto(item)).ToList());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Action fetch failed");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetItemById(string id)
        {
            try
            {
                Item? item = await _itemRepo.GetByIdAsync(id);
                if (item == null)
                {
                    return NotFound();
                }
                return Ok(new ItemDto(item));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Action fetch failed");
            }
        }
    }
}
