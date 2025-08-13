using HoldingERP.Business.Abstract;
using HoldingERP.Business.Concrete;
using HoldingERP.DataAccess.Abstract;
using HoldingERP.DataAccess.Concrete;
using HoldingERP.DataAccess.Context;
using HoldingERP.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddIdentity<Kullanici, Rol>()
    .AddEntityFrameworkStores<AppDbContext>();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(EFRepository<>));

builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericManager<>));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    var userManager = services.GetRequiredService<UserManager<Kullanici>>();
    var roleManager = services.GetRequiredService<RoleManager<Rol>>();

    context.Database.EnsureCreated();

    await DbInitializer.Initialize(context, userManager, roleManager);
}



app.Run();
