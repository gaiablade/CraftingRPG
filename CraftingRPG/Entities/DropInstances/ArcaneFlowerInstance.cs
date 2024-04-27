using CraftingRPG.AssetManagement;
using CraftingRPG.Interfaces;
using CraftingRPG.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace CraftingRPG.Entities.DropInstances;

public class ArcaneFlowerInstance : BaseItemInstance
{
    private IronBandItem Item;
    private Vector2 Position;
    private Vector2 Size = new(16, 16);

    public ArcaneFlowerInstance()
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

    public override Vector2 GetSize()
    {
        return Size;
    }

    public override RectangleF GetCollisionBox()
    {
        return new RectangleF(Position, Size);
    }

    public override Texture2D GetSpriteSheet()
    {
        return Assets.Instance.IconSpriteSheet;
    }

    public override Rectangle GetTextureRectangle()
    {
        return new(0, 256, 32, 32);
    }

    public override bool CanDrop()
    {
        return true;
    }

    public override IDroppable GetDroppable()
    {
        return new ItemDrop<ArcaneFlowerItem>();
    }

    public override void OnObtain()
    {
        AddItemToInventory<ArcaneFlowerItem>();
    }
}