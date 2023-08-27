﻿using CraftingRPG.Enums;

namespace CraftingRPG.Interfaces;

public interface IState
{
    public void Update();
    public void Render();
    public ToState GetToState();
}
