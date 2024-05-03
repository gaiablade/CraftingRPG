using System.Collections.Generic;
using CraftingRPG.Enums;
using CraftingRPG.InputManagement;
using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CraftingRPG.SourceRectangleProviders;

public class InputActionKeySourceRectangleProvider : ISourceRectangleProvider<InputAction>
{
    private static readonly Point DefaultSize = new Point(32, 32);

    private readonly IDictionary<Keys, Rectangle> SourceRectangles = new Dictionary<Keys, Rectangle>
    {
        { Keys.NumPad1, CreateRectangle(0, 0, DefaultSize) },
        { Keys.C, CreateRectangle(1, 2, DefaultSize) },
        { Keys.I, CreateRectangle(3, 1, DefaultSize) },
        { Keys.K, CreateRectangle(13, 1, DefaultSize) },
        { Keys.Q, CreateRectangle(12, 0, DefaultSize) },
        { Keys.Z, CreateRectangle(15, 1, DefaultSize) },
        { Keys.Up, CreateRectangle(5, 4, DefaultSize) },
        { Keys.Down, CreateRectangle(6, 4, DefaultSize) },
        { Keys.Right, CreateRectangle(7, 4, DefaultSize) },
        { Keys.Left, CreateRectangle(8, 4, DefaultSize) },
        { Keys.LeftControl, CreateRectangle(13, 2, DefaultSize) }
    };

    public Rectangle GetSourceRectangle(InputAction @object)
    {
        var keybinding = InputManager.Instance.GetKeyForAction(@object);
        var found = SourceRectangles.TryGetValue(keybinding, out var rectangle);
        return found ? rectangle : CreateRectangle(0, 6, DefaultSize);
    }

    private static Rectangle CreateRectangle(int x, int y, Point size) =>
        new Rectangle(new Point(x * DefaultSize.X, y * DefaultSize.Y), size);
}