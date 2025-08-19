using HoldingERP.Entities.Concrete;
using System.ComponentModel.DataAnnotations;

namespace HoldingERP.WebUI.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ad Soyad alanı gereklidir.")]
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kullanıcı Adı alanı gereklidir.")]
        [Display(Name = "Kullanıcı Adı")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta alanı gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre alanı gereklidir.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre Tekrar alanı gereklidir.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrar")]
        [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Departman alanı gereklidir.")]
        [Display(Name = "Departman")]
        public int DepartmanId { get; set; }

        // Bu, formdaki dropdown'ı doldurmak için kullanılacak.
        public IEnumerable<Departman>? Departmanlar { get; set; }

    }
}
