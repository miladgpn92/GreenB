using Autofac;
using Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Services.Services;
using Services.Services.Email;
using SharedModels.Dtos;
using WebFramework.Configuration;
using SharedModels.CustomMapping;
using WebFramework.Middlewares;
using WebFramework.Swagger;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.FileProviders;
using Web.Middleware;
using AspNetCore.ReCaptcha;
using Data;
using Microsoft.AspNetCore.Identity;
using Entities;
using System.Text;
using Microsoft.Extensions.WebEncoders;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using Microsoft.AspNetCore.DataProtection;
using Services.Services.CMS.Setting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization();

var supportedCultures = new[]
         {
                new CultureInfo("en-US"),
                new CultureInfo("fa-IR"),
            };


builder.Services.AddMemoryCache();

// Add services to the container.
// This configuration for AutoFact 
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.AddServices();
    });


// Read Setting
builder.Services.Configure<ProjectSettings>(builder.Configuration.GetSection(nameof(ProjectSettings)));

var _siteSetting = builder.Configuration.GetSection(nameof(ProjectSettings)).Get<ProjectSettings>();


// Auto Mapper Configuration
#region Auto Mapper Configuration
var sharedAssembly = typeof(AuthDto).Assembly;
var apiAssembly = typeof(SharedModels.Dtos.AuthDto).Assembly;

builder.Services.InitializeAutoMapper(sharedAssembly, apiAssembly);
#endregion




builder.Services.AddSingleton<IFileProvider>(
    new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot")));


// Database Configuration
builder.Services.AddDbContext(builder.Configuration);



//// JWT Configuration
builder.Services.AddJwtAuthentication(_siteSetting?.JwtSettings);


// Identity Configuration
builder.Services.AddCustomIdentity(_siteSetting?.IdentitySettings);


// MVC Configuration
builder.Services.AddMinimalMvc();

builder.Services.Configure<WebEncoderOptions>(options =>
{
    options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
});



// Version Configuration
builder.Services.AddCustomApiVersioning();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = new PathString("/auth/login");
    options.AccessDeniedPath = new PathString("/auth/accessDenied");
});



// Razor Configuration
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();



// Swagger Configuration
builder.Services.AddSwagger();

// Http Injection 
builder.Services.AddHttpClient<ISMSService, SMSService>();
builder.Services.AddHttpClient<IEmailService, EmailService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificSites", builder =>
    {
        // Read the list of websites from the text file
        var websites = File.ReadAllLines("wwwroot/cors-websites.txt");

        builder.WithOrigins(websites)
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// Add ReCaptcha
builder.Services.AddReCaptcha(builder.Configuration.GetSection("ReCaptcha"));
builder.Services.ConfigureApplicationCookie(options => { options.ExpireTimeSpan = TimeSpan.FromDays(30); });
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(Directory.GetCurrentDirectory())).SetDefaultKeyLifetime(TimeSpan.FromDays(30));

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.SmallestSize;
});


builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
});




 


var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/Error500");
    app.UseStatusCodePagesWithReExecute("/Error/Error{0}");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    //app.UseExceptionHandler("/Error/Error500");
    //app.UseStatusCodePagesWithReExecute("/Error/Error{0}");
     app.UseCustomExceptionHandler();
}
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("fa-IR"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

localizationOptions.RequestCultureProviders.Clear(); // This will clear all providers
app.UseRequestLocalization(localizationOptions);

app.UseHttpsRedirection();
app.UseStaticFiles();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwaggerAndUI();
//}
app.UseSwaggerAndUI();
app.UseCors("AllowSpecificSites");
app.UseRouting();

app.UseMiddleware<Error401Middleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ActiveSessionMiddleware>();

app.UseResponseCompression();

app.MapRazorPages();

app.MapControllers();

app.UseRateLimiter();

app.Run();
