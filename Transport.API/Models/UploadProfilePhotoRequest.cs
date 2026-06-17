using Microsoft.AspNetCore.Mvc;

namespace Transport.API.Models
{
    public class UploadProfilePhotoRequest
    {
        [FromForm(Name = "file")]
        public IFormFile? File { get; set; }
    }
}
