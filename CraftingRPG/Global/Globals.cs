using CraftingRPG.Entities;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace CraftingRPG.Global;

public class Globals
{
    public static readonly Globals Instance = new();

    public PlayerInstance Player { get; set; }
    public bool ActionKeyPressed { get; set; } = false;
    public OrthographicCamera Camera { get; set; }

    // MonoGame
    public SpriteFont DefaultFont { get; set; }
    public SpriteFont Fnt10 { get; set; }
    public SpriteFont Fnt12 { get; set; }
    public SpriteFont Fnt15 { get; set; }
    public SpriteFont Fnt20 { get; set; }
    public Texture2D PlayerSpriteSheet { get; set; }
}