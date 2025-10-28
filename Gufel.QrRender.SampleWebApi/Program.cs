
using Gufel.QrRender.Models.Storage;
using Gufel.QrRender.Providers.License;
using Gufel.QrRender.Providers.Storage;
using System.Text.Json.Serialization;

namespace Gufel.QrRender.SampleWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            LicenseManager.SetLicense(LicenseType.OpenSource);

            // Add services to the container.

            builder.Services.AddSingleton<ILogoRepository>(_ => new PhysicalLogoRepository("wwwroot/asset/icons/"));
            builder.Services.AddSingleton<ILogoLoader, LogoLoader>();
            builder.Services.AddSingleton<IResourceStorage, StaticResourceStorage>();

            builder.Services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.MapOpenApi();

            app.UseStaticFiles();

            app.UseSwagger();

            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
