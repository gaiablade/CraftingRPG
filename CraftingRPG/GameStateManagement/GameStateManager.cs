using System.Collections.Generic;
using CraftingRPG.Interfaces;

namespace CraftingRPG.GameStateManagement;

public class GameStateManager
{
    private class StateAddRequest
    {
        public IState State;
        public bool KeepPreviousState;
    }
    private Queue<StateAddRequest> StateAddRequests = new();
    private int StatePopRequests = 0;

    public static GameStateManager Instance = new();

    public Stack<IState> States { get; set; } = new();

    public IState CurrentState => States.Peek();

    public GameStateManager()
    {
    }

    public void PushState(IState newState, bool keepPreviousState = false)
    {
        StateAddRequests.Enqueue(new StateAddRequest { State = newState, KeepPreviousState = keepPreviousState });
    }

    public void PushState<T>(bool keepPreviousState = false) where T : IState, new()
    {
        StateAddRequests.Enqueue(new StateAddRequest { State = new T(), KeepPreviousState = keepPreviousState });
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
            States.Push(request.State);
        }

        while (StatePopRequests > 0)
        {
            States.Pop();
            StatePopRequests--;
        }
    }
}
