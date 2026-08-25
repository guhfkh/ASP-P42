namespace ASP_P42.Services.Kdf
{
    public interface IKdfService
    {
        string Dk(string password, string salt);

    }
}
