# Gufel Qr Render
Based on [`QRCoder`](https://github.com/Shane32/QRCoder) [`nuget`](https://www.nuget.org/packages/QRCoder) package, this package export qrcode as hight quality verctor svg format with customization and template. you can also export svg as png or pdf with helper service and tools.

you can build hight qulaity svg qr code with 20 predefined template.

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
8. ⭐ Outputs work on all operating systems, Linux, Mac and Windows

| <img src="/Asset/diff/Vectorized.png" width="60%" alt="13">  | <img src="/Asset/diff/Rasterisation.png" width="60%" alt="14"> |
|:---:|:--:|
| Vectorized | Rasterized |

## Sample:
for start of createtion of qrcode, you need qrcode data that can be generate using payload or simple text. reffer to QRCoder for more information. 

```csharp
using var qrCodeData = QRCodeGenerator.GenerateQrCode("https://github.com/mahdiit", QRCodeGenerator.ECCLevel.M);
```

