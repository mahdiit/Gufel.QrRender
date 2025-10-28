# Graphics
there are tree types of graphics as static resource manager, 
this resource can be extended or use a custom resource manager. below is data from the static resource manager

## How were graphics resolved?
there is `IResourceRepository` that resolves based on graphic ids
```csharp
public interface IResourceRepository
{
    GraphicDataItem GetGraphic(string id, QrGraphicType graphicType);

    QrRenderOption GetTemplate(int templateId);
}
```
and there is `GraphicDataItem` that qrcode built with it.
```csharp
public enum QrGraphicType
{
    Frame = 1,
    Ball = 2,
    Body = 3
}

public record GraphicDataItem
    {
    //internal id, not using by code
        public int Code { get; init; }
    
    //this id used by template and options
        public string GraphicID { get; init; }

    //based on QrGraphicType, frame, ball, body
        public byte GraphicTypeCode { get; init; }

    //svg content of item
        public string GraphicData { get; init; }

    //for ball and eye these property use as rotation
        public string TopLeft { get; init; }
        public string TopRight { get; init; }
        public string BottomLeft { get; init; }

    //size of vector
        public int GraphicSize { get; init; }
    }
```

## Frame (Eye)
frames are around the ball eyes. can set from 0-14, automatically rotated based on side of eye

| ![frame0.png](/Asset/graphics/frame0.png) | ![frame1.png](/Asset/graphics/frame1.png) | ![frame2.png](/Asset/graphics/frame2.png) | ![frame3.png](/Asset/graphics/frame3.png) | ![frame4.png](/Asset/graphics/frame4.png) |
|:---:|:---:|:---:|:---:|:---:|
| 0 | 1 | 2 | 3 | 4 |
| ![frame5.png](/Asset/graphics/frame5.png) | ![frame6.png](/Asset/graphics/frame6.png) | ![frame7.png](/Asset/graphics/frame7.png) | ![frame8.png](/Asset/graphics/frame8.png) | ![frame9.png](/Asset/graphics/frame9.png) |
| 5 | 6 | 7 | 8 | 9 |
| ![frame10.png](/Asset/graphics/frame10.png) | ![frame11.png](/Asset/graphics/frame11.png) | ![frame12.png](/Asset/graphics/frame12.png) | ![frame13.png](/Asset/graphics/frame13.png) | ![frame14.png](/Asset/graphics/frame14.png) |
| 10 | 11 | 12 | 13 | 14 |

## Ball (Eye)
ball eye can be set from 0-17, automatically rotated based on the side of the eye

| ![ball0.png](/Asset/graphics/ball0.png) | ![ball1.png](/Asset/graphics/ball1.png) | ![ball2.png](/Asset/graphics/ball2.png) | ![ball3.png](/Asset/graphics/ball3.png) | ![ball4.png](/Asset/graphics/ball4.png) |
|:---:|:---:|:---:|:---:|:---:|
| 0 | 1 | 2 | 3 | 4 |
| ![ball5.png](/Asset/graphics/ball5.png) | ![ball6.png](/Asset/graphics/ball6.png) | ![ball7.png](/Asset/graphics/ball7.png) | ![ball8.png](/Asset/graphics/ball8.png) | ![ball9.png](/Asset/graphics/ball9.png) |
| 5 | 6 | 7 | 8 | 9 |
| ![ball10.png](/Asset/graphics/ball10.png) | ![ball11.png](/Asset/graphics/ball11.png) | ![ball12.png](/Asset/graphics/ball12.png) | ![ball13.png](/Asset/graphics/ball13.png) | ![ball14.png](/Asset/graphics/ball14.png) |
| 10 | 11 | 12 | 13 | 14 |
| ![ball15.png](/Asset/graphics/ball15.png) | ![ball16.png](/Asset/graphics/ball16.png) | ![ball17.png](/Asset/graphics/ball17.png) |  |  |
| 15 | 16 | 17 |  |  |

if a ball must rotate for specific position use svg annotation to rotate them using correct property.
for example `ball 0` in all position is same, but `ball 14` must rotate base on postion of eye so there is config:
```json
"TopLeft": "",
"TopRight": "translate(100, 0) scale(-1,1)",
"BottomLeft": "translate(0,100) scale(1,-1)"
```
so when ball postion change, scale and translate affect shape

## Body (Pixel)
body pixel can set from 0-21

| ![body0.png](/Asset/graphics/body0.png) | ![body1.png](/Asset/graphics/body1.png) | ![body2.png](/Asset/graphics/body2.png) | ![body3.png](/Asset/graphics/body3.png) | ![body4.png](/Asset/graphics/body4.png) |
|:---:|:---:|:---:|:---:|:---:|
| 0 | 1 | 2 | 3 | 4 |
| ![body5.png](/Asset/graphics/body5.png) | ![body6.png](/Asset/graphics/body6.png) | ![body7.png](/Asset/graphics/body7.png) | ![body8.png](/Asset/graphics/body8.png) | ![body9.png](/Asset/graphics/body9.png) |
| 5 | 6 | 7 | 8 | 9 |
| ![body10.png](/Asset/graphics/body10.png) | ![body11.png](/Asset/graphics/body11.png) | ![body12.png](/Asset/graphics/body12.png) | ![body13.png](/Asset/graphics/body13.png) | ![body14.png](/Asset/graphics/body14.png) |
| 10 | 11 | 12 | 13 | 14 |
| ![body15.png](/Asset/graphics/body15.png) | ![body16.png](/Asset/graphics/body16.png) | ![body17.png](/Asset/graphics/body17.png) | ![body18.png](/Asset/graphics/body18.png) | ![body19.png](/Asset/graphics/body19.png) |
| 15 | 16 | 17 | 18 | 19 |
| ![body20.png](/Asset/graphics/body20.png) | ![body21.png](/Asset/graphics/body21.png) |  |  |  |
| 20 | 21 |  |  |  |

there are algorithms for loading body pixels, each body pixel has 4 graphics. that indicate the position of a pixel based on neighbors.
example of ids:
* `5-0001`
* `5-0011`
* `5-0111`
* `5-1111`

it anticlockwise around the current pixel and allows different styles of pixels based on neighboring pixels. it allows that end or start pixel to make a different style.
for pixel that never change, you can set them the same. for example in body 21 all are the same.