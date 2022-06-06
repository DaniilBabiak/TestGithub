using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Practice.Api.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Practice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        // POST api/<FileController>
        [HttpPost, Authorize]
        public IActionResult UnloadFile([FromForm] string folderName)
        {
            if (Request.Form.Files.Count <= 0)
            {
                return BadRequest(new { Message = "IncorrectImageFile" });
            }

            if (string.IsNullOrEmpty(folderName))
            {
                return BadRequest(new { Message = "IncorrectFolderName" });
            }

            var file = Request.Form.Files[0];
            var imagePath = _fileService.SaveFile(file, folderName);
            var thumbnailPath = _fileService.SaveThumbnail(file, "Thumbnails");
            return Ok(new { imagePath, thumbnailPath });
        }
    }
}
