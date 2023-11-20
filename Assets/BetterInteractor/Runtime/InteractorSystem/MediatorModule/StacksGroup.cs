using System;
using System.Collections.Generic;
using Better.Interactor.Runtime.Interface;

namespace Better.Interactor.Runtime.MediatorModule
{
    public class StacksGroup
    {
        private Dictionary<InteractionState, List<StateContainer>> _inRangeStacks = new Dictionary<InteractionState, List<StateContainer>>()
        {
            { InteractionState.None, new List<StateContainer>() },
            { InteractionState.PreInteract, new List<StateContainer>() },
            { InteractionState.PostInteract, new List<StateContainer>() },
            { InteractionState.Interact, new List<StateContainer>() },
        };

        private Dictionary<InteractionState, List<StateContainer>> _outRangeStacks = new Dictionary<InteractionState, List<StateContainer>>()
        {
            { InteractionState.None, new List<StateContainer>() },
            { InteractionState.PreInteract, new List<StateContainer>() },
            { InteractionState.PostInteract, new List<StateContainer>() },
            { InteractionState.Interact, new List<StateContainer>() },
        };

        public void Clear()
        {
            _inRangeStacks[InteractionState.None].Clear();
            _inRangeStacks[InteractionState.PreInteract].Clear();
            _inRangeStacks[InteractionState.PostInteract].Clear();
            _inRangeStacks[InteractionState.Interact].Clear();

            _outRangeStacks[InteractionState.None].Clear();
            _outRangeStacks[InteractionState.PreInteract].Clear();
            _outRangeStacks[InteractionState.PostInteract].Clear();
            _outRangeStacks[InteractionState.Interact].Clear();
        }

        public List<StateContainer> GetInRangeStacks(InteractionState state)
        {
            return _inRangeStacks.TryGetValue(state, out var stacks) ? stacks : null;
        }

        public List<StateContainer> GetOutRangeStacks(InteractionState state)
        {
            return _outRangeStacks.TryGetValue(state, out var stacks) ? stacks : null;
        }

        public void AddOutRange(StateContainer container)
        {
            switch (container.CurrenctState)
            {
                case InteractionState.None:
                    _outRangeStacks[InteractionState.None].Add(container);
                    break;
                case InteractionState.PreInteract:
                    _outRangeStacks[InteractionState.PostInteract].Add(container);
                    break;
                case InteractionState.PostInteract:
                    _outRangeStacks[InteractionState.None].Add(container);
                    break;
                case InteractionState.Interact:
                    _outRangeStacks[InteractionState.PostInteract].Add(container);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void AddInRange(StateContainer container)
        {
            switch (container.CurrenctState)
            {
                case InteractionState.None:
                    _inRangeStacks[InteractionState.PreInteract].Add(container);
                    break;
                case InteractionState.PreInteract:
                    _inRangeStacks[InteractionState.PreInteract].Add(container);
                    break;
                case InteractionState.PostInteract:
                    _inRangeStacks[InteractionState.PreInteract].Add(container);
                    break;
                case InteractionState.Interact:
                    _inRangeStacks[InteractionState.PreInteract].Add(container);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}