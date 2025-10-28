# Templates
static resource manager contains pre-defined templates, this template can be extended.
templates are simple way to create qr-code they convert to `QrRenderOption` and are then used by api

```csharp
public record TemplateDataItem
{
    public int Code { get; init; }
    public string BodyColor { get; init; }
    public string FrameColor { get; init; }
    public string BallColor { get; init; }
    public string BackgroundColor { get; init; }
    public int QuietZonePixel { get; init; }
    public byte Frame { get; init; }
    public byte Ball { get; init; }
    public byte Body { get; init; }
    public string Logo { get; init; }
    public string Name { get; init; }
    public string BodyColorGradient { get; init; }
    public byte? GradientOnEyes { get; init; }
    public byte? GradientType { get; init; }
}
```

sample template

```json
{
    "Code": "4",
    "BodyColor": "#2C4270",
    "FrameColor": "#3B5998",
    "BallColor": "#3B5998",
    "BackgroundColor": "#FFFFFF",
    "QuietZonePixel": "60",
    "Frame": "2",
    "Ball": "2",
    "Body": "15",
    "Logo": "facebook.svg",    
    "Name": "Facebook-Gradient",
    "BodyColorGradient": "#476CB9",
    "GradientOnEyes": "0",
    "GradientType": "1"
}
```

colors in all configs are stored as html colors, and there is also logo property 
this resolve by logo storage, default static logo storage reads files from physical path.

`GradientOnEyes` can be 0,1 when it is 1 then the gradient applies to eyes of qrcode

| <img src="/Asset/template/13.png" width="" alt="13">  | <img src="/Asset/template/14.png" width="" alt="14"> |
|:---:|:--:|
| `GradientOnEyes = 0` | `GradientOnEyes = 1` |

`GradientType` can be set to 

```csharp
public enum QrColorType
{
    Solid = 0,
    LinearGradient = 1,
    RadialGradient = 2
}
```

gradiant colors are first `BodyColor` and second `BodyColorGradient`

there is `QuietZonePixel` around the main barcode that makes the qrcode reader read the code better, this property can calculate automatically if not set.

| <img src="/Asset/template/1.png" width="" alt="1">  | <img src="/Asset/template/2.png" width="" alt="2"> |<img src="/Asset/template/3.png" width="" alt="3"> |
|:---:|:--:|:---:|
| 1 | 2 | 3 |
| <img src="/Asset/template/4.png" width="" alt="4">  | <img src="/Asset/template/5.png" width="" alt="5"> |<img src="/Asset/template/6.png" width="" alt="6"> |
| 4 | 5 | 6 |
| <img src="/Asset/template/7.png" width="" alt="7">  | <img src="/Asset/template/8.png" width="" alt="8"> |<img src="/Asset/template/9.png" width="" alt="9"> |
| 7 | 8 | 9 |
| <img src="/Asset/template/10.png" width="" alt="10">  | <img src="/Asset/template/11.png" width="" alt="11"> |<img src="/Asset/template/12.png" width="" alt="12"> |
| 10 | 11 | 12 |
| <img src="/Asset/template/13.png" width="" alt="13">  | <img src="/Asset/template/14.png" width="" alt="14"> |<img src="/Asset/template/15.png" width="" alt="15"> |
| 13 | 14 | 15 |
| <img src="/Asset/template/16.png" width="" alt="16">  | <img src="/Asset/template/17.png" width="" alt="17"> |<img src="/Asset/template/18.png" width="" alt="18"> |
| 16 | 17 | 18 |
| <img src="/Asset/template/19.png" width="" alt="19">  | | |
| 19 |  |  |