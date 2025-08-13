using HoldingERP.Entities;
using System.ComponentModel.DataAnnotations;

namespace HoldingERP.WebUI.Models
{
    public class AssignAmirViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }

        [Display(Name = "Amir Olarak Ata")]
        public int? SecilenAmirId { get; set; }
        public IEnumerable<Kullanici>? AmirAdaylari { get; set; }
    }
}
