﻿using Better.Interactor.Runtime.MediatorModule;
using Better.Interactor.Runtime.Models;

namespace Better.Interactor.Runtime.Interface
{
    public interface IInteractionHandler
    {
        /// <summary>
        /// PreInteract behavior for IInteractor controller.
        /// Respond with InteractorUI state.
        /// </summary>
        /// <param name="interactable"></param>
        /// <returns></returns>
        public ReturnState PreInteract(IInteractable interactable);

        /// <summary>
        /// Interact behavior for IInteractor controller. Interact calls when user press interact button.
        /// Respond with InteractorUI state.
        /// </summary>
        /// <param name="interactable"></param>
        /// <returns></returns>
        public ReturnState Interact(IInteractable interactable);

        /// <summary>
        /// PostInteract behavior for IInteractor controller.
        /// Respond with InteractorUI state.
        /// </summary>
        /// <param name="interactable"></param>
        /// <returns></returns>
        public ReturnState PostInteract(IInteractable interactable);
    }
}
