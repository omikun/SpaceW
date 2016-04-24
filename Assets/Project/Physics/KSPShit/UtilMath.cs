﻿using System;

namespace Experimental
{
    public class UtilMath
    {
        public static double TwoPI;

        public static float TwoPIf;

        public static double HalfPI;

        public static float HalfPIf;

        static UtilMath()
        {
            TwoPI = 6.28318530717959;
            TwoPIf = 6.28318548f;
            HalfPI = 1.5707963267949;
            HalfPIf = 1.57079637f;
        }

        public static double ACosh(double x)
        {
            return Math.Log(x + Math.Sqrt(x * x - 1));
        }

        public static double ACoth(double x)
        {
            return ATanh(1 / x);
        }

        public static double ACsch(double x)
        {
            return ASinh(1 / x);
        }

        public static double ASech(double x)
        {
            return ACosh(1 / x);
        }

        public static double ASinh(double x)
        {
            return Math.Log(x + Math.Sqrt(x * x + 1));
        }

        public static double ATanh(double x)
        {
            return Math.Log((1 + x) / (1 - x)) / 2;
        }

        public static int BSPSolver(ref double v0, double dv, Func<double, double> solveFor, double vMin, double vMax, double epsilon, int maxIterations)
        {
            int num;

            if (v0 < vMin) num = 0;
            else if (v0 <= vMax)
            {
                int num1 = 0;

                double num2 = solveFor(v0);

                while (true)
                {
                    if ((dv <= epsilon ? true : num1 >= maxIterations)) break;

                    double num3 = solveFor(v0 + dv);
                    double num4 = solveFor(v0 - dv);

                    if (v0 - dv < vMin) num4 = double.MaxValue;
                    if (v0 + dv > vMax) num3 = double.MaxValue;

                    num3 = Math.Abs(num3);
                    num4 = Math.Abs(num4);
                    num2 = Math.Min(num2, Math.Min(num3, num4));

                    if (num2 == num4) v0 = v0 - dv;
                    else if (num2 == num3) v0 = v0 + dv;

                    dv = dv / 2;
                    num1++;
                }
                num = num1;
            }
            else
            {
                num = 0;
            }
            return num;
        }

        public static int BSPSolver(ref float v0, float dv, Func<float, float> solveFor, float vMin, float vMax, float epsilon, int maxIterations)
        {
            int num;

            if (v0 < vMin) num = 0;
            else if (v0 <= vMax)
            {
                int num1 = 0;

                float single = solveFor(v0);

                while (true)
                {
                    if ((dv <= epsilon ? true : num1 >= maxIterations)) break;

                    float single1 = solveFor(v0 + dv);
                    float single2 = solveFor(v0 - dv);

                    if (v0 - dv < vMin) single2 = float.MaxValue;
                    if (v0 + dv > vMax) single1 = float.MaxValue;

                    single1 = Math.Abs(single1);
                    single2 = Math.Abs(single2);
                    single = Math.Min(single, Math.Min(single1, single2));

                    if (single == single2) v0 = v0 - dv;
                    else if (single == single1) v0 = v0 + dv;

                    dv = dv / 2f;
                    num1++;
                }
                num = num1;
            }
            else
            {
                num = 0;
            }
            return num;
        }

        public static double Coth(double x)
        {
            return Math.Cosh(x) / Math.Sinh(x);
        }

        public static double Csch(double x)
        {
            return 1 / Math.Sinh(x);
        }

        public static double Flatten(double z, double midPoint, double easing)
        {
            return 1 - 1 / (Math.Pow(1 / midPoint * Math.Abs(z), easing) + 1) * (double)Math.Sign(z);
        }

        public static bool IsDivisible(int n, int byN)
        {
            while (byN % 2 == 0)
            {
                byN = byN / 2;
            }

            while (byN % 5 == 0)
            {
                byN = byN / 5;
            }

            return n % byN == 0;
        }

        public static bool IsPowerOfTwo(int x)
        {
            return (x & checked(x - 1)) == 0;
        }

        public static double Lerp(double a, double b, double t)
        {
            t = Math.Min(1, Math.Max(0, t));

            return (1 - t) * a + t * b;
        }

        public static double LerpUnclamped(double a, double b, double t)
        {
            return (1 - t) * a + t * b;
        }

        public static float RoundToPlaces(float value, int decimalPlaces)
        {
            float single = (float)Math.Pow(10, (double)decimalPlaces);

            return (float)(Math.Round((double)(value * single)) / (double)single);
        }

        public static double RoundToPlaces(double value, int decimalPlaces)
        {
            double place = Math.Pow(10, decimalPlaces);

            return Math.Round(value * place) / place;
        }

        public static double Sech(double x)
        {
            return 1 / Math.Cosh(x);
        }

        public static float WrapAround(float value, float min, float max)
        {
            if (value < min) value = value + max;

            return value % max;
        }

        public static double WrapAround(double value, double min, double max)
        {
            if (value < min) value = value + max;

            return value % max;
        }

        public static int WrapAround(int value, int min, int max)
        {
            if (value < min) value = checked(value + max);

            return value % max;
        }
    }
}