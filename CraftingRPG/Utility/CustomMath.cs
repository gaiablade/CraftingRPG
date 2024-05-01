using Microsoft.Xna.Framework;
using System;

namespace CraftingRPG.Utility;

public static class CustomMath
{
    public static int WrapAround(int x, int floor, int ceil)
    {
        return floor + (x - floor + ceil - floor + 1) % (ceil - floor + 1);
    }

    public static Vector2 UnitVector(Vector2 v)
    {
        var magnitude = (float)Math.Sqrt(Math.Pow(v.X, 2) + Math.Pow(v.Y, 2));
        return new Vector2(v.X / magnitude, v.Y / magnitude);
    }

    public static double Lerp(double x, double y, double t)
    {
        return x + (y - x) * Math.Clamp(t, 0, 1);
    }
}
