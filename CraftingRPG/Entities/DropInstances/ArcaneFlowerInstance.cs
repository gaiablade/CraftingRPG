using CraftingRPG.Items;

namespace CraftingRPG.Entities.DropInstances;

public class ArcaneFlowerInstance : BaseItemInstance
{
    public ArcaneFlowerInstance()
    {
        Item = new ArcaneFlowerItem();
        Droppable = new ItemDrop<ArcaneFlowerItem>();
    }
}