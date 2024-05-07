using System.Collections.Generic;

namespace CraftingRPG.Global;

public static class Flags
{
    // Paused
    public static bool IsPaused = false;
    // Chest opened
    public static IDictionary<int, bool> ChestOpened = new Dictionary<int, bool>();
    // Debug
    public const bool DebugShowEnemyHitBoxes = false;
    public const bool DebugMuteMusic = false;
}
