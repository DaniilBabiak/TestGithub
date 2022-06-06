using Microsoft.AspNetCore.Http;

namespace Practice.Api.Services
{
    public interface IFileService
    {
        void DeleteFile(string path);
        public string SaveFile(IFormFile file, string folderName);
        public string SaveThumbnail(IFormFile file, string folderName);
    }
}
