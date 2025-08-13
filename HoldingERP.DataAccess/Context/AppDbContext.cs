
using HoldingERP.Entities;
using HoldingERP.Entities.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HoldingERP.DataAccess.Context
{
   
    public class AppDbContext : IdentityDbContext<Kullanici, Rol, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        
        public DbSet<Departman> Departmanlar { get; set; }
        public DbSet<Urun> Urunler { get; set; }
        public DbSet<Stok> Stoklar { get; set; }
        public DbSet<Tedarikci> Tedarikciler { get; set; }
        public DbSet<SatinAlmaTalebi> SatinAlmaTalepleri { get; set; }
        public DbSet<SatinAlmaTalepUrunu> SatinAlmaTalepUrunleri { get; set; }
        public DbSet<Teklif> Teklifler { get; set; }
        public DbSet<TeklifKalem> TeklifKalemleri { get; set; }
        public DbSet<Onay> Onaylar { get; set; }
        public DbSet<Fatura> Faturalar { get; set; }
        public DbSet<StokHareketi> StokHareketleri { get; set; }
        public DbSet<Bildirim> Bildirimler { get; set; }
        public DbSet<LogKaydi> LogKayitlari { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);

            
            
            modelBuilder.Entity<Teklif>()
                .HasOne(t => t.SatinAlmaTalebi)
                .WithMany(s => s.Teklifler)
                .HasForeignKey(t => t.SatinAlmaTalebiId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Teklif>()
                .HasOne(t => t.TeklifYapanKullanici)
                .WithMany(k => k.GirilenTeklifler)
                .HasForeignKey(t => t.TeklifYapanKullaniciId)
                .OnDelete(DeleteBehavior.Restrict);

     
            modelBuilder.Entity<Stok>().Property(p => p.Miktar).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<StokHareketi>().Property(p => p.Miktar).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<SatinAlmaTalepUrunu>().Property(p => p.Miktar).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Teklif>().Property(p => p.ToplamFiyat).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<TeklifKalem>().Property(p => p.BirimFiyat).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<TeklifKalem>().Property(p => p.Miktar).HasColumnType("decimal(18,2)");
        }
    }
}