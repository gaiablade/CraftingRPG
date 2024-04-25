using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace CraftingRPG.Extensions;

public static class RectangleExtensions
{
    public static Vector2 GetIntersectionDepth(this Rectangle a, Rectangle b)
    {
        var hwa = a.Width / 2F;
        var hha = a.Height / 2F;
        var hwb = b.Width / 2F;
        var hhb = b.Height / 2F;

        var ca = new Vector2(a.Center.X, a.Center.Y);
        var cb = new Vector2(b.Center.X, b.Center.Y);

        var distX = ca.X - cb.X;
        var distY = ca.Y - cb.Y;
        var minDistX = hwa + hwb;
        var minDistY = hha + hhb;

        if (Math.Abs(distX) >= minDistX || Math.Abs(distY) >= minDistY)
            return Vector2.Zero;

        var depthX = distX > 0 ? minDistX - distX : -minDistX - distX;
        var depthY = distY > 0 ? minDistY - distY : -minDistY - distY;

        return new Vector2(depthX, depthY);
    }
    
    public static Vector2 GetIntersectionDepth(this RectangleF a, RectangleF b)
    {
        var hwa = a.Width / 2F;
        var hha = a.Height / 2F;
        var hwb = b.Width / 2F;
        var hhb = b.Height / 2F;

        var ca = new Vector2(a.Center.X, a.Center.Y);
        var cb = new Vector2(b.Center.X, b.Center.Y);

        var distX = ca.X - cb.X;
        var distY = ca.Y - cb.Y;
        var minDistX = hwa + hwb;
        var minDistY = hha + hhb;

        if (Math.Abs(distX) >= minDistX || Math.Abs(distY) >= minDistY)
            return Vector2.Zero;

        var depthX = distX > 0 ? minDistX - distX : -minDistX - distX;
        var depthY = distY > 0 ? minDistY - distY : -minDistY - distY;

        return new Vector2(depthX, depthY);
    }
}