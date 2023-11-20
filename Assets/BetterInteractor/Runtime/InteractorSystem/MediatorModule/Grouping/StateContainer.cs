using Better.Interactor.Runtime.BoundingBox;
using Better.Interactor.Runtime.Interface;
using UnityEngine;

namespace Better.Interactor.Runtime.MediatorModule
{
    public class StateContainer
    {
        private IInteractable _interactable;
        private InteractionState _currenctState;

        public InteractionState CurrenctState => _currenctState;

        public IInteractable Interactable => _interactable;

        public StateContainer(IInteractable interactable)
        {
            _interactable = interactable;
            _currenctState = InteractionState.None;
        }

        public void SetCurrentState(InteractionState state)
        {
            _currenctState = state;
        }

        public bool Intersects(OrientedBoundingBox bounds)
        {
            return Interactable.Bounds.Intersects(bounds);
        }

        public bool Raycast(Ray ray, out Vector3 position)
        {
            return Interactable.Bounds.Raycast(ray, out position);
        }
    }
}