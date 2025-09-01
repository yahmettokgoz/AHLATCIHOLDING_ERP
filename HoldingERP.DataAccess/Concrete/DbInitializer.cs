using HoldingERP.DataAccess.Context;
using HoldingERP.Entities.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HoldingERP.DataAccess.Concrete
{
    public class DbInitializer
    {
        public static async Task Initialize(AppDbContext context, UserManager<Kullanici> userManager, RoleManager<Rol> roleManager)
        {
            context.Database.Migrate();

            if (!context.Departmanlar.Any())
            {
                context.Departmanlar.AddRange(
                    new Departman { Ad = "Yönetim" },
                    new Departman { Ad = "Teknik Destek" },
                    new Departman { Ad = "Satın Alma" },
                    new Departman { Ad = "Muhasebe" },
                    new Departman { Ad = "İnsan Kaynakları" },
                    new Departman { Ad = "Çağrı Merkezi" },
                    new Departman { Ad = "Depo" }
                );
                await context.SaveChangesAsync();
            }

            if (!context.Urunler.Any())
            {
                context.Urunler.AddRange(
        new Urun { Ad = "Klavye", Birim = "Adet", Aciklama = "Standart Q Klavye" },
        new Urun { Ad = "Mouse", Birim = "Adet", Aciklama = "Optik Kablosuz Mouse" },
        new Urun { Ad = "Monitör", Birim = "Adet", Aciklama = "24 inch Full HD Monitör" },
        new Urun { Ad = "Yazıcı Kartuşu", Birim = "Adet", Aciklama = "Siyah Renk" },
        new Urun { Ad = "Toplantı Masası", Birim = "Adet", Aciklama = "8 Kişilik" },
        new Urun { Ad = "Router", Birim = "Adet", Aciklama = "Kablosuz Ağ Yönlendirici" },
        new Urun { Ad = "Switch", Birim = "Adet", Aciklama = "24 Port Gigabit Switch" },
        new Urun { Ad = "Firewall Cihazı", Birim = "Adet", Aciklama = "Güvenlik Duvarı Donanımı" },
        new Urun { Ad = "Tablet", Birim = "Adet", Aciklama = "10 inch Android Tablet" },
        new Urun { Ad = "Kulaklık", Birim = "Adet", Aciklama = "Mikrofonlu Ofis Tipi Kulaklık" },
        new Urun { Ad = "Güvenlik Kamerası", Birim = "Adet", Aciklama = "IP Dome Kamera" },
        new Urun { Ad = "Arşiv Dolabı", Birim = "Adet", Aciklama = "Çelik Klasör Dolabı" },
        new Urun { Ad = "Projeksiyon Cihazı", Birim = "Adet", Aciklama = "Full HD projeksiyon" }
    );

            }

            string[] roller = { "Admin", "Talep Eden", "Onaycı", "Satın Almacı", "Muhasebe", "Depo", "İnsan Kaynakları" };
            foreach (var rol in roller)
            {
                if (!await roleManager.RoleExistsAsync(rol))
                {
                    await roleManager.CreateAsync(new Rol { Name = rol });
                }
            }

            if (await userManager.FindByNameAsync("admin") == null)
            {
                var yonetimDepartmani = await context.Departmanlar.FirstOrDefaultAsync(d => d.Ad == "Yönetim");
                if (yonetimDepartmani != null)
                {
                    var adminUser = new Kullanici
                    {
                        UserName = "admin",
                        Email = "admin@holding.com",
                        AdSoyad = "Sistem Yöneticisi",
                        AktifMi = true,
                        OlusturmaTarihi = DateTime.Now,
                        DepartmanId = yonetimDepartmani.Id
                    };

                    var result = await userManager.CreateAsync(adminUser, "Password123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                        await userManager.AddToRoleAsync(adminUser, "Onaycı");
                    }
                }
            }
        }
    }
}