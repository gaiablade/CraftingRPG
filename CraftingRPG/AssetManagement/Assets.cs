using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace CraftingRPG.AssetManagement;

public class Assets
{
    public static Assets Instance = new();

    // Textures //
    public Texture2D WoodUiSpriteSheet { get; private set; }
    public Texture2D SlimeSpriteSheet { get; private set; }
    public Texture2D CraftingUi { get; private set; }
    public Texture2D InventoryUi { get; private set; }
    public Texture2D QuestUi { get; private set; }
    public Texture2D QuestDetailsUi { get; private set; }
    public Texture2D IconSpriteSheet { get; private set; }
    public Texture2D PaperNoteSpriteSheet { get; private set; }
    public Texture2D WoodCursorSpriteSheet { get; private set; }
    public Texture2D PlayerSpriteSheet { get; set; }
    public Texture2D TitleUi { get; set; }
    public Texture2D KeyIconSpriteSheet { get; set; }
    public Texture2D InstructionsUi { get; set; }
    public Texture2D DialogueUi { get; set; }
    public Texture2D ThanksForPlaying { get; set; }
    public Texture2D HealthBarUi { get; set; }
    
    // Fonts //
    public SpriteFont Toriko10 { get; private set; }
    public SpriteFont Monogram6 { get; private set; }
    public SpriteFont Monogram12 { get; private set; }
    public SpriteFont Monogram18 { get; private set; }
    public SpriteFont Monogram24 { get; private set; }
    
    // Sfx //
    public SoundEffect SwingSfx01 { get; private set; }
    public SoundEffect HitSfx01 { get; private set; }
    public SoundEffect MaterialGrabSfx01 { get; private set; }
    public SoundEffect RecipeGrabSfx01 { get; private set; }
    public SoundEffect MenuHoverSfx01 { get; private set; }
    public SoundEffect MenuConfirmSfx01 { get; private set; }
    public SoundEffect Swoosh02 { get; private set; }
    public SoundEffect Impact01 { get; private set; }
    public SoundEffect EnemyDeath02 { get; private set; }
    public SoundEffect BagOpen { get; private set; }
    
    // Music
    public Song Field02 { get; private set; }
    public Song GameOver02 { get; private set; }
    
    public void LoadAssets(ContentManager contentManager)
    {
        // Textures
        WoodUiSpriteSheet = contentManager.Load<Texture2D>("textures/wood_ui");
        SlimeSpriteSheet = contentManager.Load<Texture2D>("textures/slime");
        CraftingUi = contentManager.Load<Texture2D>("textures/crafting_menu");
        InventoryUi = contentManager.Load<Texture2D>("textures/inventory_menu");
        QuestUi = contentManager.Load<Texture2D>("textures/quest_menu");
        QuestDetailsUi = contentManager.Load<Texture2D>("textures/quest_details_menu");
        IconSpriteSheet = contentManager.Load<Texture2D>("textures/icons_full_32");
        PaperNoteSpriteSheet = contentManager.Load<Texture2D>("textures/UI_Papernote_Spritesheet");
        WoodCursorSpriteSheet = contentManager.Load<Texture2D>("textures/Spritesheet_UI_Wood_Animation_Select_01");
        PlayerSpriteSheet = contentManager.Load<Texture2D>("textures/player");
        TitleUi = contentManager.Load<Texture2D>("textures/title_background");
        KeyIconSpriteSheet = contentManager.Load<Texture2D>("textures/MV Icons Keyboard - ALL");
        InstructionsUi = contentManager.Load<Texture2D>("textures/instructions_ui");
        DialogueUi = contentManager.Load<Texture2D>("textures/dialogue_ui");
        ThanksForPlaying = contentManager.Load<Texture2D>("textures/thanks_for_playing");
        HealthBarUi = contentManager.Load<Texture2D>("textures/health_bar");
        
        // Fonts
        Toriko10 = contentManager.Load<SpriteFont>("fonts/toriko-10");
        Monogram6 = contentManager.Load<SpriteFont>("fonts/monogram-6");
        Monogram12 = contentManager.Load<SpriteFont>("fonts/monogram-12");
        Monogram18 = contentManager.Load<SpriteFont>("fonts/monogram-18");
        Monogram24 = contentManager.Load<SpriteFont>("fonts/monogram-24");
        
        // Sfx
        SwingSfx01 = contentManager.Load<SoundEffect>("sfx/swoosh_01");
        HitSfx01 = contentManager.Load<SoundEffect>("sfx/Pierce_01");
        MaterialGrabSfx01 = contentManager.Load<SoundEffect>("sfx/Leather");
        RecipeGrabSfx01 = contentManager.Load<SoundEffect>("sfx/Scroll");
        MenuHoverSfx01 = contentManager.Load<SoundEffect>("sfx/Hover_04");
        MenuConfirmSfx01 = contentManager.Load<SoundEffect>("sfx/Confirm_05");
        Swoosh02 = contentManager.Load<SoundEffect>("sfx/30_swoosh_03");
        Impact01 = contentManager.Load<SoundEffect>("sfx/09_Impact_01");
        EnemyDeath02 = contentManager.Load<SoundEffect>("sfx/70_Enemy_death_02");
        BagOpen = contentManager.Load<SoundEffect>("sfx/Bag_Open");
        
        // Music
        Field02 = contentManager.Load<Song>("music/field_theme_2");
        GameOver02 = contentManager.Load<Song>("music/Game_Over_2");
    }
}