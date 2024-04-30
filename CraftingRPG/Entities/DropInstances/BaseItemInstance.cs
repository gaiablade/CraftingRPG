using CraftingRPG.AssetManagement;
using CraftingRPG.Interfaces;
using CraftingRPG.QuestManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace CraftingRPG.Entities.DropInstances;

public abstract class BaseItemInstance : IDropInstance
{
    protected Vector2 Position { get; set; }
    protected Point Size { get; set; } = new(16, 16);
    protected double Depth { get; set; } = -1;
    protected IDroppable Droppable { get; set; }
    protected IItem Item { get; set; }

    public virtual Vector2 GetPosition() => Position;

    public virtual Vector2 SetPosition(Vector2 position) => Position = position;

    public virtual double GetDepth() => Depth;

    public virtual Point GetSize() => Size;

    public virtual RectangleF GetCollisionBox() => new(Position, Size);

    public virtual Texture2D GetSpriteSheet()
    {
        return Assets.Instance.IconSpriteSheet;
    }

    public virtual Rectangle GetTextureRectangle() => Item.GetSourceRectangle();

    public virtual bool CanDrop() => true;

    public virtual IDroppable GetDroppable() => Droppable;

    public virtual void OnObtain()
    {
        AddItemToInventory(Item);
    }

    protected static void AddItemToInventory<T>(int quantity = 1) where T : IItem, new()
    {
        AddItemToInventory(new T(), quantity);
    }
    
    protected static void AddItemToInventory(IItem item, int quantity = 1)
    {
        var player = GameManager.PlayerInfo;

        player.Inventory[item.GetId()] += quantity;
        GameManager.MaterialGrabSfx01.Play(0.3F, 0F, 0F);

        foreach (var questInstance in player.QuestBook.GetActiveQuests())
        {
            if (questInstance is FetchQuestInstance fetchQuestInstance)
            {
                fetchQuestInstance.AddCollectedItem(item.GetId(), quantity);
            }
        }
    }
}