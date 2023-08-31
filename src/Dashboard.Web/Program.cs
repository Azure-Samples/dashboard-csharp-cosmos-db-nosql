using Dashboard.Web.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<Connection>()
    .Bind(builder.Configuration.GetSection(nameof(Connection)));

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddHttpClient();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();