using CraftingRPG.Enums;
using Microsoft.Xna.Framework;

namespace CraftingRPG.MapManagement;

public class MapObjectAttributes
{
    public bool IsSolid;
    public bool IsInteractive;
    public Rectangle CollisionRectangle;

    public static MapObjectAttributes GetObjectAttributes(MapObjectId objectId)
    {
        var attr = new MapObjectAttributes()
        {
            IsSolid = true,
            IsInteractive = false,
            CollisionRectangle = new Rectangle(Point.Zero, new Point(16, 16))
        };

        switch (objectId)
        {
            case MapObjectId.None:
                attr.CollisionRectangle.Size = Point.Zero;
                break;
            case MapObjectId.Bench01:
            case MapObjectId.Log01:
            case MapObjectId.Log02:
                attr.CollisionRectangle.Size = new Point(32, 16);
                break;
            case MapObjectId.Tree01:
            case MapObjectId.Tree02:
                attr.CollisionRectangle = new Rectangle(new Point(12, 43), new Point(24, 15));
                break;
            case MapObjectId.Sign01:
            case MapObjectId.Sign02:
            case MapObjectId.Chest01:
                attr.IsInteractive = true;
                break;
            case MapObjectId.Spawn:
                attr.IsSolid = false;
                attr.CollisionRectangle = Rectangle.Empty;
                break;
        }

        return attr;
    }
}