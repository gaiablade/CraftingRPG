using CraftingRPG.Interfaces;
using System.Collections.Generic;

namespace CraftingRPG.Entities;

public class StateManager
{
    private class StateAddRequest
    {
        public IState State;
        public bool KeepPreviousState;
    }
    private Queue<StateAddRequest> StateAddRequests = new();
    private int StatePopRequests = 0;

    public static StateManager Instance = new();

    public Stack<IState> States { get; set; } = new();

    public IState CurrentState => States.Peek();

    public StateManager()
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
