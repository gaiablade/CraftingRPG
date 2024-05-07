using CraftingRPG.Enums;
using CraftingRPG.MapManagement;
using Microsoft.Xna.Framework;

namespace CraftingRPG.Interfaces;

public interface IMapObject : IInstance
{
    public MapObjectId GetId();
    public MapTileSet GetTileSet();
    public void SetId(MapObjectId id);
    public MapObjectAttributes GetAttributes();
    public void SetAttributes(MapObjectAttributes attributes);
    public object OnInteract();
    public void SetSize(Point size);
    public void SetTileSet(MapTileSet tileSet);
    public void SetTextureRectangle(Rectangle textureRectangle);
}