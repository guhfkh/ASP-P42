namespace ASP_P42.Services.Storage
{
    public interface IStorageService
    {
        string Save(IFormFile file);

        byte[] Load(string filename);
    }
}
