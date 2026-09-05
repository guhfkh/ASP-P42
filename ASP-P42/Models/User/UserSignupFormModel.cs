using System.Text.Json.Serialization;

namespace ASP_P42.Models.User
{
    public class UserSignupFormModel
    {
        [JsonPropertyName("fullName")]
        public string FullName { get; set; } = null!;

        [JsonPropertyName("login")]
        public string Login { get; set; } = null!;

        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [JsonPropertyName("phone")]
        public string? Phone { get; set; } = null!;

        [JsonPropertyName("password")]
        public string Password { get; set; } = null!;

        [JsonPropertyName("repeat")]
        public string Repeat { get; set; } = null!;

        [JsonPropertyName("isAgree")]
        public bool IsAgree { get; set; }
    }
}
