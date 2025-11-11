// Program.cs
using StockhubWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Register HTTP services
builder.Services.AddScoped<IHttpService, HttpService>();

// Register application services
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Configure HTTP client with settings from appsettings.json
builder.Services.AddScoped(provider =>
{
    var httpService = provider.GetRequiredService<IHttpService>();
    var configuration = provider.GetRequiredService<IConfiguration>();

    var baseUrl = configuration["ApiSettings:BaseUrl"];
    var timeoutSeconds = configuration.GetValue<int>("ApiSettings:TimeoutSeconds", 30);

    if (!string.IsNullOrEmpty(baseUrl))
    {
        httpService.SetBaseUrl(baseUrl);
    }
    httpService.SetTimeout(TimeSpan.FromSeconds(timeoutSeconds));

    return httpService;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();