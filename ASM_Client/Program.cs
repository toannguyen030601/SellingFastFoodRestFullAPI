using ASM_api.DB;
using ASM_api.Services;
using ASM_Client.Services;
using AspNetCoreHero.ToastNotification;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<net105AsmDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDB"))
);

//đăng kí servives
builder.Services.AddSingleton<IVnPaySvc, VnPaySvc>();

//đăng kí session
builder.Services.AddSession();

//đăng kí thông báo
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 3;
    config.IsDismissable = true;
    config.Position = NotyfPosition.TopRight;
});

//dang ki http
builder.Services.AddHttpClient();

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
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
