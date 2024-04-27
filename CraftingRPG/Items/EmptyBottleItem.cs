using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using System.Collections.Generic;
using CraftingRPG.AssetManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CraftingRPG.Items
{
    public class EmptyBottleItem : IItem
    {
        public ItemId GetId() => ItemId.EmptyBottle;

        public ISet<ItemCategory> GetItemCategories() => new HashSet<ItemCategory>
        {
            { ItemCategory.Ingredient }
        };

        public Texture2D GetTileSet()
        {
            return Assets.Instance.IconSpriteSheet;
        }

        public Rectangle GetSourceRectangle()
        {
            return new Rectangle(224, 544, 32, 32);
        }

        public string GetName() => "Empty Bottle";
    }
}
