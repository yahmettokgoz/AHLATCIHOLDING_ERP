using HoldingERP.Business.Abstract;
using HoldingERP.Business.Concrete;
using HoldingERP.DataAccess.Abstract;
using HoldingERP.DataAccess.Concrete;
using HoldingERP.DataAccess.Context;
using HoldingERP.Entities.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<Kullanici, Rol>()
    .AddEntityFrameworkStores<AppDbContext>();


builder.Services.AddScoped(typeof(IRepository<>), typeof(EFRepository<>));

builder.Services.AddScoped<IDepartmanService, DepartmanManager>();
builder.Services.AddScoped<IFaturaService, FaturaManager>();
builder.Services.AddScoped<ISatinAlmaService, SatinAlmaManager>();
builder.Services.AddScoped<IStokHareketiService, StokHareketiManager>();
builder.Services.AddScoped<IStokService, StokManager>();
builder.Services.AddScoped<ITalepService, TalepManager>();
builder.Services.AddScoped<ITedarikciService, TedarikciManager>();
builder.Services.AddScoped<ITeklifService, TeklifManager>();
builder.Services.AddScoped<IUrunService, UrunManager>();
builder.Services.AddScoped<ITalepUrunService, TalepUrunManager>();
builder.Services.AddScoped<IStokHareketiService, StokHareketiManager>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
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

    context.Database.Migrate();

    await DbInitializer.Initialize(context, userManager, roleManager);
}


app.Run();