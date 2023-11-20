using System;
using System.Collections.Generic;
using Better.Interactor.Runtime.Interface;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Better.Interactor.Runtime.MediatorModule
{
    public static class InteractorSystem
    {
        private static InteractionMediator _interactionMediator = new InteractionMediator();
        private static List<IInteractionHandler> _idleInteractors;
        private static List<IInteractable> _idleInteractables;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            _idleInteractors = new List<IInteractionHandler>();
            _idleInteractables = new List<IInteractable>();
            var currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
            currentPlayerLoop.InsertAfter<Update.ScriptRunBehaviourUpdate>(UpdateDelegate, typeof(CustomUpdate));
            PlayerLoop.SetPlayerLoop(currentPlayerLoop);
        }

        public struct CustomUpdate
        {
        }

        private static void UpdateDelegate()
        {
            _interactionMediator.Update();
        }

        public static void Pause(bool pause)
        {
            _interactionMediator.SetPause(pause);
        }

        public static void Register(IInteractionHandler interactionHandler)
        {
            _interactionMediator.RegisterInteractor(interactionHandler);
        }

        public static void Unregister(IInteractionHandler interactionHandler)
        {
            _interactionMediator.UnregisterInteractor(interactionHandler);
        }

        public static void Unregister(IInteractable interactable)
        {
            _interactionMediator.UnregisterInteractable(interactable);
        }

        public static void Register(IInteractable interactable)
        {
            _interactionMediator.RegisterInteractable(interactable);
        }
        
        public static void Register(IInteractorContainer interactorContainer)
        {
            _interactionMediator.RegisterContainer(interactorContainer);
        }

        public static void Unregister(IInteractorContainer interactor)
        {
            _interactionMediator.UnregisterContainer(interactor);
        }
    }
}