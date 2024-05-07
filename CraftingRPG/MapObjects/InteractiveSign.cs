using System.Collections.Generic;
using CraftingRPG.Enums;
using CraftingRPG.GameStateManagement;
using CraftingRPG.GameStateManagement.GameStates;
using CraftingRPG.InputManagement;

namespace CraftingRPG.MapObjects;

public class InteractiveSign : BaseMapObject
{
    private static readonly IDictionary<int, string> Messages = new Dictionary<int, string>
    {
        {
            1,
            "Look for hidden paths between the trees."
        }
    };

    private string Message;

    public InteractiveSign(int messageId)
    {
        SetMessage(messageId);
    }

    public void SetMessage(int messageId)
    {
        Message = Messages[messageId];
    }

    public override object OnInteract()
    {
        var displayMessage = ReplaceParameters(Message);
        GameStateManager.Instance.PushState(new DialogueGameState(displayMessage), true);
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