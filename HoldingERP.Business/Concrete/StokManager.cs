using System;
using System.Collections.Generic;
using HoldingERP.Business.Abstract;
using HoldingERP.DataAccess.Abstract;
using HoldingERP.Entities.Concrete;

namespace HoldingERP.Business.Concrete
{
    public class StokManager : GenericManager<Stok>, IStokService
    {
        private readonly IStokHareketiService _stokHareketiService;

        public StokManager(
            IRepository<Stok> repository,
            IStokHareketiService stokHareketiService
        ) : base(repository)
        {
            _stokHareketiService = stokHareketiService;
        }

       
        public void FaturaIleStokGirisiYap(Fatura fatura, IEnumerable<TeklifKalem> kalemler, int talepId, int yapanKullaniciId)
        {
            if (kalemler == null) return;

            foreach (var kalem in kalemler)
            {
                var mevcutStok = Get(s => s.UrunId == kalem.UrunId);

                if (mevcutStok == null)
                {
                    mevcutStok = new Stok
                    {
                        UrunId = kalem.UrunId,
                        Miktar = kalem.Miktar,
                        Lokasyon = "Merkez Depo",
                        GuncellemeTarihi = DateTime.Now
                    };
                    Create(mevcutStok);
                }
                else
                {
                    mevcutStok.Miktar += kalem.Miktar;
                    mevcutStok.GuncellemeTarihi = DateTime.Now;
                    Update(mevcutStok);
                }

                var hareket = new StokHareketi
                {
                    UrunId = kalem.UrunId,
                    Miktar = kalem.Miktar,
                    IslemTuru = IslemTuru.Giris,
                    Tarih = DateTime.Now,
                    IslemiYapanKullaniciId = yapanKullaniciId,
                    FaturaId = fatura?.Id,
                    SatinAlmaTalebiId = talepId
                };
                _stokHareketiService.Create(hareket);
            }

            SaveChanges();
            _stokHareketiService.SaveChanges();
        }

    }
}
