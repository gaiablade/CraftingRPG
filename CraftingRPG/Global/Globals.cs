using CraftingRPG.Entities;
using MonoGame.Extended;

namespace CraftingRPG.Global;

public static class Globals
{
    public static PlayerInfo PlayerInfo { get; set; }
    public static PlayerInstance Player { get; set; }
    public static  bool ActionKeyPressed { get; set; } = false;
    public static  OrthographicCamera Camera { get; set; }
}