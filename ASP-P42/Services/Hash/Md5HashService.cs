namespace ASP_P42.Services.Hash
{
    public class Md5HashService : IHashServices
    {
        public string Digest(string input)
        {
            return System.Convert.ToHexString(
                System.Security.Cryptography.MD5.HashData(
                    System.Text.Encoding.UTF8.GetBytes(input)));
        }
    }
}
