using Better.Interactor.Runtime.MediatorModule;

namespace Better.Interactor.Runtime.Interface
{
    public class InteractionHandler : IInteractionHandler
    {
        ReturnState IInteractionHandler.PreInteract(IInteractable interactable)
        {
            throw new System.NotImplementedException();
        }

        ReturnState IInteractionHandler.Interact(IInteractable interactable)
        {
            throw new System.NotImplementedException();
        }

        ReturnState IInteractionHandler.PostInteract(IInteractable interactable)
        {
            throw new System.NotImplementedException();
        }
    }
    

    public class InteractionHandler<TInteractor, TInteractable> : InteractionHandler where TInteractable : IInteractable where TInteractor : IInteractorContainer
    {
        
    }
}