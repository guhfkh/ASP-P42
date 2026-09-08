using ASP_P42.Services.Storage;

namespace ASP_P42.LocalStorage
{
    public class LocalStorageService : IStorageService
    {
        private readonly string[] allowedExtensions = [".jpd", ".png", ".jpeg", ".webp"];
        private readonly string localFolder = "LocalStorage";

        public byte[] Load(string filename)
        {
            return File.ReadAllBytes(
                    Path.Combine(localFolder, filename));
        }

        public string Save(IFormFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file),
                    "Data not received");
            }
            if (file.Length < 256)
            {
                throw new ArgumentException("File too short");
            }
            if (file.Length > 1e7)
            {
                throw new ArgumentException("File too long");
            }

            int dotPosition = file.FileName.LastIndexOf('.');
            if(dotPosition < 0)
            {
                throw new ArgumentException("File must have extension");
            }
            string ext = file.FileName[dotPosition..];

            if(!allowedExtensions.Contains(ext))
            {
                throw new ArgumentException("File type not Allowed");
            }

            string savedName = Guid.NewGuid().ToString() + ext;
            using FileStream stream = File.OpenWrite(
                Path.Combine(localFolder, savedName)
            );
            file.CopyTo(stream);
            return savedName;


        }
    }
}
