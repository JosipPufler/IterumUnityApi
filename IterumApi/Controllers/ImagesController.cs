using IterumApi.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IterumApi.DTOs;
using IterumApi.Repositories;
using Amazon.S3;

namespace IterumApi.Controllers
{
    [Route("iterum/api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        char[] separators = { '/' };

        [Authorize]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (username == null)
                return Unauthorized();

            if (image == null || image.Length == 0)
                return BadRequest("No image uploaded.");

            var keyName = $"{username}/images/{image.FileName}";

            try
            {
                using var stream = image.OpenReadStream();
                await BucketService.UploadStreamAsync(stream, keyName, image.ContentType);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Image upload failed.");
            }
        }

        [Authorize]
        [HttpGet("list/{filename}")]
        public async Task<IActionResult> DownloadImage(string filename)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (username == null)
                return Unauthorized();

            var keyName = $"{username}/images/{filename}";

            try
            {
                var stream = await BucketService.DownloadStreamAsync(keyName);
                var contentType = GetContentType(filename);
                Response.Headers.ContentLength = stream.Length;
                return File(stream, contentType, filename);
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound("Image not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Failed to download image.");
            }
        }

        [Authorize]
        [HttpGet("preview/{filename}")]
        public async Task<IActionResult> PreviewImage(string filePath)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (username == null)
                return Unauthorized();

            var fileName = Path.GetFileName(filePath);
            try
            {
                var stream = await BucketService.DownloadStreamAsync(filePath);
                var contentType = GetContentType(filePath);
                Response.Headers.ContentLength = stream.Length;
                return File(stream, contentType, fileName);
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound("Image not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Failed to download image.");
            }
        }

        [Authorize]
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<string>>> ListImages()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var userPrefix = $"{username}/images";
            IEnumerable<string> files = await BucketService.ListFilesWithExtensionsAsync(userPrefix, [".png", ".jpg", ".jpeg", ".webp"]);
            files = files.Select(x => {
                return x.Split(separators, StringSplitOptions.RemoveEmptyEntries).Reverse().ElementAt(0);
            });

            return Ok(files);
        }

        [Authorize]
        [HttpDelete("delete/{name}")]
        public async Task<IActionResult> DeleteImage(string name)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var key = $"{username}/images/{name}";
            var success = await BucketService.DeleteFileAsync(key);

            if (!success)
                return StatusCode(500, "Failed to delete image.");

            return Ok("Image deleted successfully.");
        }

        private static string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }
}
