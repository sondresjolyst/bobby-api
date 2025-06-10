using Microsoft.AspNetCore.Mvc;

namespace bobby_api.Controllers;

[ApiController]
[Route("[controller]")]
public class PictureOfTheDayController : ControllerBase
{
    private readonly string _imagesFolder = "images";

    [HttpGet]
    public IActionResult Get()
    {
        if (!Directory.Exists(_imagesFolder))
            return NotFound("Image folder not found.");

        var files = Directory.GetFiles(_imagesFolder)
            .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                        f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                        f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
            .OrderBy(f => f)
            .ToArray();

        if (files.Length == 0)
            return NotFound("No images found.");

        var dayOfYear = DateTime.UtcNow.DayOfYear;
        var index = dayOfYear % files.Length;
        var filePath = files[index];
        var contentType = GetContentType(filePath);

        var fileBytes = System.IO.File.ReadAllBytes(filePath);
        var base64String = Convert.ToBase64String(fileBytes);

        return Ok(new
        {
            FileName = Path.GetFileName(filePath),
            ContentType = contentType,
            Base64 = base64String
        });
    }

    private static string GetContentType(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        return ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream"
        };
    }
}
