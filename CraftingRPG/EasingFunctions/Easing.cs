using System;

namespace CraftingRPG.EasingFunctions;

public static class Easing
{
    public static float EaseOutSine(float x)
    {
        return (float)Math.Sin((x * Math.PI) / 2);
    }
    
    public static double EaseOutSine(double x)
    {
        return Math.Sin((x * Math.PI) / 2.0);
    }
}