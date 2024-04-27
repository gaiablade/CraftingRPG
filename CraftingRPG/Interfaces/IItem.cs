using CraftingRPG.Enums;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CraftingRPG.Interfaces;

public interface IItem
{
    public string GetName();
    public ItemId GetId();
    public ISet<ItemCategory> GetItemCategories();
    public Texture2D GetTileSet();
    public Rectangle GetSourceRectangle();
}
