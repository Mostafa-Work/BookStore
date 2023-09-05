namespace BookStore.Helpers
{
    public class FilesHelper
    {
        private readonly IWebHostEnvironment env;
        public FilesHelper(IWebHostEnvironment env)
        {
            this.env = env;
        }
        public async Task<string> UploadFileAsync(string folderPath, IFormFile file)
        {
            string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string creationPhysicalPath = Path.Combine(env.WebRootPath, folderPath, fileName);
            FileStream fileStream = new FileStream(creationPhysicalPath, FileMode.Create);
            await file.CopyToAsync(fileStream);
            fileStream.Close();
            return fileName;
        }

        public async Task DeleteFileAsync(string filePath)
        {
            await Task.Run(() => File.Delete(filePath));
        }
    }
}
