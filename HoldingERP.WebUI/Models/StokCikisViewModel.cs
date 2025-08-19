using HoldingERP.Entities.Concrete;
using System.ComponentModel.DataAnnotations;

namespace HoldingERP.WebUI.Models
{
    public class StokCikisViewModel
    {

        public int StokId { get; set; }
        public string UrunAdi { get; set; }
        public decimal MevcutMiktar { get; set; }

        [Required(ErrorMessage = "Miktar girilmelidir.")]
        [Display(Name = "Çıkış Yapılacak Miktar")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Miktar 0'dan büyük olmalıdır.")]
        public decimal CikisMiktari { get; set; }

        [Required(ErrorMessage = "Departman seçilmelidir.")]
        [Display(Name = "Hangi Departman İçin")]
        public int DepartmanId { get; set; }

        public IEnumerable<Departman>? Departmanlar { get; set; }

    }
}
