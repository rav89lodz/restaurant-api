using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;

namespace RestaurantAPI.Controllers;

[ApiController]
[Route("file")]
[Authorize]
public class FileController : ControllerBase
{
    [HttpGet]
    // odpowiedź jest zapisywana w cache po stronie klienta
    [ResponseCache(Duration = 1200, VaryByQueryKeys = new []{ "fileName" })]
    public ActionResult GetFile([FromQuery] string fileName)
    {
        var rootPath = Directory.GetCurrentDirectory();
        var filePath = $"{rootPath}/PrivateFiles/{fileName}";

        if(!System.IO.File.Exists(filePath))
        {
            return NotFound();
        }

        var contentProvider = new FileExtensionContentTypeProvider();
        contentProvider.TryGetContentType(filePath, out var contentType);

        var file = System.IO.File.ReadAllBytes(filePath);
        return File(file, contentType, fileName);
    }

    [HttpPost]
    public ActionResult Upload([FromForm] IFormFile file)
    {
        if(file == null || file.Length < 1)
        {
            return BadRequest();
        }
        var rootPath = Directory.GetCurrentDirectory();
        var fileName = file.FileName;
        var filePath = $"{rootPath}/PrivateFiles/{fileName}";

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            file.CopyTo(stream);
        }
        return Ok();
    }
}
