using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Disk.Api.Models;

public class FileDto
{
    public IFormFile? File { get; set; }

    public FileDto(IFormFile? file) => this.File = file;

    public static ValueTask<FileDto?> BindAsync(HttpContext httpContext)
    {
        var fileDto = new FileDto(httpContext.Request.Form.Files["file"]);
        return ValueTask.FromResult<FileDto?>(fileDto);
    }
}