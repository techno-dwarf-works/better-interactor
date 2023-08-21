using Better.Interactor.Runtime.Interface;
using Better.Interactor.Runtime.Models;
using Better.Interactor.Runtime.Test;
using UnityEngine;

namespace Better.Interactor.Runtime
{
    public abstract class Interactable : MonoBehaviour, IInteractable
    {
        public abstract void InvokeGaze();
        public abstract OrientedBoundingBox Bounds { get; }

        private void OnEnable()
        {
            InteractorSystem.RegisterInteractable(this);
        }

        private void OnDisable()
        {
            InteractorSystem.UnregisterInteractable(this);
        }
    }
}