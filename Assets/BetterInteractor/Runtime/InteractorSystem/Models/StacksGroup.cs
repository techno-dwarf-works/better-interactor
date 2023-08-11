using System;
using System.Collections.Generic;

namespace Better.Interactor.Runtime.Models
{
    public class StacksGroup
    {
        private Dictionary<InteractionState, List<InteractableStack>> _inRangeStacks = new Dictionary<InteractionState, List<InteractableStack>>()
        {
            { InteractionState.None, new List<InteractableStack>() },
            { InteractionState.PreInteract, new List<InteractableStack>() },
            { InteractionState.PostInteract, new List<InteractableStack>() },
        };

        private Dictionary<InteractionState, List<InteractableStack>> _outRangeStacks = new Dictionary<InteractionState, List<InteractableStack>>()
        {
            { InteractionState.None, new List<InteractableStack>() },
            { InteractionState.PreInteract, new List<InteractableStack>() },
            { InteractionState.PostInteract, new List<InteractableStack>() },
        };

        public void Clear()
        {
            _inRangeStacks[InteractionState.None].Clear();
            _inRangeStacks[InteractionState.PreInteract].Clear();
            _inRangeStacks[InteractionState.PostInteract].Clear();
            
            _outRangeStacks[InteractionState.None].Clear();
            _outRangeStacks[InteractionState.PreInteract].Clear();
            _outRangeStacks[InteractionState.PostInteract].Clear();
        }

        public List<InteractableStack> GetInRangeStacks(InteractionState state)
        {
            return _inRangeStacks.TryGetValue(state, out var stacks) ? stacks : null;
        }

        public List<InteractableStack> GetOutRangeStacks(InteractionState state)
        {
            return _outRangeStacks.TryGetValue(state, out var stacks) ? stacks : null;
        }

        public void AddOutRange(InteractableStack stack)
        {
            switch (stack.CurrentState)
            {
                case InteractionState.None:
                    _outRangeStacks[InteractionState.None].Add(stack);
                    break;
                case InteractionState.PreInteract:
                    _outRangeStacks[InteractionState.PostInteract].Add(stack);
                    break;
                case InteractionState.PostInteract:
                    _outRangeStacks[InteractionState.None].Add(stack);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void AddInRange(InteractableStack stack)
        {
            switch (stack.CurrentState)
            {
                case InteractionState.None:
                    _inRangeStacks[InteractionState.PreInteract].Add(stack);
                    break;
                case InteractionState.PreInteract:
                    _inRangeStacks[InteractionState.None].Add(stack);
                    break;
                case InteractionState.PostInteract:
                    _inRangeStacks[InteractionState.PreInteract].Add(stack);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}