using HoldingERP.Entities.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HoldingERP.WebUI.Models
{
    public class TeklifKalemModel
    {
        public int TalepUrunId { get; set; }
        public string UrunAdi { get; set; } = string.Empty;
        public decimal Miktar { get; set; }

        [Required(ErrorMessage = "Birim fiyat girilmelidir.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
        public decimal BirimFiyat { get; set; }
    }

    public class TeklifModel
    {
        [Required(ErrorMessage = "Lütfen bir tedarikçi adı giriniz.")]
        [Display(Name = "Tedarikçi")]
        public string TedarikciAdi { get; set; } = string.Empty;

        public List<TeklifKalemModel> TeklifKalemleri { get; set; }

        public TeklifModel()
        {
            TeklifKalemleri = new List<TeklifKalemModel>();
        }
    }

    public class TeklifGirViewModel
    {
        public int TalepId { get; set; }
        public string? TalepAciklamasi { get; set; }
        public List<TeklifModel> GirilenTeklifler { get; set; }

        public IEnumerable<SatinAlmaTalepUrunu>? TalepUrunleri { get; set; }

        public TeklifGirViewModel()
        {
            GirilenTeklifler = new List<TeklifModel> { new TeklifModel() };
        }
    }
}