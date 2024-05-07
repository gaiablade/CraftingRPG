using System.Linq;
using CraftingRPG.Enums;
using CraftingRPG.GameStateManagement;
using CraftingRPG.GameStateManagement.GameStates;
using CraftingRPG.Global;
using CraftingRPG.InputManagement;

namespace CraftingRPG.MapObjects;

public class DemoSign : BaseMapObject
{
    private const string Message =
        "Check your quests with [[QUEST_MENU_KEY]]. After completing all quests, interact with this sign again " +
        "to end the game.";

    public override object OnInteract()
    {
        // Are the players quests done?
        var quests = Globals.PlayerInfo.QuestBook.GetActiveQuests();

        if (quests.All(x => x.IsComplete()))
        {
            GameStateManager.Instance.PushState(new DemoEndingGameState(), true);
        }
        else
        {
            var displayMessage = ReplaceParameters(Message);
            GameStateManager.Instance.PushState(new DialogueGameState(displayMessage), true);
            return null;
        }

        return null;
    }
    
    private string ReplaceParameters(string message)
    {
        var finishedMessage = message;
        finishedMessage = finishedMessage.Replace("[[QUEST_MENU_KEY]]",
            InputManager.Instance.GetKeyForAction(InputAction.OpenQuestsMenu).ToString());
        return finishedMessage;
    }
}