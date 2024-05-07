using System.Collections.Generic;
using CraftingRPG.AssetManagement;
using CraftingRPG.Global;
using CraftingRPG.Interfaces;
using CraftingRPG.Items;
using Microsoft.Xna.Framework;

namespace CraftingRPG.MapObjects;

public class Chest : BaseMapObject
{
    private static readonly IDictionary<int, IItem> ChestContents = new Dictionary<int, IItem>
    {
        { 1, EmptyBottleItem.Instance }
    };

    private readonly int ChestId;
    private readonly IItem Contents;

    public Chest(int chestId)
    {
        ChestId = chestId;
        Contents = ChestContents[ChestId];

        Flags.ChestOpened.TryAdd(ChestId, false);
    }

    public override object OnInteract()
    {
        if (!Flags.ChestOpened[ChestId])
        {
            Flags.ChestOpened[ChestId] = true;
            Assets.Instance.BagOpen.Play(0.5F, 0F, 0F);
            Attributes.IsInteractive = false;
            return Contents;
        }

        return null;
    }

    public override Rectangle GetTextureRectangle()
    {
        if (Flags.ChestOpened[ChestId])
        {
            return new Rectangle(new Point(48, 0), SourceRectangle.Size);
        }

        return SourceRectangle;
    }
}