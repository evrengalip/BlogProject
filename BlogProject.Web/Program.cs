using Microsoft.AspNetCore.Authentication.Cookies;
using NToastNotify;
using BlogProject.Web.Filters.ArticleVisitors;
using BlogProject.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// API servislerini kaydet
builder.Services.AddHttpClient();
builder.Services.AddSingleton<ApiClient>();
builder.Services.AddScoped<ArticleApiService>();
builder.Services.AddScoped<CategoryApiService>();
builder.Services.AddScoped<CommentApiService>();
builder.Services.AddScoped<DashboardApiService>();
builder.Services.AddScoped<AuthApiService>();
builder.Services.AddScoped<UserApiService>();
builder.Services.AddScoped<VisitorApiService>();

// Session yap�land�rmas�
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.MaxAge = null; // Kalıcı cookie kullanmıyoruz
});

// Add services to the container.
builder.Services.AddControllersWithViews(opt =>
{
    opt.Filters.Add<ArticleVisitorFilter>();
})
    .AddNToastNotifyToastr(new ToastrOptions()
    {
        PositionClass = ToastPositions.TopRight,
        TimeOut = 3000,
    })
    .AddRazorRuntimeCompilation();

// HttpContextAccessor ekliyoruz
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Kimlik do�rulama ayarlar� - Cookie tabanl� kimlik do�rulama
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "ApplicationCookie";
    options.DefaultSignInScheme = "ApplicationCookie";
    options.DefaultChallengeScheme = "ApplicationCookie";
})
.AddCookie("ApplicationCookie", options =>
{
    options.LoginPath = "/Admin/Auth/Login";
    options.LogoutPath = "/Admin/Auth/Logout";
    options.AccessDeniedPath = "/Admin/Auth/AccessDenied";
    options.Cookie.Name = "BlogProject";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Süreyi azalttık
    options.SlidingExpiration = false; // Sürekli yenilenmeyi kapattık
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseNToastNotify();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.Use(async (context, next) =>
{
    // Kimlik doğrulama gereken sayfalarda önbelleği devre dışı bırak
    if (context.Request.Path.StartsWithSegments("/Admin"))
    {
        context.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
        context.Response.Headers.Append("Pragma", "no-cache");
        context.Response.Headers.Append("Expires", "0");
    }

    await next();
});


app.UseRouting();

// Kimlik do�rulama ve yetkilendirme middleware'lar�n� do�ru s�rayla ekleyin
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapAreaControllerRoute(
    name: "Admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Home}/{action=Index}/{id?}"
    );
    endpoints.MapDefaultControllerRoute();
});

app.Run();