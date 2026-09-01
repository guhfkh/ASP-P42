namespace ASP_P42.Models
{
    public class AuthJournal
    {
        public Guid Id { get; set; }

        public DateTime DateTime { get; set; }

        public string Login { get; set; } = string.Empty;

        public string Dk { get; set; } = string.Empty;

        public bool IsOk { get; set; }
    }
}
