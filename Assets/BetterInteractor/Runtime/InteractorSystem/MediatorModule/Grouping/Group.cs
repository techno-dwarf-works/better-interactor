using System.Collections;
using System.Collections.Generic;
using Better.Interactor.Runtime.BoundingBox;
using Better.Interactor.Runtime.Interface;
using UnityEngine;

namespace Better.Interactor.Runtime.MediatorModule
{
    public class Group : IEnumerable<StateContainer>
    {
        private Dictionary<IInteractable, StateContainer> _interactable = new();
        private TrackedOBB groupBounds = new TrackedOBB();
        private int _count;
        private bool _isReadOnly;

        public int Count => _count;

        public bool IsReadOnly => _isReadOnly;

        public int Mask { get; }

        public Group(int mask)
        {
            Mask = mask;
        }

        public OrientedBoundingBox Bounds => groupBounds;
        
        public void Add(IInteractable interactable)
        {
            if (interactable == null) return;
            groupBounds.Encapsulate(interactable.Bounds);
            _interactable.Add(interactable, new StateContainer(interactable));
        }

        public bool Remove(IInteractable interactable)
        {
            if (interactable == null) return false;
            var removed = _interactable.Remove(interactable);
            if (!removed) return false;
            groupBounds = new TrackedOBB();
            foreach (var interactableStack in _interactable.Keys)
            {
                groupBounds.Encapsulate(interactableStack.Bounds);
            }

            return true;
        }

        public Vector3 GetClosestPointOnBounds(Vector3 point)
        {
            return groupBounds.GetClosestPointOnBounds(point);
        }

        public bool Intersects(OrientedBoundingBox bounds)
        {
            return groupBounds.Intersects(bounds);
        }

        public float SqrDistanceTo(OrientedBoundingBox interactableBounds)
        {
            return groupBounds.SqrDistanceTo(interactableBounds);
        }

        public IEnumerator<StateContainer> GetEnumerator()
        {
            return _interactable.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}