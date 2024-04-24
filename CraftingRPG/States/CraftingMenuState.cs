using CraftingRPG.Entities;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using CraftingRPG.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using CraftingRPG.Global;

namespace CraftingRPG.States;

public class CraftingMenuState : IState
{
    private IDictionary<RecipeId, IRecipe> Recipes;
    private int Cursor = 0;

    public CraftingMenuState()
    {
        Recipes = GameManager.PlayerInfo.RecipeBook
            .Recipes
            .OrderBy(x => x.Value.GetId())
            .ToDictionary(x => x.Key, x => x.Value);
        GameManager.AddKeyIfNotExists(Keys.LeftControl);
    }

    public void Render()
    {
        DrawRecipeList();

        if (Recipes.Count > 0)
        {

            var selection = Recipes.ElementAt(Cursor);
            var recipe = selection.Value;

            var craftedItem = GameManager.ItemInfo[recipe.GetCraftedItem()];
            var name = craftedItem.GetName();

            var sprite = craftedItem.GetSpriteSheetIndex();
            GameManager.SpriteBatch.Draw(GameManager.SpriteSheet,
                new Rectangle(GameManager.Resolution.X / 2 + 20, 10, 32, 32),
                new Rectangle(0, sprite * 32, 32, 32),
                Color.White);

            var nameSize = Globals.Instance.DefaultFont.MeasureString(name);
            GameManager.SpriteBatch.DrawString(Globals.Instance.DefaultFont,
                name,
                new Vector2(GameManager.Resolution.X / 2 + 20 + 32, 10 + 16 - nameSize.Y / 2),
                Color.White);

            GameManager.SpriteBatch.DrawString(Globals.Instance.DefaultFont,
                "Ingredients:",
                new Vector2(GameManager.Resolution.X / 2 + 20, 10 + nameSize.Y + 30),
                Color.White);

            var ingredients = recipe.GetIngredients();
            var i = 0;
            foreach (var (itemId, requiredQty) in ingredients)
            {
                var itemInfo = GameManager.ItemInfo[itemId];
                var ingredientSprite = itemInfo.GetSpriteSheetIndex();
                var playersItemCount = GameManager.PlayerInfo.Inventory[itemId];
                var color = playersItemCount >= requiredQty ? Color.White : Color.DarkGray;
                var itemsReq = $"{itemInfo.GetName()} x{requiredQty}";
                var itemsReqSize = Globals.Instance.Fnt12.MeasureString(itemsReq);
                GameManager.SpriteBatch.DrawString(Globals.Instance.Fnt12,
                    itemsReq,
                    new Vector2(GameManager.Resolution.X / 2 + 20, 10 + nameSize.Y + 30 + nameSize.Y + 20 + (10 + nameSize.Y) * i + 16 - nameSize.Y / 2),
                    color);
                GameManager.SpriteBatch.Draw(GameManager.SpriteSheet,
                    new Rectangle((int)(GameManager.Resolution.X / 2 + 20 + itemsReqSize.X + 10), (int)(10 + nameSize.Y + 30 + nameSize.Y + 20 + (10 + nameSize.Y) * i), 32, 32),
                    new Rectangle(0, 32 * ingredientSprite, 32, 32),
                    color);

                if (playersItemCount > 0)
                {
                    var playersItemCountDisplay = $"({playersItemCount})";
                    var itemCountDisplaySize = Globals.Instance.Fnt12.MeasureString(playersItemCountDisplay);
                    GameManager.SpriteBatch.DrawString(Globals.Instance.Fnt12,
                        playersItemCountDisplay,
                        new Vector2((int)(GameManager.Resolution.X / 2 + 20 + itemsReqSize.X + 10) + 40, 10 + nameSize.Y + 30 + nameSize.Y + 20 + (10 + nameSize.Y) * i + 16 - nameSize.Y / 2),
                        Color.White);
                }

                i++;
            }

            // Craft button
            var craftText = "Craft (Z)";
            var craftTextSize = Globals.Instance.Fnt15.MeasureString(craftText);
            var ctColor = CanRecipeBeCrafted(Recipes.ElementAt(Cursor).Value) ? Color.Green : Color.Gray;
            GameManager.SpriteBatch.Draw(GameManager.Pixel,
                new Rectangle(GameManager.Resolution.X / 2 + GameManager.Resolution.X / 4 - (int)(craftTextSize.X + 50) / 2,
                    GameManager.Resolution.Y - 50 - 50,
                    (int)craftTextSize.X + 50,
                    50),
                ctColor);
            GameManager.SpriteBatch.DrawString(Globals.Instance.Fnt15,
                craftText,
                new Vector2((int)(GameManager.Resolution.X / 2 + GameManager.Resolution.X / 4 - craftTextSize.X / 2),
                    (int)(GameManager.Resolution.Y - 50 - 25 - craftTextSize.Y / 2)),
                Color.White);
        }
    }

    private void DrawRecipeList()
    {
        var i = 0;
        foreach (var (id, recipe) in Recipes)
        {
            var canBeCrafted = CanRecipeBeCrafted(recipe);
            var color = Cursor == i ? Color.Orange : canBeCrafted ? Color.White : Color.DarkGray;
            var itemId = recipe.GetCraftedItem();
            var itemInfo = GameManager.ItemInfo[itemId];
            var itemName = $"{i + 1}. {itemInfo.GetName()}";
            var nameSize = Globals.Instance.Fnt15.MeasureString(itemName);
            GameManager.SpriteBatch.DrawString(Globals.Instance.Fnt15,
                itemName,
                new Vector2(5, 15 + (nameSize.Y + 5) * i),
                color);
            i++;
        }
    }

    private bool CanRecipeBeCrafted(IRecipe recipe)
    {
        var inventory = GameManager.PlayerInfo.Inventory;
        var ingredientList = recipe.GetIngredients();

        foreach (var (itemId, requiredQty) in ingredientList)
        {
            var ownedQty = inventory[itemId];
            if (ownedQty < requiredQty)
                return false;
        }
        return true;
    }

    public void Update(GameTime gameTime)
    {
        if (GameManager.FramesKeysHeld[Keys.Down] == 1)
        {
            GameManager.MenuHoverSfx01.Play(0.3F, 0F, 0F);
            Cursor = CustomMath.WrapAround(Cursor + 1, 0, Recipes.Count - 1);
        }
        else if (GameManager.FramesKeysHeld[Keys.Up] == 1)
        {
            GameManager.MenuHoverSfx01.Play(0.3F, 0F, 0F);
            Cursor = CustomMath.WrapAround(Cursor - 1, 0, Recipes.Count - 1);
        }

        if (GameManager.FramesKeysHeld[Keys.Z] == 1)
        {
            var recipe = Recipes.ElementAt(Cursor);
            if (CanRecipeBeCrafted(recipe.Value))
            {
                foreach (var (ingredientId, qty) in recipe.Value.GetIngredients())
                {
                    GameManager.PlayerInfo.Inventory[ingredientId] -= qty;
                }
                var itemId = recipe.Value.GetCraftedItem();
                GameManager.PlayerInfo.Inventory[itemId]++;
                GameManager.MenuConfirmSfx01.Play(0.3F, 0F, 0F);
            }
        }

        if (GameManager.FramesKeysHeld[Keys.LeftControl] == 1)
        {
            StateManager.Instance.PopState();
        }
    }
}
