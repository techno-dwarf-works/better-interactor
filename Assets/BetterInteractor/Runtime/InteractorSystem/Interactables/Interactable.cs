using Better.Interactor.Runtime.BoundingBox;
using Better.Interactor.Runtime.Interface;
using Better.Interactor.Runtime.MediatorModule;
using UnityEngine;

namespace Better.Interactor.Runtime.Interactables
{
    public abstract class Interactable : MonoBehaviour, IInteractable
    {
        public abstract void InvokeGaze();
        public abstract OrientedBoundingBox Bounds { get; }
        public abstract int Mask { get; }

        private void OnEnable()
        {
            InteractorSystem.Register(this);
        }

        private void OnDisable()
        {
            InteractorSystem.Unregister(this);
        }
    }
}