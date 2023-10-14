using System;
using System.Collections.Generic;
using Better.Interactor.Runtime.Interface;
using UnityEngine;

namespace Better.Interactor.Runtime
{
    public static class InteractorSystem
    {
        private static Interactor _interactor;
        private static readonly List<IInteractionHandler> IdleInteractors;
        private static readonly List<IInteractable> IdleInteractables;
        private static bool _isInteractorRegistered;

        static InteractorSystem()
        {
            IdleInteractors = new List<IInteractionHandler>();
            IdleInteractables = new List<IInteractable>();
        }

        public static void AssignPlayer(IPlayerContainer playerContainer)
        {
            _interactor.AssignPlayer(playerContainer);
        }

        public static void Pause(bool pause)
        {
            _interactor.SetPause(pause);
        }

        internal static void RegisterInteractorRegistry(Interactor interactor)
        {
            if (_isInteractorRegistered)
            {
                Debug.LogWarning($"Only one {nameof(_interactor)} allowed in the same time!");
                return;
            }

            _interactor = interactor;
            _isInteractorRegistered = true;
            RegisterIdle(IdleInteractors, _interactor.RegisterInteractor);
            RegisterIdle(IdleInteractables, _interactor.RegisterInteractable);
        }

        private static void RegisterIdle<T>(List<T> items, Action<T> registerActon)
        {
            if (items.Count <= 0) return;
            for (var i = 0; i < items.Count; i++)
            {
                registerActon(items[i]);
            }

            items.Clear();
        }

        internal static void UnregisterInteractorRegistry(Interactor interactor)
        {
            if (_interactor != interactor)
            {
                Debug.LogWarning($"Only registered {nameof(_interactor)} allowed to unregister");
                return;
            }

            _interactor = null;
            _isInteractorRegistered = false;
        }
        
        private static void AddIdle<T>(List<T> idle, T item)
        {
            if (!idle.Contains(item))
            {
                idle.Add(item);
            }
        }

        public static void Register(IInteractionHandler interactionHandler)
        {
            if (_isInteractorRegistered)
            {
                _interactor.RegisterInteractor(interactionHandler);
            }
            else
            {
                AddIdle(IdleInteractors, interactionHandler);
            }
        }
        
        public static void Unregister(IInteractionHandler interactionHandler)
        {
            if (_isInteractorRegistered)
            {
                _interactor.UnregisterInteractor(interactionHandler);
            }
        }

        public static void UnregisterInteractable(IInteractable interactable)
        {
            if (_isInteractorRegistered)
            {
                _interactor.UnregisterInteractable(interactable);
            }
        }

        public static void RegisterInteractable(IInteractable interactable)
        {
            if (_isInteractorRegistered)
            {
                _interactor.RegisterInteractable(interactable);
            }
            else
            {
                AddIdle(IdleInteractables, interactable);
            }
        }
    }
}