using System;
using Better.Interactor.Runtime.Interface;
using Better.Interactor.Runtime.Test;
using UnityEngine;

namespace Better.Interactor.Runtime.Models
{
    public enum InteractionState
    {
        None,
        PreInteract,
        Interact,
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

        public void SetCurrentState(InteractionState state)
        {
            CurrentState = state;
        }

        public bool Intersects(OrientedBoundingBox bounds)
        {
            return Interactable.Bounds.Intersects(bounds);
        }
        
        public bool Raycast(Ray ray, out Vector3 hitPoint)
        {
            return Interactable.Bounds.Raycast(ray, out hitPoint);
        }
    }
}