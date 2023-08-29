namespace Rahpele.Services.Interfaces
{
    public interface IFileUploader
    {
        Task<bool> UploadPictureWithFileName(IFormFile file, string path, string filename);
    }
}
