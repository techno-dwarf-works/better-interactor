using System;
using Better.Interactor.Runtime.Interface;
using UnityEngine;

namespace Better.Interactor.Runtime.Models
{
    public enum InteractionState
    {
        None,
        PreInteract,
        PostInteract
    }

    [Serializable]
    public class InteractableStack
    {
        public InteractableStack(IInteractable interactable)
        {
            Interactable = interactable;
        }

        public InteractionState CurrentState { get; private set; }

        public IInteractable Interactable { get; }
        public Vector3 TrackedPosition => Interactable.transform.position;

        public void SetCurrentState(InteractionState state)
        {
            CurrentState = state;
        }

        public bool Intersects(OrientedBoundingBox bounds)
        {
            return Interactable.GetBounds().Intersects(bounds);
        }
        
#if UNITY_EDITOR
        public void DrawGizmos()
        {
            Interactable.GetBounds().DrawGizmos();
        }
#endif
    }
}