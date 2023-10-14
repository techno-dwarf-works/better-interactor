using System.Collections;
using System.Collections.Generic;
using Better.Interactor.Runtime.BoundingBox;
using Better.Interactor.Runtime.Interface;
using UnityEngine;

namespace Better.Interactor.Runtime.Models
{
    public class InteractableGroup : IEnumerable<InteractableStack>
    {
        private StackDictionary<IInteractable, InteractableStack> _interactable = new();
        private TrackedOBB groupBounds = new TrackedOBB();

        public OrientedBoundingBox Bounds => groupBounds;

        public void AddInteractable(IInteractable interactable)
        {
            groupBounds.Encapsulate(interactable.Bounds);
            _interactable.Push(interactable, new InteractableStack(interactable));
        }

        public void RemoveInteractable(IInteractable interactable)
        {
            _interactable.Remove(interactable);
            groupBounds = new TrackedOBB();
            foreach (var interactableStack in _interactable)
            {
                groupBounds.Encapsulate(interactableStack.Interactable.Bounds);
            }
        }

        public Vector3 GetClosestPointOnBounds(Vector3 point)
        {
            return groupBounds.GetClosestPointOnBounds(point);
        }

        public bool Intersects(OrientedBoundingBox bounds)
        {
            return groupBounds.Intersects(bounds);
        }

        IEnumerator<InteractableStack> IEnumerable<InteractableStack>.GetEnumerator()
        {
            return _interactable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _interactable.GetEnumerator();
        }

        public float SqrDistanceTo(OrientedBoundingBox interactableBounds)
        {
            return groupBounds.SqrDistanceTo(interactableBounds);
        }
    }
}