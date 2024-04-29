using CraftingRPG.Items;

namespace CraftingRPG.Entities.DropInstances;

public class EmptyBottleInstance : BaseItemInstance
{
    public EmptyBottleInstance()
    {
        Item = new EmptyBottleItem();
        Droppable = new ItemDrop<EmptyBottleItem>();
    }
}