namespace CraftingRPG.Interfaces;

public interface IDroppable
{
    public void OnObtain();

    public int GetSpriteSheetIndex();

    public bool CanDrop();

    public string GetName();
}
