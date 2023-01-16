using UnityEngine;

namespace Better.Interactor.Runtime.Interface
{
    public interface IInteractable
    {
        public void InvokeGaze();
        public Vector3 TrackedPosition { get; }
    }
}
