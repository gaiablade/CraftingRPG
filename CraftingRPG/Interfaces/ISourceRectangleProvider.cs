using Microsoft.Xna.Framework;

namespace CraftingRPG.Interfaces;

public interface ISourceRectangleProvider<T>
{
    public Rectangle GetSourceRectangle(T @object);
}