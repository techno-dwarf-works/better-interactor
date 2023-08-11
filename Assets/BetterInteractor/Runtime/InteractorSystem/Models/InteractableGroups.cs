using System.Collections.Generic;
using Better.Interactor.Runtime.Interface;
using UnityEngine;

namespace Better.Interactor.Runtime.Models
{
    public class InteractableGroups
    {
        private static StacksGroup _group = new StacksGroup();
        private readonly List<InteractableGroup> _groups = new List<InteractableGroup>();
        public int Count => _groups.Count;

        public StacksGroup GetIntersecting(OrientedBoundingBox bounds)
        {
            _group.Clear();
            foreach (var group in _groups)
            {
                if (!group.Intersects(bounds)) continue;
                foreach (var stack in group)
                {
                    if (stack.Intersects(bounds))
                    {
                        _group.AddInRange(stack);
                    }
                    else
                    {
                        _group.AddOutRange(stack);
                    }
                }
            }

            return _group;
        }

#if UNITY_EDITOR
        public void DrawGizmos()
        {
            foreach (var group in _groups)
            {
                group.DrawGizmos();
                foreach (var stack in group)
                {
                    stack.DrawGizmos();
                }
            }
        }
#endif

        public void AddInteractable(IInteractable interactable)
        {
            InteractableGroup selectedGroup = null;
            if (_groups.Count <= 0)
            {
                selectedGroup = new InteractableGroup();
                _groups.Add(selectedGroup);
            }
            else
            {
                selectedGroup = _groups[0];
            }

            selectedGroup.AddInteractable(interactable);
        }
    }
}