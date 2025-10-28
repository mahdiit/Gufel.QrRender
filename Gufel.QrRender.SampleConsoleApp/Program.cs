using System.Diagnostics;
using Gufel.QrRender.Models.Storage;
using Gufel.QrRender.Providers;
using Gufel.QrRender.Providers.Export.Inkscape;
using Gufel.QrRender.Providers.Export.Magick;
using Gufel.QrRender.Providers.Export.Skia;
using Gufel.QrRender.Providers.License;
using Gufel.QrRender.Providers.Storage;
using Microsoft.Extensions.DependencyInjection;
using QRCoder;

namespace Gufel.QrRender.SampleConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Start");
            LicenseManager.SetLicense(LicenseType.OpenSource);

            var services = new ServiceCollection();
            services.AddSingleton<ILogoRepository>(_ => new PhysicalLogoRepository("Asset/Icons/"));
            services.AddSingleton<ILogoLoader, LogoLoader>();
            services.AddSingleton<IResourceStorage, StaticResourceStorage>();

            var app = services.BuildServiceProvider();

            using var qrCodeData = QRCodeGenerator.GenerateQrCode("https://github.com/mahdiit", QRCodeGenerator.ECCLevel.M);
            using var svgRenderer = new SvgQrCode(app.GetRequiredService<IResourceStorage>());
            svgRenderer.SetQRCodeData(qrCodeData);

            var dir = new DirectoryInfo("svg-output");
            if (!dir.Exists)
                dir.Create();

            var svgFiles = dir.GetFiles("*.svg");
            var inkscape = new InkscapeExportBuilder(@"C:\Program Files\Inkscape\bin\inkscape.exe");
            inkscape.ExportType([InkscapeExportTypes.pdf, InkscapeExportTypes.png]);

            var timer = Stopwatch.StartNew();
            for (var i = 1; i <= 19; i++)
            {
                Console.WriteLine($"--- Export {i} ---");
                var svgPath = Path.Combine(dir.FullName, $"output-template-{i}.svg");
                File.WriteAllText(svgPath, svgRenderer.GetGraphic(templateId: i).SvgContent);

                Console.WriteLine("#Inkscape");
                inkscape.InputFile(svgPath);
                await inkscape.Execute();
                Console.WriteLine(timer.Elapsed.ToString("G"));
                timer.Restart();
                Console.WriteLine();

                Console.WriteLine("#Skia");
                SkiaSharpExport.SvgToPdf(svgPath, svgPath + "-skia.pdf");
                SkiaSharpExport.SvgToPng(svgPath, svgPath + "-skia.png");
                Console.WriteLine(timer.Elapsed.ToString("G"));
                timer.Restart();
                Console.WriteLine();

                Console.WriteLine("#Magick");
                MagickNetExport.SvgToPdf(svgPath, svgPath + "-magick.pdf");
                MagickNetExport.SvgToPng(svgPath, svgPath + "-magick.png");
                Console.WriteLine(timer.Elapsed.ToString("G"));
                timer.Restart();
                Console.WriteLine();
            }

            Console.WriteLine("press any key...");
            Console.ReadKey();
        }
    }
}
