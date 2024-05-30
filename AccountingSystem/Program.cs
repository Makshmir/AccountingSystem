using AccountingSystem.Services;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ItemService>();
builder.Services.AddScoped<CheckService>();
builder.Services.AddScoped<SupplierService>();


var app = builder.Build();
IronPdf.License.LicenseKey = "IRONSUITE.SANCHOHMIR.META.UA.30126-3A03753998-CVNKUZ7-W2NXWWEBJNRA-B62MOWITS6T3-AG3X65XZFNI4-47VQDJAIY4ST-E47TMIRBEN2K-WQW4A5ZLF2LS-UYQVRC-TPTDJTD3MS2MUA-DEPLOYMENT.TRIAL-KWZNTA.TRIAL.EXPIRES.25.JUN.2024";
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

var defaultDateCulture = "en-US";
var ci = new CultureInfo(defaultDateCulture);
ci.NumberFormat.NumberDecimalSeparator = ".";
ci.NumberFormat.CurrencyDecimalSeparator = ".";

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(ci),
    SupportedCultures = new List<CultureInfo> { ci },
    SupportedUICultures = new List<CultureInfo> { ci }
});


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
