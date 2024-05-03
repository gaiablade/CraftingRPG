using Microsoft.Xna.Framework;

namespace CraftingRPG.MapManagement;

public class LoadingZone
{
    public string ToMap { get; set; }
    public Vector2 ToPosition { get; set; }
    public Point Position { get; set; }
    public Point Size { get; set; }
    public int MoveOut { get; set; }
    public int MoveIn { get; set; }

    public Rectangle GetCollider() => new(Position, Size);
}