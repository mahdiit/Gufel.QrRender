# Gufel Advanced QrCode Svg Render
Based on [`QRCoder`](https://github.com/Shane32/QRCoder) [`nuget`](https://www.nuget.org/packages/QRCoder) package, this package exports qrcode as high quality verctor svg format with customization and templates. You can also export svg as png or pdf with helper services and tools.

you can build high quality svg qrcode with 20 predefined template.

| <img src="/Asset/template/13.png" width="60%" alt="13">  | <img src="/Asset/template/15.png" width="60%" alt="14"> |
|:---:|:--:|

## Features:

1. ⭐ Customization of eye color, eye frame and eye
2. ⭐ Gradient customization
3. ⭐ Ability to define and rotate eye graphics based on placement
4. ⭐ Ability to customize all graphics by defining dedicated storage
5. ⭐ Template definition to simplify production
6. ⭐ PDF and PNG output helper
7. ⭐ Sample and Dockerized project
8. ⭐ Outputs work on all operating systems: Linux, Mac and Windows

| <img src="/Asset/diff/Vectorized.png" width="60%" alt="13">  | <img src="/Asset/diff/Rasterisation.png" width="60%" alt="14"> |
|:---:|:--:|
| Vectorized | Rasterized |

## ☕ Buy Me a Coffee (Crypto)

If you'd like to support this project, you can send crypto to:

**Bitcoin (BTC)**  `18gMqPRoLmG9vMvFYgdREpAtYPafYC2r5B`

**Dogecoin (DOGE)** `DNrtbptXEpWjniJ7zTMH14namJd38494Sv`


## Sample:
To start the creation of qrcode, you need qrcode data that can be generated using payload or simple text. reffer to QRCoder for more information. 

```csharp
using var qrCodeData = QRCodeGenerator.GenerateQrCode("https://github.com/mahdiit", QRCodeGenerator.ECCLevel.M);
```
main class must load resources and svg graphics for creating qrcode, so we need to register default implementation and static content for resource:
```csharp
services.AddSingleton<ILogoRepository, StaticLogoRepository>();
services.AddSingleton<ILogoLoader, LogoLoader>();
services.AddSingleton<IResourceRepository, StaticResourceRepository>();
```
> note: default resource manger implemented and use static resource that embed in assembly file

next pass `qrCodeData` to main svg creator:
```csharp
 using var svgRenderer = new SvgQrCode(app.GetRequiredService<IResourceRepository>());
 svgRenderer.SetQRCodeData(qrCodeData);
```
to complete creation of high quality vector qrcode you need to pass [options](/Asset/graphics.md) or template. simple way is predefined template. 
see [template list](/Asset/template.md)
```csharp
svgRenderer.GetGraphic(templateId: 1).SvgContent
```
## Convert Helper
there are 3 helpers for converting svg content to pdf or png and each of them has pros and cons and there is a commercial library that can help. in this library use a free/open source and fast solution.

| Method       | Platform           | Free | Quality    |
| ------------ | ------------------ | ---- | ---------- |
| ⭐ Svg.Skia     | Cross-platform     | ✅    | Excellent  |
| ⭐ Magick.NET   | Cross-platform     | ✅    | Excellent  |
| ⭐ Inkscape CLI | Any OS w/ Inkscape | ✅    | Very high  |
| Aspose.SVG   | Any                | ❌    | Enterprise |

| Method         | Vector PDF | Cross-Platform | Free | Dependencies         |
| -------------- | ---------- | -------------- | ---- | -------------------- |
| Svg.Skia       | ✅          | ✅              | ✅    | SkiaSharp + Svg.Skia |
| Aspose.SVG     | ✅          | ✅              | ❌    | Paid library         |
| Magick.NET     | ❌ (raster) | ✅              | ✅    | ImageMagick core     |
| Inkscape CLI   | ✅          | ✅              | ✅    | Inkscape installed   |


### Inkscape Cli
for setting inkscape paramater see [official inkscape documentation](https://inkscape.org/doc/inkscape-man.html)

for working with inkscape, first install Inkscape in os or docker image and then pass execlutable file to helper service:
```csharp
// Windows 
// C:\Program Files\Inkscape\bin\inkscape.exe
// Linux
// usr/bin/inkscape
var inkscape = new InkscapeExportBuilder(path);
inkscape.ExportType([InkscapeExportTypes.pdf, InkscapeExportTypes.png]);
inkscape.InputFile(svgPath);

// default output path is input folder
await inkscape.Execute();
```
install docker file sample:
```bash
USER root
# Install Inkscape
RUN apt-get update && \
    apt-get install -y inkscape && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Print Inkscape path and set environment variable
RUN which inkscape
ENV INKSCAPE_PATH=/usr/bin/inkscape
```
### SkiaSharp
```csharp
SkiaSharpExport.SvgToPdf(svgPath, svgPath + "-skia.pdf");
SkiaSharpExport.SvgToPng(svgPath, svgPath + "-skia.png");
```

### MagickNet
```csharp
MagickNetExport.SvgToPdf(svgPath, svgPath + "-magick.pdf");
MagickNetExport.SvgToPng(svgPath, svgPath + "-magick.png");
```
## License
By using this software, you agree to the terms of either the GPLv3 license or the commercial license, depending on your use in [LICENSE](License.md).

