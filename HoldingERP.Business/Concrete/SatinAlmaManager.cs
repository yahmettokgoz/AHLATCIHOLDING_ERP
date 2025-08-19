using HoldingERP.Business.Abstract;
using HoldingERP.DataAccess.Abstract;
using HoldingERP.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HoldingERP.Entities.Concrete.Teklif;

namespace HoldingERP.Business.Concrete
{
    public class SatinAlmaManager: ISatinAlmaService
    {
        private readonly IRepository<Teklif> _teklifRepository;
        private readonly IRepository<SatinAlmaTalebi> _talepRepository;

        public SatinAlmaManager(IRepository<Teklif> teklifRepository, IRepository<SatinAlmaTalebi> talepRepository)
        {
            _teklifRepository = teklifRepository;
            _talepRepository = talepRepository;
        }

        public string TeklifiSecVeSüreciIlerlet(int secilenTeklifId)
        {
            var secilenTeklif = _teklifRepository.GetById(secilenTeklifId);
            if (secilenTeklif == null) return "Seçilen teklif bulunamadı.";

            var anaTalep = _talepRepository.GetById(secilenTeklif.SatinAlmaTalebiId);
            if (anaTalep == null) return "İlişkili ana talep bulunamadı.";

            var digerTeklifler = _teklifRepository
                .Find(t => t.SatinAlmaTalebiId == anaTalep.Id && t.Id != secilenTeklifId)
                .ToList();

            foreach (var teklif in digerTeklifler)
            {
                teklif.Durum = TeklifDurumu.Reddedildi;
                _teklifRepository.Update(teklif);
            }

            decimal limit = 100000;
            string successMessage;

            if (secilenTeklif.ToplamFiyat > limit)
            {
                secilenTeklif.Durum = TeklifDurumu.Beklemede;
                anaTalep.Durum = TalepDurumu.YoneticiOnayiBekliyor;
                successMessage = $"Teklif seçildi. Tutar ({secilenTeklif.ToplamFiyat:C}) limiti aştığı için yönetici onayına gönderildi.";
            }
            else
            {
                secilenTeklif.Durum = TeklifDurumu.Onaylandi;
                anaTalep.Durum = TalepDurumu.Onaylandi;
                successMessage = $"'{secilenTeklif.Tedarikci?.Ad}' tedarikçisinin teklifi başarıyla seçildi ve talep onaylandı.";
            }

            _teklifRepository.Update(secilenTeklif);
            _talepRepository.Update(anaTalep);

            _talepRepository.SaveChanges(); // Değişiklikleri kaydet

            return successMessage;
        }
    }
}
