using System.Collections.Generic;
using Better.Interactor.Runtime.Interface;
using UnityEngine;

namespace Better.Interactor.Runtime
{
    public static class InteractorContainer
    {
        private static InteractorRegistry _interactorRegistry;
        private static readonly List<IInteractor> IdleInteractors;
        private static bool _isInteractorRegistered;

        static InteractorContainer()
        {
            IdleInteractors = new List<IInteractor>();
        }

        public static void AssignPlayerHead(IPlayerHead player)
        {
            _interactorRegistry.AssignPlayerHead(player);
        }

        internal static void RegisterInteractorRegistry(InteractorRegistry interactorRegistry)
        {
            if (_isInteractorRegistered)
            {
                Debug.LogWarning($"Only one {nameof(_interactorRegistry)} allowed in the same time!");
                return;
            }

            _interactorRegistry = interactorRegistry;
            _isInteractorRegistered = true;
            RegisterIdleInteractors();
        }

        internal static void UnregisterInteractorRegistry(InteractorRegistry interactorRegistry)
        {
            if (_interactorRegistry != interactorRegistry)
            {
                Debug.LogWarning($"Only registered {nameof(_interactorRegistry)} allowed to unregister");
                return;
            }

            _interactorRegistry = null;
            _isInteractorRegistered = false;
        }

        private static void RegisterIdleInteractors()
        {
            if (IdleInteractors.Count <= 0) return;
            for (int i = 0; i < IdleInteractors.Count; i++)
            {
                _interactorRegistry.RegisterInteractor(IdleInteractors[i]);
            }

            IdleInteractors.Clear();
        }

        private static void AddIdleInteractor(IInteractor interactor)
        {
            if (IdleInteractors.Contains(interactor))
            {
                IdleInteractors.Add(interactor);
            }
        }

        public static void Register(IInteractor interactor)
        {
            if (_isInteractorRegistered)
            {
                RegisterIdleInteractors();

                _interactorRegistry.RegisterInteractor(interactor);
            }
            else
            {
                AddIdleInteractor(interactor);
            }
        }
        
        public static void Unregister(IInteractor interactor)
        {
            if (_isInteractorRegistered)
            {
                _interactorRegistry.UnregisterInteractor(interactor);
            }
        }
    }
}