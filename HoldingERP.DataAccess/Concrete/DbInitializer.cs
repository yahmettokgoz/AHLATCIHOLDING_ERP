using HoldingERP.DataAccess.Context;
using HoldingERP.Entities;
using HoldingERP.Entities.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoldingERP.DataAccess.Concrete
{
    public class DbInitializer
    {
        public static async Task Initialize(AppDbContext context, UserManager<Kullanici> userManager, RoleManager<Rol> roleManager)
        {
            // Departmanları ve Rolleri oluştur
            if (!context.Departmanlar.Any())
            {
                context.Departmanlar.AddRange(
                    new Departman { Ad = "Yönetim" },
                    new Departman { Ad = "Teknik Destek" },
                    new Departman { Ad = "Satın Alma" },
                    new Departman { Ad = "Muhasebe" },
                    new Departman { Ad = "İnsan Kaynakları" }
                );
                // Değişiklikleri hemen kaydet
                await context.SaveChangesAsync();
            }

            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new Rol { Name = "Admin" });
                await roleManager.CreateAsync(new Rol { Name = "Talep Eden" });
                await roleManager.CreateAsync(new Rol { Name = "Onaycı" });
                await roleManager.CreateAsync(new Rol { Name = "Satın Almacı" });
                await roleManager.CreateAsync(new Rol { Name = "Muhasebe" });
                await roleManager.CreateAsync(new Rol { Name = "Depo" });
            }

            // Admin kullanıcısını oluştur
            if (await userManager.FindByNameAsync("admin") == null)
            {
                // Yönetim departmanını bul
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
                        // Admin'e varsayılan olarak Onaycı rolünü de verelim
                        await userManager.AddToRoleAsync(adminUser, "Onaycı");
                    }
                }
            }
        }

    }
}
