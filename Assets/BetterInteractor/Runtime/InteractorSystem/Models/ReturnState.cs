using Better.Interactor.Runtime.Interface;

namespace Better.Interactor.Runtime.Models
{
    public struct ReturnState
    {
        public ReturnState(IInteractable interactable, bool value)
        {
            _interactable = interactable;
            Value = value;
        }

        private readonly IInteractable _interactable;

        public bool Value { get; }

        public void InvokeGaze()
        {
            _interactable.InvokeGaze();
        }
    }
}
