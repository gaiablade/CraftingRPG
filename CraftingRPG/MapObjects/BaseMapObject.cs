using CraftingRPG.Enums;
using CraftingRPG.Graphics;
using CraftingRPG.Interfaces;
using CraftingRPG.MapManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace CraftingRPG.MapObjects;

public abstract class BaseMapObject : IMapObject
{
    protected MapObjectId Id;
    protected Vector2 Position;
    protected MapObjectAttributes Attributes;
    protected Point Size;

    protected MapTileSet TileSet;
    protected Rectangle SourceRectangle;

    public virtual Vector2 GetPosition() => Position;
    public virtual Vector2 SetPosition(Vector2 position) => Position = position;
    public virtual Vector2 Move(Vector2 movementVector) => Position += movementVector;
    public virtual double GetDepth() => GetCollisionBox().Y + GetCollisionBox().Height;
    public virtual Point GetSize() => Size;

    public virtual RectangleF GetCollisionBox()
    {
        var collisionBox = GetAttributes().CollisionRectangle;
        return new RectangleF(collisionBox.X + GetPosition().X, collisionBox.Y + GetPosition().Y, collisionBox.Width,
            collisionBox.Height);
    }

    public virtual SpriteDrawingData GetDrawingData() => new SpriteDrawingData
    {
        Texture = GetSpriteSheet(),
        SourceRectangle = GetTextureRectangle()
    };

    public virtual Texture2D GetSpriteSheet() => TileSet.SpriteSheetTexture;
    public virtual Rectangle GetTextureRectangle() => SourceRectangle;
    public virtual Vector2 GetMovementVector() => Vector2.Zero;
    public MapObjectId GetId() => Id;
    public MapTileSet GetTileSet() => TileSet;

    public void SetId(MapObjectId id) => Id = id;
    public virtual MapObjectAttributes GetAttributes() => Attributes;
    public void SetAttributes(MapObjectAttributes attributes) => Attributes = attributes;

    public virtual object OnInteract()
    {
        return null;
    }

    public void SetSize(Point size) => Size = size;
    public void SetTileSet(MapTileSet tileSet) => TileSet = tileSet;
    public void SetTextureRectangle(Rectangle textureRectangle) => SourceRectangle = textureRectangle;
}