using Better.Interactor.Runtime.BoundingBox;
using UnityEngine;

namespace Better.Interactor.Runtime.Interface
{
    public interface IInteractable
    {
        public void InvokeGaze();

        public Transform transform { get; }
        public OrientedBoundingBox Bounds { get; }
        public int Mask { get; }
    }
}
