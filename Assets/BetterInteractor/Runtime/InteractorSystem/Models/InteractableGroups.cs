using System.Collections.Generic;
using Better.Interactor.Runtime.Interface;
using Better.Interactor.Runtime.Test;
using UnityEngine;

namespace Better.Interactor.Runtime.Models
{
    public class InteractableGroups
    {
        private static StacksGroup _interactableStacks = new StacksGroup();
        private readonly List<InteractableGroup> _groups = new List<InteractableGroup>();
        public int Count => _groups.Count;

        public List<InteractableGroup> Groups => _groups;

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

        public void AddInteractable(IInteractable interactable)
        {
            InteractableGroup selectedGroup = null;
            var distance = 10f;
            foreach (var group in _groups)
            {
                var position = interactable.Bounds.GetWorldCenter();
                var closestPointOnBounds = group.GetClosestPointOnBounds(position);
                var currentDistance = Vector3.Distance(closestPointOnBounds, position);
                if (!(currentDistance <= distance)) continue;
                selectedGroup = group;
                distance = currentDistance;
            }

            if (selectedGroup == null)
            {
                selectedGroup = new InteractableGroup();
                _groups.Add(selectedGroup);
            }
            
            selectedGroup.AddInteractable(interactable);
        }
    }
}