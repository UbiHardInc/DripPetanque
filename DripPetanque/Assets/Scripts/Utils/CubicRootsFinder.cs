// From : https://github.com/mathnet/mathnet-numerics/blob/master/src/Numerics/RootFinding/Cubic.cs

using System;
using System.Numerics;

public static class CubicRootsFinder
{
    public const double Pi2 = 6.2831853071795864769252867665590057683943387987502d;
    // D = Q^3 + R^2 is the polynomial discriminant.
    // D > 0, 1 real root
    // D = 0, 3 real roots, at least two are equal
    // D < 0, 3 real and unequal roots

    /// <summary>
    /// Q and R are transformed variables.
    /// </summary>
    private static void QR(double a2, double a1, double a0, out double Q, out double R)
    {
        Q = (3 * a1 - a2 * a2) / 9.0;
        R = (9.0 * a2 * a1 - 27 * a0 - 2 * a2 * a2 * a2) / 54.0;
    }

    /// <summary>
    /// n^(1/3) - work around a negative double raised to (1/3)
    /// </summary>
    private static double PowThird(double n)
    {
        return Math.Pow(Math.Abs(n), 1d / 3d) * Math.Sign(n);
    }

    /// <summary>
    /// Find all real-valued roots of the cubic equation ax^3 + bx^2 + cx + b = 0.
    /// </summary>
    public static (double, double, double) RealRoots(double a, double b, double c, double d)
    {
        double a0 = d / a;
        double a1 = c / a;
        double a2 = b / a;

        QR(a2, a1, a0, out double Q, out double R);

        var Q3 = Q * Q * Q;
        var D = Q3 + R * R;
        var shift = -a2 / 3d;

        double x1;
        double x2 = double.NaN;
        double x3 = double.NaN;

        if (D >= 0)
        {
            // when D >= 0, use eqn (54)-(56) where S and T are real
            double sqrtD = Math.Pow(D, 0.5);
            double S = PowThird(R + sqrtD);
            double T = PowThird(R - sqrtD);
            x1 = shift + (S + T);
            if (D == 0)
            {
                x2 = shift - S;
            }
        }
        else
        {
            // 3 real roots, use eqn (70)-(73) to calculate the real roots
            double theta = Math.Acos(R / Math.Sqrt(-Q3));
            x1 = 2d * Math.Sqrt(-Q) * Math.Cos(theta / 3.0) + shift;
            x2 = 2d * Math.Sqrt(-Q) * Math.Cos((theta + Pi2) / 3d) + shift;
            x3 = 2d * Math.Sqrt(-Q) * Math.Cos((theta - Pi2) / 3d) + shift;
        }

        return (x1, x2, x3);
    }

    /// <summary>
    /// Find all three complex roots of the cubic equation d + c*x + b*x^2 + a*x^3 = 0.
    /// Note the special coefficient order ascending by exponent (consistent with polynomials).
    /// </summary>
    public static (Complex, Complex, Complex) Roots(double d, double c, double b, double a)
    {
        double A = b * b - 3 * a * c;
        double B = 2 * b * b * b - 9 * a * b * c + 27 * a * a * d;
        double s = -1 / (3 * a);

        double D = (B * B - 4 * A * A * A) / (-27 * a * a);
        if (D == 0d)
        {
            if (A == 0d)
            {
                var u = new Complex(s * b, 0d);
                return (u, u, u);
            }

            var v = new Complex((9 * a * d - b * c) / (2 * A), 0d);
            var w = new Complex((4 * a * b * c - 9 * a * a * d - b * b * b) / (a * A), 0d);
            return (v, v, w);
        }

        var C = (A == 0)
            ? new Complex(B, 0d).CubicRoots()
            : ((B + Complex.Sqrt(B * B - 4 * A * A * A)) / 2).CubicRoots();

        return (s * (b + C.Item1 + A / C.Item1), s * (b + C.Item2 + A / C.Item2), s * (b + C.Item3 + A / C.Item3));
    }

    /// <summary>
    /// Evaluate all cubic roots of this <c>Complex</c>.
    /// </summary>
    public static (Complex, Complex, Complex) CubicRoots(this Complex complex)
    {
        var r = Math.Pow(complex.Magnitude, 1d / 3d);
        var theta = complex.Phase / 3;
        const double shift = Pi2 / 3;
        return (Complex.FromPolarCoordinates(r, theta),
            Complex.FromPolarCoordinates(r, theta + shift),
            Complex.FromPolarCoordinates(r, theta - shift));
    }
}
