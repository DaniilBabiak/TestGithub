using Microsoft.AspNetCore.Http;
using Practice.Service.Exceptions;
using System;
using System.IO;
using ThumbnailSharp;

namespace Practice.Api.Services
{
    public class FileSerivce : IFileService
    {
        public string SaveFile(IFormFile file, string folderName)
        {
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\" + folderName + @"\");

            string fileName = Guid.NewGuid().ToString() + ".jpg";
            string fullPath = pathToSave + fileName;
            string dbPath = folderName + @"\" + fileName;

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return dbPath;
        }

        public string SaveThumbnail(IFormFile file, string folderName)
        {
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\" + folderName + @"\");

            string fileName = "thumbnail_" + Guid.NewGuid().ToString() + ".jpg";
            string fullPath = pathToSave + fileName;
            string dbPath = folderName + @"\" + fileName;

            try
            {
                using (Stream resultStream = new ThumbnailCreator().CreateThumbnailStream(
                thumbnailSize: 100,
                imageStream: file.OpenReadStream(),
                imageFormat: Format.Jpeg))
                {
                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        resultStream.CopyTo(fileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ServiceException(ex.Message, ex);
            }


            return dbPath;
        }

        public void DeleteFile(string path)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\" + path);

            File.Delete(fullPath);
        }
    }
}
