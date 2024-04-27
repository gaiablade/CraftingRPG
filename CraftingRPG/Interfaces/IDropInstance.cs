namespace CraftingRPG.Interfaces;

public interface IDropInstance : IInstance
{
    public bool CanDrop();
    public IDroppable GetDroppable();
    public void OnObtain();
}
