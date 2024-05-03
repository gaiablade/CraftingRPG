using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using CraftingRPG.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CraftingRPG.InputManagement;

public class InputManager
{
    public static InputManager Instance = new();

    private const string KeybindingsConfigurationPath = "keyconfig.xml";

    public Dictionary<InputAction, Keys> Keybindings { get; private set; } = new();
    public Dictionary<Keys, double> DurationHeld { get; private set; } = new();
    public Dictionary<Keys, KeyPressState> KeyPressStates { get; private set; } = new();
    public ISet<Keys> IgnoreUntilNotPressed { get; private set; } = new HashSet<Keys>();

    public double GetDurationHeld(InputAction action)
    {
        var actionKey = Keybindings[action];
        return DurationHeld[actionKey];
    }

    public KeyPressState GetKeyPressState(InputAction action)
    {
        var actionKey = Keybindings[action];
        return KeyPressStates[actionKey];
    }

    public bool IsKeyPressed(InputAction action)
    {
        var actionKey = Keybindings[action];
        return KeyPressStates[actionKey] == KeyPressState.Pressed;
    }

    public bool IsKeyHeld(InputAction action)
    {
        var actionKey = Keybindings[action];
        return KeyPressStates[actionKey] == KeyPressState.Held;
    }

    public bool IsKeyNotPressed(InputAction action)
    {
        var actionKey = Keybindings[action];
        return KeyPressStates[actionKey] == KeyPressState.NotPressed;
    }

    public void Debounce(InputAction action)
    {
        var actionKey = Keybindings[action];
        DurationHeld[actionKey] = 0;
        KeyPressStates[actionKey] = KeyPressState.NotPressed;
        IgnoreUntilNotPressed.Add(actionKey);
    }

    public void Update(KeyboardState keyState, GameTime gameTime)
    {
        var pressedKeys = keyState.GetPressedKeys();

        foreach (var key in Keybindings.Values.Distinct())
        {
            var keyIsPressed = false;
            foreach (var pressedKey in pressedKeys)
            {
                if (pressedKey == key)
                {
                    keyIsPressed = true;

                    if (!IgnoreUntilNotPressed.Contains(key))
                    {
                        if (DurationHeld[key] == 0)
                        {
                            KeyPressStates[key] = KeyPressState.Pressed;
                        }
                        else
                        {
                            KeyPressStates[key] = KeyPressState.Held;
                        }

                        DurationHeld[key] += gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }
            }

            if (!keyIsPressed)
            {
                if (IgnoreUntilNotPressed.Contains(key))
                {
                    IgnoreUntilNotPressed.Remove(key);
                }
                DurationHeld[key] = 0;
                KeyPressStates[key] = KeyPressState.NotPressed;
            }
        }
    }

    public void LoadKeybindingConfiguration()
    {
        using var reader = XmlReader.Create(KeybindingsConfigurationPath);
        var serializer = new XmlSerializer(typeof(KeyConfig));
        var keyConfig = serializer.Deserialize(reader) as KeyConfig;

        if (keyConfig == null)
        {
            throw new ArgumentOutOfRangeException();
        }

        foreach (var config in keyConfig.Items)
        {
            var key = GetKeyForConfiguredName(config.Key);
            var action = (InputAction)Enum.Parse(typeof(InputAction), config.Name);
            Keybindings[action] = key;
        }

        foreach (var key in Keybindings.Values)
        {
            DurationHeld[key] = 0;
            KeyPressStates[key] = KeyPressState.NotPressed;
        }
    }

    private Keys GetKeyForConfiguredName(string keyName)
    {
        var couldParse = Enum.TryParse(typeof(Keys), keyName, true, out var key);

        if (couldParse && key != null)
        {
            return (Keys)key;
        }

        switch (keyName)
        {
            case "ArrowLeft":
                return Keys.Left;
            case "ArrowRight":
                return Keys.Right;
            case "ArrowUp":
                return Keys.Up;
            case "ArrowDown":
                return Keys.Down;
            case "LeftCtrl":
                return Keys.LeftControl;
            default:
                return Keys.None;
        }
    }
}