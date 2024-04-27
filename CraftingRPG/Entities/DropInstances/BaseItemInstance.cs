using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace CraftingRPG.Entities.DropInstances;

public abstract class BaseItemInstance : IDropInstance
{
    public virtual Vector2 GetPosition()
    {
        throw new System.NotImplementedException();
    }

    public virtual Vector2 SetPosition(Vector2 position)
    {
        throw new System.NotImplementedException();
    }

    public virtual double GetDepth()
    {
        throw new System.NotImplementedException();
    }

    public virtual Vector2 GetSize()
    {
        throw new System.NotImplementedException();
    }

    public virtual RectangleF GetCollisionBox()
    {
        throw new System.NotImplementedException();
    }

    public virtual Texture2D GetSpriteSheet()
    {
        throw new System.NotImplementedException();
    }

    public virtual Rectangle GetTextureRectangle()
    {
        throw new System.NotImplementedException();
    }

    public virtual bool CanDrop()
    {
        throw new System.NotImplementedException();
    }

    public virtual IDroppable GetDroppable()
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnObtain()
    {
    }

    protected void AddItemToInventory<T>() where T : IItem, new()
    {
        var item = new T();
        var player = GameManager.PlayerInfo;

        player.Inventory[item.GetId()]++;
        GameManager.MaterialGrabSfx01.Play(0.3F, 0F, 0F);

        foreach (var questInstance in player.Quests)
        {
            var fetchQuestInstance = questInstance as FetchQuestInstance;
            fetchQuestInstance.AddCollectedItem(item.GetId(), 1);
        }
    }
}