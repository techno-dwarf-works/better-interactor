﻿using System.Collections;
using System.Collections.Generic;
using Better.Interactor.Runtime.Interface;
using UnityEngine;

namespace Better.Interactor.Runtime.Models
{
    public class InteractableGroup : IEnumerable<InteractableStack>
    {
        private StackDictionary<IInteractable, InteractableStack> _interactable = new();
        private OrientedBoundingBox groupBounds = new OrientedBoundingBox();

        public void AddInteractable(IInteractable interactable)
        {
            groupBounds.Encapsulate(interactable.GetBounds());
            _interactable.Push(interactable, new InteractableStack(interactable));
        }

        public void RemoveInteractable(IInteractable interactable)
        {
            _interactable.Remove(interactable);
            groupBounds = new OrientedBoundingBox(Vector3.zero, Vector3.zero, Quaternion.identity);
            foreach (var interactableStack in _interactable)
            {
                groupBounds.Encapsulate(interactableStack.Interactable.GetBounds());
            }
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

#if UNITY_EDITOR
        public void DrawGizmos()
        {
            groupBounds.DrawGizmos();
        }
#endif
    }
}