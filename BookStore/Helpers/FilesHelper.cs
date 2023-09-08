namespace BookStore.Helpers
{
    public class FilesHelper
    {
        private readonly IWebHostEnvironment env;
        public FilesHelper(IWebHostEnvironment env)
        {
            this.env = env;
        }
        public async Task UploadFileAsync(string creationPhysicalPath, IFormFile file)
        {
            FileStream fileStream = new FileStream(creationPhysicalPath, FileMode.Create);
            await file.CopyToAsync(fileStream);
            fileStream.Close();
        }

        public async Task DeleteFileAsync(string filePath)
        {
            await Task.Run(() => File.Delete(filePath));
        }
    }
}
