using Microsoft.Xna.Framework;

namespace CraftingRPG.ActorBehaviors;

public class BehaviorPath
{
    public Vector2 Start { get; set; }
    public Vector2 End { get; set; }
    public Vector2 Current { get; set; }
    public double MaxTime { get; set; }
}