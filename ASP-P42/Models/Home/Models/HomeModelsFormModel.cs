using Microsoft.AspNetCore.Mvc;

namespace ASP_P42.Models.Home.Models
{
    public class HomeModelsFormModel
    {
        [FromForm(Name = "user-login")]
        public string UserLogin { get; set; } = null!;

        [FromForm(Name = "user-password")]
        public string UserPassword { get; set; } = null!;

        [FromForm(Name = "remember-me")]
        public bool RememberMe { get; set; }

        // Радиокнопки
        [FromForm(Name = "gender")]
        public string Gender { get; set; } = null!;

        // Дата
        [FromForm(Name = "birth-date")]
        public DateTime? BirthDate { get; set; }

        [FromForm(Name = "favorite-color")]
        public string FavoriteColor { get; set; } = null!;

        // Число
        [FromForm(Name = "age")]
        public int? Age { get; set; }

        // Диапазон
        [FromForm(Name = "rating")]
        public int? Rating { get; set; }

        // Email
        [FromForm(Name = "email")]
        public string Email { get; set; } = null!;

        [FromForm(Name = "phone")]
        public string Phone { get; set; } = null!;

        // Выпадающий список
        [FromForm(Name = "country")]
        public string Country { get; set; } = null!;
    }
}