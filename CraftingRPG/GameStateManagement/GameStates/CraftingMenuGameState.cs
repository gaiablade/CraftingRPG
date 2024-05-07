using System.Collections.Generic;
using System.Linq;
using CraftingRPG.AssetManagement;
using CraftingRPG.Enums;
using CraftingRPG.Extensions;
using CraftingRPG.Global;
using CraftingRPG.InputManagement;
using CraftingRPG.Interfaces;
using CraftingRPG.QuestManagement;
using CraftingRPG.Timers;
using CraftingRPG.Utility;
using Microsoft.Xna.Framework;

namespace CraftingRPG.GameStateManagement.GameStates;

public class CraftingMenuGameState : BaseGameState
{
    private int Cursor;
    private bool MenuClosed;
    private readonly IDictionary<RecipeId, IRecipe> Recipes;

    private readonly ITimer TransitionTimer;

    public CraftingMenuGameState()
    {
        Recipes = Globals.PlayerInfo.RecipeBook
            .Recipes
            .OrderBy(x => x.Value.GetId())
            .ToDictionary(x => x.Key, x => x.Value);
        TransitionTimer = new EaseOutTimer(0.5);
    }

    public override void DrawWorld()
    {
    }

    public override void DrawUi()
    {
        var percent = (float)TransitionTimer.GetPercent();

        var displacement = GameManager.Resolution.Y * percent;
        var backgroundColor = 0.75F * percent;

        GameManager.SpriteBatch.Draw(GameManager.Pixel,
            new Rectangle(Point.Zero, GameManager.Resolution),
            Color.Black * backgroundColor);
        GameManager.SpriteBatch.Draw(Assets.Instance.CraftingUi,
            new Rectangle(new Point(-16, (int)(-GameManager.Resolution.Y + displacement - 16)),
                GameManager.Resolution),
            Assets.Instance.CraftingUi.Bounds,
            Color.White);

        DrawRecipeList(displacement);

        if (Recipes.Count > 0)
        {
            DrawSelectedRecipe(displacement);
        }
    }

    private void DrawRecipeList(float displacement)
    {
        const int listX = 208;
        const int listY = 100;

        foreach (var ((_, recipe), i) in Recipes.WithIndex())
        {
            var canBeCrafted = CanRecipeBeCrafted(recipe);
            var color = Cursor == i ? Color.DarkRed : canBeCrafted ? Color.White : Color.DarkGray;
            var itemInfo = recipe.GetCraftedItem();
            var itemName = $"{itemInfo.GetName()}";
            var nameSize = Assets.Instance.Monogram24.MeasureString(itemName);
            GameManager.SpriteBatch.DrawString(Assets.Instance.Monogram24,
                itemName,
                new Vector2(listX - nameSize.X / 2,
                    (int)(-GameManager.Resolution.Y + displacement + listY + (nameSize.Y + 5) * i)),
                color);
        }
    }

    private void DrawSelectedRecipe(float displacement)
    {
        const int labelX = 592;
        const int labelY = 68;
        
        var selection = Recipes.ElementAt(Cursor);
        var recipe = selection.Value;

        var craftedItem = recipe.GetCraftedItem();
        var name = craftedItem.GetName();

        var nameSize = Assets.Instance.Monogram12.MeasureString(name);
        GameManager.SpriteBatch.DrawString(Assets.Instance.Monogram12,
            name,
            new Vector2(labelX - nameSize.X / 2, -GameManager.Resolution.Y + displacement + labelY),
            Color.Black);

        const int ingredientsLabelX = 592;
        const int ingredientsLabelY = 192;
        
        var ingredientsLabelSize = Assets.Instance.Monogram24.MeasureString("Ingredients");
        GameManager.SpriteBatch.DrawString(Assets.Instance.Monogram24,
            "Ingredients",
            new Vector2(ingredientsLabelX - ingredientsLabelSize.X / 2, 
                -GameManager.Resolution.Y + displacement + ingredientsLabelY),
            Color.Black);

        const int ingredientNameX = 463;
        var ingredientNameY = ingredientsLabelY + ingredientsLabelSize.Y + 15;
        
        var ingredients = recipe.GetIngredients();
        
        foreach (var ((itemInfo, requiredQty), i) in ingredients.WithIndex())
        {
            var playersItemCount = Globals.PlayerInfo.Inventory.GetItemCount(itemInfo);
            var color = playersItemCount >= requiredQty ? Color.Black : Color.Red * 0.5F;
            var itemsReq = $"{itemInfo.GetName()} x{requiredQty}";
            var itemsReqSize = Assets.Instance.Monogram18.MeasureString(itemsReq);

            GameManager.SpriteBatch.DrawString(Assets.Instance.Monogram18,
                itemsReq,
                new Vector2(ingredientNameX,
                    -GameManager.Resolution.Y + displacement + ingredientNameY + itemsReqSize.Y * i),
                color);

            if (playersItemCount > 0)
            {
                var playersItemCountDisplay = $"({playersItemCount})";
                GameManager.SpriteBatch.DrawString(Assets.Instance.Monogram18,
                    playersItemCountDisplay,
                    new Vector2(ingredientNameX + itemsReqSize.X + 10, 
                        -GameManager.Resolution.Y + displacement + ingredientNameY + itemsReqSize.Y * i),
                    Color.DarkGreen);
            }
        }
    }

    private bool CanRecipeBeCrafted(IRecipe recipe)
    {
        var inventory = Globals.PlayerInfo.Inventory;
        var ingredientList = recipe.GetIngredients();

        foreach (var (itemInfo, requiredQty) in ingredientList)
        {
            var ownedQty = inventory.GetItemCount(itemInfo);
            if (ownedQty < requiredQty)
                return false;
        }

        return true;
    }

    public override void Update(GameTime gameTime)
    {
        TransitionTimer.Update(gameTime);

        if (!MenuClosed)
        {
            if (InputManager.Instance.GetKeyPressState(InputAction.MoveSouth) == KeyPressState.Pressed)
            {
                Assets.Instance.MenuHoverSfx01.Play(0.3F, 0F, 0F);
                Cursor = CustomMath.WrapAround(Cursor + 1, 0, Recipes.Count - 1);
            }
            else if (InputManager.Instance.GetKeyPressState(InputAction.MoveNorth) == KeyPressState.Pressed)
            {
                Assets.Instance.MenuHoverSfx01.Play(0.3F, 0F, 0F);
                Cursor = CustomMath.WrapAround(Cursor - 1, 0, Recipes.Count - 1);
            }

            if (InputManager.Instance.GetKeyPressState(InputAction.MenuSelect) == KeyPressState.Pressed)
            {
                var recipe = Recipes.ElementAt(Cursor);
                if (CanRecipeBeCrafted(recipe.Value))
                {
                    foreach (var (ingredientInfo, qty) in recipe.Value.GetIngredients())
                    {
                        Globals.PlayerInfo.Inventory.AddQuantity(ingredientInfo, -qty);
                    }

                    var itemInfo = recipe.Value.GetCraftedItem();
                    Globals.Player.GetInfo().Inventory.AddQuantity(itemInfo, 1);
                    Globals.Player.GetInfo().RecipeBook.NumberCrafted[recipe.Key]++;
                    
                    // Check for crafting quests
                    foreach (var questInstance in Globals.Player.GetInfo().QuestBook.GetActiveQuests())
                    {
                        if (questInstance is CraftQuestInstance craftQuestInstance)
                        {
                            craftQuestInstance.ItemCrafted(itemInfo);
                        }
                    }
                    
                    Assets.Instance.MenuConfirmSfx01.Play(0.3F, 0F, 0F);
                }
            }

            if (InputManager.Instance.GetKeyPressState(InputAction.ExitMenu) == KeyPressState.Pressed)
            {
                MenuClosed = true;
                TransitionTimer.SetReverse();
            }
        }
        else
        {
            if (TransitionTimer.IsDone())
            {
                GameStateManager.Instance.PopState();
                Flags.IsPaused = false;
            }
        }
    }
}