using CraftingRPG.MapManagement;
using Microsoft.Xna.Framework.Content;

namespace CraftingRPG.Interfaces;

public interface IMapLoader
{
    public GameMap LoadMapFromFile(string mapFile, ContentManager contentManager);
}