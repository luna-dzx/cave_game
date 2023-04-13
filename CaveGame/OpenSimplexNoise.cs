/* OpenSimplex Noise in C#
 * Ported from https://gist.github.com/KdotJPG/b1270127455a94ac5d19
 * and heavily refactored to improve performance. */

using System;
using System.Runtime.CompilerServices;

namespace CaveGame;

public class OpenSimplexNoise
{
    private const double Stretch2D = -0.211324865405187;    //(1/Math.sqrt(2+1)-1)/2;
    private const double Squish2D = 0.366025403784439;      //(Math.sqrt(2+1)-1)/2;
    private const double Norm2D = 1.0 / 47.0;

    private byte[] _perm;
    private byte[] _perm2D;

    private static double[] _gradients2D =
    {
        5,  2,    2,  5,
        -5,  2,   -2,  5,
        5, -2,    2, -5,
        -5, -2,   -2, -5,
    };
    
    private static Contribution2[] _lookup2D;

    static OpenSimplexNoise()
    {
        var base2D = new[]
        {
            new[] { 1, 1, 0, 1, 0, 1, 0, 0, 0 },
            new[] { 1, 1, 0, 1, 0, 1, 2, 1, 1 }
        };
        var p2D = new[] { 0, 0, 1, -1, 0, 0, -1, 1, 0, 2, 1, 1, 1, 2, 2, 0, 1, 2, 0, 2, 1, 0, 0, 0 };
        var lookupPairs2D = new[] { 0, 1, 1, 0, 4, 1, 17, 0, 20, 2, 21, 2, 22, 5, 23, 5, 26, 4, 39, 3, 42, 4, 43, 3 };

        var contributions2D = new Contribution2[p2D.Length / 4];
        for (int i = 0; i < p2D.Length; i += 4)
        {
            var baseSet = base2D[p2D[i]];
            Contribution2 previous = null, current = null;
            for (int k = 0; k < baseSet.Length; k += 3)
            {
                current = new Contribution2(baseSet[k], baseSet[k + 1], baseSet[k + 2]);
                if (previous == null)
                {
                    contributions2D[i / 4] = current;
                }
                else
                {
                    previous.Next = current;
                }
                previous = current;
            }
            current!.Next = new Contribution2(p2D[i + 1], p2D[i + 2], p2D[i + 3]);
        }

        _lookup2D = new Contribution2[64];
        for (var i = 0; i < lookupPairs2D.Length; i += 2)
        {
            _lookup2D[lookupPairs2D[i]] = contributions2D[lookupPairs2D[i + 1]];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FastFloor(double x)
    {
        var xi = (int)x;
        return x < xi ? xi - 1 : xi;
    }

    public OpenSimplexNoise()
        : this(DateTime.Now.Ticks)
    {
    }

    private OpenSimplexNoise(long seed)
    {
        _perm = new byte[256];
        _perm2D = new byte[256];

        var source = new byte[256];
        for (int i = 0; i < 256; i++)
        {
            source[i] = (byte)i;
        }
        seed = seed * 6364136223846793005L + 1442695040888963407L;
        seed = seed * 6364136223846793005L + 1442695040888963407L;
        seed = seed * 6364136223846793005L + 1442695040888963407L;
        for (int i = 255; i >= 0; i--)
        {
            seed = seed * 6364136223846793005L + 1442695040888963407L;
            int r = (int)((seed + 31) % (i + 1));
            if (r < 0)
            {
                r += (i + 1);
            }
            _perm[i] = source[r];
            _perm2D[i] = (byte)(_perm[i] & 0x0E);
            source[r] = source[i];
        }
    }

    public double Evaluate(double x, double y)
    {
        double stretchOffset = (x + y) * Stretch2D;
        double xs = x + stretchOffset;
        double ys = y + stretchOffset;

        int xsb = FastFloor(xs);
        int ysb = FastFloor(ys);

        double squishOffset = (xsb + ysb) * Squish2D;
        double dx0 = x - (xsb + squishOffset);
        double dy0 = y - (ysb + squishOffset);

        double xins = xs - xsb;
        double yins = ys - ysb;

        double inSum = xins + yins;

        int hash =
            (int)(xins - yins + 1) |
            (int)(inSum) << 1 |
            (int)(inSum + yins) << 2 |
            (int)(inSum + xins) << 4;

        Contribution2 c = _lookup2D[hash];

        double value = 0.0;
        while (c != null)
        {
            double dx = dx0 + c.Dx;
            double dy = dy0 + c.Dy;
            double attn = 2 - dx * dx - dy * dy;
            if (attn > 0)
            {
                int px = xsb + c.Xsb;
                int py = ysb + c.Ysb;

                byte i = _perm2D[(_perm[px & 0xFF] + py) & 0xFF];
                double valuePart = _gradients2D[i] * dx + _gradients2D[i + 1] * dy;

                attn *= attn;
                value += attn * attn * valuePart;
            }
            c = c.Next;
        }
        return value * Norm2D;
    }

    private class Contribution2
    {
        public double Dx, Dy;
        public int Xsb, Ysb;
        public Contribution2 Next;

        public Contribution2(double multiplier, int xsb, int ysb)
        {
            Dx = -xsb - multiplier * Squish2D;
            Dy = -ysb - multiplier * Squish2D;
            Xsb = xsb;
            Ysb = ysb;
        }
    }
}