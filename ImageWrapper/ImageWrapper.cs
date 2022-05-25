using System.Drawing;

namespace ImageWrapper;

public class ImageWrapper
{
    private readonly int _width;
    private readonly int _height;
    private readonly Color[,] _colorMap;

    public ImageWrapper(string image)
    {
        var img = new Bitmap(image);
        _width = img.Width;
        _height = img.Height;

        _colorMap = new Color[img.Width, img.Height];

        for (var i = 0; i < img.Width; i++)
        {
            for (var j = 0; j < img.Height; j++)
            {
                _colorMap[i, j] = img.GetPixel(i, j);
            }
        }
    }

    public Bitmap Equalization(Func<Color, byte> getAlgorithm, Func<Color, byte, Color> setAlgorithm)
    {
        const int size = byte.MaxValue + 1;
        var h = new int[size];
        var hN = new double[size];

        for (var i = 0; i < _width; i++)
        {
            for (var j = 0; j < _height; j++)
            {
                h[GetBright(i, j, getAlgorithm)]++;
            }
        }

        var hTotal = h.Sum();
        hN = h.Select(x => (double)x / hTotal).ToArray();

        var sh = new double[size];

        sh[0] = hN[0];
        for (var i = 1; i < sh.Length; i++)
        {
            sh[i] = sh[i - 1] + hN[i];
        }

        var bm = new Bitmap(_width, _height);
        for (var i = 0; i < _width; i++)
        {
            for (var j = 0; j < _height; j++)
            {
                bm.SetPixel(i, j, setAlgorithm(_colorMap[i, j], (byte)Math.Round(255 * sh[GetBright(i, j, getAlgorithm)])));
            }
        }

        return bm;
    }

    public Bitmap LinearContrast(Func<Color, byte> getAlgorithm, Func<Color, byte, Color> setAlgorithm)
    {
        var bm = new Bitmap(_width, _height);

        byte min = byte.MaxValue;
        byte max = 0;

        for (var i = 0; i < _width; i++)
        {
            for (var j = 0; j < _height; j++)
            {
                var temp = GetBright(i, j, getAlgorithm);
                if (temp < min)
                {
                    min = temp;
                }

                if (temp > max)
                {
                    max = temp;
                }
            }
        }

        for (var i = 0; i < _width; i++)
        {
            for (var j = 0; j < _height; j++)
            {
                var newValue = (double)(byte.MaxValue * (GetBright(i, j, getAlgorithm) - min)) / (max - min);

                bm.SetPixel(i, j, SetBright(i, j, (byte)Math.Round(newValue), setAlgorithm));
            }
        }

        return bm;
    }

    public Bitmap MedianFilter(int radius, Func<Color, byte> getAlgorithm, Func<Color, byte, Color> setAlgorithm)
    {
        var bm = new Bitmap(_width, _height);
        var temp = new byte[(radius * 2 + 1) * (radius * 2 + 1)];
        var middle = temp.Length / 2;

        for (var i = 0; i < _width; i++)
        {
            for (var j = 0; j < _height; j++)
            {
                if (i < radius || j < radius || i >= _width - radius || j >= _height - radius)
                {
                    bm.SetPixel(i, j, _colorMap[i, j]);
                }
                else
                {
                    int index = 0;
                    for (int n = i - radius; n <= i + radius; n++)
                    {
                        for (int m = j - radius; m <= j + radius; m++)
                        {
                            temp[index++] = GetBright(n, m, getAlgorithm);
                        }
                    }

                    Array.Sort(temp);

                    bm.SetPixel(i, j, setAlgorithm(_colorMap[i, j], temp[middle]));
                }
            }
        }

        return bm;
    }

    private byte GetBright(int i, int j, Func<Color, byte> algorithm)
    {
        return algorithm(_colorMap[i, j]);
    }

    private Color SetBright(int i, int j, byte newValue, Func<Color, byte, Color> algorithm)
    {
        return algorithm(_colorMap[i, j], newValue);
    }
}