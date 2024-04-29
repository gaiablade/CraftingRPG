using CraftingRPG.Entities;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace CraftingRPG.Global;

public static class Globals
{
    public static PlayerInstance Player { get; set; }
    public static  bool ActionKeyPressed { get; set; } = false;
    public static  OrthographicCamera Camera { get; set; }
}