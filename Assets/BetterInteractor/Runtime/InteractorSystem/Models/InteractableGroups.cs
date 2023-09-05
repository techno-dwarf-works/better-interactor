using System.Collections;
using System.Collections.Generic;
using Better.Interactor.Runtime.Interface;
using Better.Interactor.Runtime.Test;
using UnityEngine;

namespace Better.Interactor.Runtime.Models
{
    public class InteractableGroups : IEnumerable<InteractableGroup>
    {
        private static StacksGroup _interactableStacks = new StacksGroup();
        private readonly List<InteractableGroup> _groups = new List<InteractableGroup>();
        public int Count => _groups.Count;

        public StacksGroup GetIntersecting(OrientedBoundingBox bounds)
        {
            _interactableStacks.Clear();
            foreach (var group in _groups)
            {
                if (!group.Intersects(bounds)) continue;
                foreach (var stack in group)
                {
                    if (stack.Intersects(bounds))
                    {
                        _interactableStacks.AddInRange(stack);
                    }
                    else
                    {
                        _interactableStacks.AddOutRange(stack);
                    }
                }
            }

            return _interactableStacks;
        }

        public void TrackBoxes()
        {
            foreach (var group in _groups)
            {
                group.TrackBoxes();
            }
        }

        public void AddInteractable(IInteractable interactable)
        {
            InteractableGroup selectedGroup = null;
            const float distance = 10f;
            foreach (var group in _groups)
            {
                var currentDistance = group.DistanceTo(interactable.Bounds);
                if (!(currentDistance <= distance)) continue;
                selectedGroup = group;
            }

            if (selectedGroup == null)
            {
                selectedGroup = new InteractableGroup();
                _groups.Add(selectedGroup);
            }
            
            selectedGroup.AddInteractable(interactable);
        }

        IEnumerator<InteractableGroup> IEnumerable<InteractableGroup>.GetEnumerator()
        {
            return _groups.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _groups.GetEnumerator();
        }
    }
}