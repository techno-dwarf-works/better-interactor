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

        public static ReturnState GetValid(IInteractable interactable)
        {
            return new ReturnState(interactable, true);
        }        
        
        public static ReturnState GetNonValid(IInteractable interactable)
        {
            return new ReturnState(interactable, false);
        }
    }
}
