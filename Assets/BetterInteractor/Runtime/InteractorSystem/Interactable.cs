using Better.Interactor.Runtime.Interface;
using Better.Interactor.Runtime.Models;
using UnityEngine;

namespace Better.Interactor.Runtime
{
    public abstract class Interactable : MonoBehaviour, IInteractable
    {
        public abstract void InvokeGaze();
        public abstract OrientedBoundingBox GetBounds();

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