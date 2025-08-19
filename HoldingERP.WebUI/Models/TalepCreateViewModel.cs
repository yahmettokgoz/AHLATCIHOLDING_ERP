using HoldingERP.Entities;
using HoldingERP.Entities.Concrete;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace HoldingERP.WebUI.Models
{
    public class TalepUrunModel
    {
        [Display(Name = "Ürün")]
        [Required(ErrorMessage = "Lütfen bir ürün seçiniz.")]
        public int UrunId { get; set; }

        [Display(Name = "Miktar")]
        [Required(ErrorMessage = "Lütfen bir miktar giriniz.")]
        [Range(1, 10000, ErrorMessage = "Miktar 1 ile 10000 arasında olmalıdır.")]
        public int Miktar { get; set; }
    }

    public class TalepCreateViewModel
    {
        [Display(Name = "Talep Açıklaması")]
        [Required(ErrorMessage = "Lütfen bir açıklama giriniz.")]
        [StringLength(500, ErrorMessage = "Açıklama 500 karakterden uzun olamaz.")]
        public string Aciklama { get; set; }

        public List<TalepUrunModel> TalepUrunleri { get; set; }

        public IEnumerable<Urun>? Urunler { get; set; }

        public TalepCreateViewModel()
        {
            TalepUrunleri = new List<TalepUrunModel> { new TalepUrunModel() };
        }
    }
}