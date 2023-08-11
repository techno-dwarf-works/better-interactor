using System;
using Better.Interactor.Runtime.Interface;
using Better.Interactor.Runtime.Models;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Better.Interactor.Runtime
{
    /// <summary>
    /// Class responsible for interaction with objects 
    /// </summary>
    public class Interactor : MonoBehaviour
    {
        private IPlayerContainer _playerContainer;

        private List<OnInteract> _onInteract = new List<OnInteract>();
        private List<OnInteract> _onPreInteract = new List<OnInteract>();
        private List<OnInteract> _onPostInteract = new List<OnInteract>();

        private InteractableGroups _groups = new InteractableGroups();
        private bool _isPause;

        /// <summary>
        /// Event receive state to show and neediness to show.
        /// </summary>
        //private event CustomEventTypes.HighlightableStateDelegate HighlightInteractable;
        private delegate ReturnState OnInteract(IInteractable interactor);

        private void Awake()
        {
            InteractorSystem.RegisterInteractorRegistry(this);
        }

        private void OnDestroy()
        {
            InteractorSystem.UnregisterInteractorRegistry(this);
        }

        internal void AssignPlayer(IPlayerContainer playerContainer)
        {
            _playerContainer = playerContainer;
        }

        internal void RegisterInteractor(IInteractionHandler interactionHandler)
        {
            _onPostInteract.Add(interactionHandler.PostInteract);
            _onInteract.Add(interactionHandler.Interact);
            _onPreInteract.Add(interactionHandler.PreInteract);
        }

        internal void UnregisterInteractor(IInteractionHandler interactionHandler)
        {
            _onPostInteract.Remove(interactionHandler.PostInteract);
            _onInteract.Remove(interactionHandler.Interact);
            _onPreInteract.Remove(interactionHandler.PreInteract);
        }

        /// <summary>
        /// Internal PreInteract behaviour.
        /// </summary>
        /// <param name="item"></param>
        private void PreInteractInternal(InteractableStack item)
        {
            item.Interactable.InvokeGaze();
            item.SetCurrentState(InteractionState.PreInteract);
            InvokeDelegate(item.Interactable, _onPreInteract);
        }

        /// <summary>
        /// Internal PreInteract behaviour.
        /// </summary>
        /// <param name="item"></param>
        private void PostInteractInternal(InteractableStack item)
        {
            item.Interactable.InvokeGaze();
            InvokeDelegate(item.Interactable, _onPostInteract);
            item.SetCurrentState(InteractionState.PostInteract);
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            _groups.DrawGizmos();
        }
#endif

        private void Update()
        {
            if (_isPause) return;
            if (_groups.Count <= 0) return;
            if (_playerContainer == null) return;
            var info = _groups.GetIntersecting(_playerContainer.Bounds);

            var item = FindLookingAt(info.GetInRangeStacks(InteractionState.PreInteract));

            if (item != null)
            {
                PreInteractInternal(item);

                //TODO: Move to New Input System
                //User calls interaction with object
                if (!Input.GetKeyDown(KeyCode.E))
                {
                    InvokeDelegate(item.Interactable, _onInteract);
                }
            }

            var outRangeStacks = info.GetOutRangeStacks(InteractionState.PostInteract);
            foreach (var interactableStack in outRangeStacks)
            {
                PostInteractInternal(interactableStack);
                interactableStack.SetCurrentState(InteractionState.None);
            }
        }

        /// <summary>
        /// Check if user currently looks on some interactable object
        /// </summary>
        /// <param name="interactables"></param>
        /// <returns></returns>
        private InteractableStack FindLookingAt(List<InteractableStack> interactables)
        {
            if (interactables == null || interactables.Count <= 0) return null;
            var playerHead = _playerContainer == null ? transform : _playerContainer.transform;

            var stack = MathfIsLookingAt(interactables, playerHead);

            if (ReferenceEquals(stack, null)) return null;
            return stack;
        }

        /// <summary>
        /// Mathematical calculation if user looks on some object
        /// </summary>
        /// <param name="interactables"></param>
        /// <param name="playerHead"></param>
        private InteractableStack MathfIsLookingAt(List<InteractableStack> interactables, Transform playerHead)
        {
            InteractableStack stack = null;
            var dot = float.MinValue;
            foreach (var interactableStack in interactables)
            {
                var position = interactableStack.TrackedPosition;
                var playerHeadPosition = playerHead.position;
                var buffer = Vector3.Dot(playerHead.forward, (position - playerHeadPosition).normalized);
                if (buffer <= 0 || buffer < dot) continue;
                stack = interactableStack;
                dot = buffer;
            }

            return stack;
        }

        /// <summary>
        /// Invoking delegate with all subscribed events to it, with passing IInteractable.
        /// </summary>
        /// <param name="interactable"></param>
        /// <param name="onInteract"></param>
        private void InvokeDelegate(IInteractable interactable, List<OnInteract> onInteract)
        {
            for (var i = 0; i < onInteract.Count; i++)
            {
                var returnState = onInteract[i].Invoke(interactable);
                if (returnState.Value)
                {
                    break;
                }
            }
        }

        internal void SetPause(bool pause)
        {
            _isPause = pause;
        }

        public void RegisterInteractable(IInteractable interactable)
        {
            _groups.AddInteractable(interactable);
        }

        public void UnregisterInteractable(IInteractable interactable)
        {
        }
    }
}