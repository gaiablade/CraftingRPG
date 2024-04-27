namespace CraftingRPG.Interfaces;

public interface IDroppable
{
    public void OnObtain();

    public bool CanDrop();

    public string GetName();
}
