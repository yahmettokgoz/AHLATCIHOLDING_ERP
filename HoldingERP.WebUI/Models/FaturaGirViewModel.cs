using HoldingERP.Entities.Concrete;
using System.ComponentModel.DataAnnotations;

namespace HoldingERP.WebUI.Models
{
    public class FaturaGirViewModel
    {
        public int TalepId { get; set; }
        public Teklif OnaylanmisTeklif { get; set; }

        [Required(ErrorMessage = "Fatura Numarası gereklidir.")]
        [Display(Name = "Fatura Numarası")]
        [StringLength(50)]
        public string FaturaNo { get; set; }

        [Required(ErrorMessage = "Fatura Tarihi gereklidir.")]
        [Display(Name = "Fatura Tarihi")]
        [DataType(DataType.Date)]
        public DateTime FaturaTarihi { get; set; } = DateTime.Today;
    }
}
