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
            if (secilenTeklif == null) return "Hata: Seçilen teklif bulunamadı.";

            var anaTalep = _talepRepository.GetById(secilenTeklif.SatinAlmaTalebiId);
            if (anaTalep == null) return "Hata: İlişkili ana talep bulunamadı.";

            var digerTeklifler = _teklifRepository
                .Find(t => t.SatinAlmaTalebiId == anaTalep.Id && t.Id != secilenTeklifId)
                .ToList();

            foreach (var teklif in digerTeklifler)
            {
                teklif.Durum = TeklifDurumu.Reddedildi;
                _teklifRepository.Update(teklif);
            }

            secilenTeklif.Durum = TeklifDurumu.Onaylandi;
            _teklifRepository.Update(secilenTeklif);

            decimal yonetimKuruluLimiti = 1000000;
            string successMessage;

            if (secilenTeklif.ToplamFiyat < yonetimKuruluLimiti)
            {
                anaTalep.Durum = TalepDurumu.GenelMudurOnayiBekliyor;
                successMessage = "Teklif seçildi ve Genel Müdür onayına gönderildi.";
            }
            else
            {
                anaTalep.Durum = TalepDurumu.YonetimKuruluOnayiBekliyor;
                successMessage = "Teklif seçildi. Tutar limiti aştığı için Yönetim Kurulu Başkanı onayına gönderildi.";
            }

            _talepRepository.Update(anaTalep);
            _talepRepository.SaveChanges();

            return successMessage;
        }
    }
}
