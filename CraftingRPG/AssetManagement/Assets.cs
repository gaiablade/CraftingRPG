using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CraftingRPG.AssetManagement;

public class Assets
{
    public static Assets Instance = new();

    // Textures //
    public Texture2D WoodUISpriteSheet { get; private set; }
    public Texture2D SlimeSpriteSheet { get; private set; }
    public Texture2D CraftingUi { get; private set; }
    public Texture2D InventoryUi { get; private set; }
    public Texture2D QuestUi { get; private set; }
    public Texture2D IconSpriteSheet { get; private set; }
    public Texture2D PaperNoteSpriteSheet { get; private set; }
    public Texture2D WoodCursorSpriteSheet { get; private set; }
    
    // Fonts //
    public SpriteFont Toriko10 { get; private set; }
    public SpriteFont Monogram12 { get; private set; }
    public SpriteFont Monogram18 { get; private set; }
    public SpriteFont Monogram24 { get; private set; }
    
    public void LoadAssets(ContentManager contentManager)
    {
        // Textures
        WoodUISpriteSheet = contentManager.Load<Texture2D>("textures/wood_ui");
        SlimeSpriteSheet = contentManager.Load<Texture2D>("textures/slime");
        CraftingUi = contentManager.Load<Texture2D>("textures/crafting_menu");
        InventoryUi = contentManager.Load<Texture2D>("textures/inventory_menu");
        QuestUi = contentManager.Load<Texture2D>("textures/quest_menu");
        IconSpriteSheet = contentManager.Load<Texture2D>("textures/icons_full_32");
        PaperNoteSpriteSheet = contentManager.Load<Texture2D>("textures/UI_Papernote_Spritesheet");
        WoodCursorSpriteSheet = contentManager.Load<Texture2D>("textures/Spritesheet_UI_Wood_Animation_Select_01");
        
        // Fonts
        Toriko10 = contentManager.Load<SpriteFont>("fonts/toriko-10");
        Monogram12 = contentManager.Load<SpriteFont>("fonts/monogram-12");
        Monogram18 = contentManager.Load<SpriteFont>("fonts/monogram-18");
        Monogram24 = contentManager.Load<SpriteFont>("fonts/monogram-24");
    }
}