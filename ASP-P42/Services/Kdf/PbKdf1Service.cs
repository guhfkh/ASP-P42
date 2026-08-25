using ASP_P42.Services.Hash;

namespace ASP_P42.Services.Kdf
{
    public class PbKdf1Service(IHashServices hashService) : IKdfService
    {
        private readonly IHashServices _hashService = hashService;
        private const int iterationCount = 1_000_000;
        private const int dkLength = 32;
        private const string filler = "B6915281DD9C4436963BB2970FD6DC93";

        public string Dk(string password, string salt)
        {
            string t = _hashService.Digest(password + salt);

            for(int i = 1; i < iterationCount; i++)
            {
                t = _hashService.Digest(t);
            }

            return t.Length >= dkLength ?
                t[..dkLength] :
                t + filler[..(dkLength - t.Length)];


        }
    }
}
