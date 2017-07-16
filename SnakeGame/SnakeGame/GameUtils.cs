using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class ImageUtils
{

    public static System.Drawing.Bitmap thumbImage(System.Drawing.Image srcBitmap, int width, int height)
    {
        var bitmap = new System.Drawing.Bitmap(width, height);

        float scaleRateW = bitmap.Width / (float)srcBitmap.Width;
        float scaleRateH = bitmap.Height / (float)srcBitmap.Height;
        float scaleRate = Math.Min(scaleRateW, scaleRateH);

        int width2 = (int)(srcBitmap.Width * scaleRate);
        int height2 = (int)(srcBitmap.Height * scaleRate);

        int x2 = bitmap.Width / 2 - width2 / 2;
        int y2 = bitmap.Height / 2 - height2 / 2;

        var destRect = new System.Drawing.Rectangle(x2, y2, width2, height2);

        using (var g = System.Drawing.Graphics.FromImage(bitmap))
        {
            g.DrawImage(srcBitmap, destRect, new System.Drawing.Rectangle(0, 0, srcBitmap.Width, srcBitmap.Height), System.Drawing.GraphicsUnit.Pixel);

            // g.DrawRectangle(System.Drawing.Pens.Red, new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height));
        }

        return bitmap;
    }
}


class ArrayUtils
{
    public static T[,] create2DArray<T>(int width, int height, T value)
    {
        T[,] map = new T[height, width];

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                map[y, x] = value;
            }
        }
        return map;
    }
}


class EaseUtils
{
    public static double easeLinear(double t, double b, double c, double d)
    {
        return c * t / d + b;
    }

    public static double easeOutCubic(double t, double b, double c, double d)
    {
        t /= d;
        t--;
        return c * (t * t * t + 1) + b;
    }

    public static double easeInCubic(double t, double b, double c, double d)
    {
        t /= d / 2;
        if (t < 1) return c / 2 * t * t * t + b;
        t -= 2;
        return c / 2 * (t * t * t + 2) + b;
    }

    public static double easeInElastic(double t, double b, double c, double d)
    {

        return (0.04 - 0.04 / t) * Math.Sin(25 * t) + 1;
    }

    public static double easeOutElastic(double t, double b, double c, double d)
    {

        return 0.04 * t / (--t) * Math.Sin(25 * t);
    }

    public static double easeInOutElastic(double t, double b, double c, double d)
    {
        t -= 0.5;

        if (t < 0)
        {
            return (0.01 + 0.01 / t) * Math.Sin(50 * t);
        }

        return (0.02 - 0.01 / t) * Math.Sin(50 * t) + 1;
    }
}

