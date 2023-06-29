using Rig.Sample.Web.Api;
using Rig.Sample.Web.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureDatabase();
builder.ConfigureJsonOptions();
builder.ConfigureApi();

var app = builder.Build();
app.MapApi();

if (app.Environment.IsDevelopment())
{
    await app.InitializeDatabase();
}

app.Run();
