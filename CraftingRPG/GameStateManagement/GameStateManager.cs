using System.Collections.Generic;
using CraftingRPG.Interfaces;

namespace CraftingRPG.GameStateManagement;

public class GameStateManager
{
    private class StateAddRequest
    {
        public IGameState GameState;
        public bool KeepPreviousState;
    }
    private readonly Queue<StateAddRequest> StateAddRequests = new();
    private int StatePopRequests;

    public static readonly GameStateManager Instance = new();

    public Stack<IGameState> States { get; set; } = new();

    public IGameState CurrentGameState => States.Peek();

    public void PushState(IGameState newGameState, bool keepPreviousState = false)
    {
        StateAddRequests.Enqueue(new StateAddRequest { GameState = newGameState, KeepPreviousState = keepPreviousState });
    }

    public void PushState<T>(bool keepPreviousState = false) where T : IGameState, new()
    {
        StateAddRequests.Enqueue(new StateAddRequest { GameState = new T(), KeepPreviousState = keepPreviousState });
    }

    public void PopState()
    {
        StatePopRequests++;
    }

    public void ProcessStateRequests()
    {
        while (StateAddRequests.Count > 0)
        {
            var request = StateAddRequests.Dequeue();
            if (!request.KeepPreviousState)
            {
                States.Pop();
            }
            States.Push(request.GameState);
        }

        while (StatePopRequests > 0)
        {
            States.Pop();
            StatePopRequests--;
        }
    }
}
