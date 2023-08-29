using Rahpele.Services.Interfaces;

namespace Rahpele.Services
{
    public class FileUploader : IFileUploader
    {
        string rootPath = Directory.GetCurrentDirectory() + "\\wwwroot";
        public async Task<bool> UploadPictureWithFileName(IFormFile file, string path, string filename)
        {
            string currentpath = Path.Combine(rootPath, path, filename);

            try
            {
                using (var stream = new FileStream(currentpath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
