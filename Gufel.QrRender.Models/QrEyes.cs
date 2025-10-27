using System.Drawing;

namespace Gufel.QrRender.Models;

public class QrEyes
{
    public QrEye UpperLeft { get; set; } = new QrEye();

    public QrEye UpperRight { get; set; } = new QrEye();

    public QrEye LowerLeft { get; set; } = new QrEye();

    public bool HasGradient { get; set; } = false;

    public void SetFrameColor(Color color)
    {
        UpperLeft.FrameColor = color;
        UpperRight.FrameColor = color;
        LowerLeft.FrameColor = color;
    }

    public void SetBallColor(Color color)
    {
        UpperLeft.BallColor = color;
        UpperRight.BallColor = color;
        LowerLeft.BallColor = color;
    }

    public void SetFrame(byte frame)
    {
        UpperLeft.Frame = frame;
        UpperRight.Frame = frame;
        LowerLeft.Frame = frame;
    }

    public void SetBall(byte ball)
    {
        UpperLeft.Ball = ball;
        UpperRight.Ball = ball;
        LowerLeft.Ball = ball;
    }
}