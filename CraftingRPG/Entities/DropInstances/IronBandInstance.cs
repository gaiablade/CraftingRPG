using CraftingRPG.Items;

namespace CraftingRPG.Entities.DropInstances;

public class IronBandInstance : BaseItemInstance
{
    public IronBandInstance()
    {
        Item = new IronBandItem();
        Droppable = new ItemDrop<IronBandItem>();
    }
}