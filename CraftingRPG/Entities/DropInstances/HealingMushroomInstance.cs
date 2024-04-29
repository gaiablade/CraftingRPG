using CraftingRPG.Interfaces;
using CraftingRPG.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace CraftingRPG.Entities.DropInstances;

public class HealingMushroomInstance : BaseItemInstance
{
    private HealingMushroomItem Item;

    public HealingMushroomInstance()
    {
        Item = new();
    }
    
    public override Vector2 GetPosition()
    {
        return Position;
    }

    public override Vector2 SetPosition(Vector2 position)
    {
        Position = position;
        return position;
    }

    public override double GetDepth()
    {
        return -1;
    }

    public override Point GetSize()
    {
        return Size;
    }

    public override RectangleF GetCollisionBox()
    {
        return new RectangleF(Position, Size);
    }

    public override Texture2D GetSpriteSheet()
    {
        return Item.GetTileSet();
    }

    public override Rectangle GetTextureRectangle()
    {
        return Item.GetSourceRectangle();
    }

    public override bool CanDrop()
    {
        return true;
    }

    public override IDroppable GetDroppable()
    {
        return new ItemDrop<HealingMushroomItem>();
    }

    public override void OnObtain()
    {
        AddItemToInventory<HealingMushroomItem>();
    }
}