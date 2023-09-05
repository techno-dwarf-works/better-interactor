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
            Debug.Log($"{nameof(PreInteractInternal)}: {item.Interactable.transform.name}", item.Interactable.transform);
            item.SetCurrentState(InteractionState.Interact);
            InvokeDelegate(item.Interactable, _onPreInteract);
        }

        /// <summary>
        /// Internal PreInteract behaviour.
        /// </summary>
        /// <param name="item"></param>
        private void PostInteractInternal(InteractableStack item)
        {
            Debug.Log($"{nameof(PostInteractInternal)}: {item.Interactable.transform.name}", item.Interactable.transform);
            item.SetCurrentState(InteractionState.None);
            InvokeDelegate(item.Interactable, _onPostInteract);
        }

        /// <summary>
        /// Internal PreInteract behaviour.
        /// </summary>
        /// <param name="item"></param>
        private void InteractInternal(InteractableStack item)
        {
            Debug.Log($"{nameof(InteractInternal)}: {item.Interactable.transform.name}", item.Interactable.transform);
            item.SetCurrentState(InteractionState.PostInteract);
            InvokeDelegate(item.Interactable, _onInteract);
        }

        private void Update()
        {
            if (_isPause) return;
            if (_groups.Count <= 0) return;
            if (_playerContainer == null) return;
            var info = _groups.GetIntersecting(_playerContainer.Bounds);

            _groups.TrackBoxes();
            var inRangeStacks = info.GetInRangeStacks(InteractionState.PreInteract);
            Debug.Log($"{nameof(inRangeStacks)}: {inRangeStacks.Count}");
            var item = FindLookingAt(inRangeStacks);

            if (item != null)
            {
                item.Interactable.InvokeGaze();

                if (item.CurrentState != InteractionState.Interact)
                {
                    PreInteractInternal(item);
                }

                //TODO: Move to New Input System
                //User calls interaction with object
                if (Input.GetKeyDown(KeyCode.E))
                {
                    InteractInternal(item);
                }
            }
            else
            {
                var rangeStacks = info.GetInRangeStacks(InteractionState.PreInteract);
                foreach (var interactableStack in rangeStacks)
                {
                    interactableStack.SetCurrentState(InteractionState.PreInteract);
                }
            }

            var outRangeStacks = info.GetOutRangeStacks(InteractionState.PostInteract);
            Debug.Log($"{nameof(outRangeStacks)}: {outRangeStacks.Count}");
            foreach (var interactableStack in outRangeStacks)
            {
                PostInteractInternal(interactableStack);
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

            var stack = MathfIsLookingAt(interactables, _playerContainer);

            if (ReferenceEquals(stack, null)) return null;
            return stack;
        }

        /// <summary>
        /// Mathematical calculation if user looks on some object
        /// </summary>
        /// <param name="interactables"></param>
        /// <param name="playerContainer"></param>
        private InteractableStack MathfIsLookingAt(List<InteractableStack> interactables, IPlayerContainer playerContainer)
        {
            InteractableStack stack = null;
            var playerHead = playerContainer.transform;
            var playerHeadForward = playerHead.forward;
            var distance = float.MaxValue;
            foreach (var interactableStack in interactables)
            {
                if (!interactableStack.Intersects(playerContainer.Bounds)) continue;

                var position = playerContainer.Bounds.GetWorldCenter();
                if (!interactableStack.Raycast(new Ray(position, playerHeadForward), out var hitPoint)) continue;

                var hitVector = position - hitPoint;
                if (Vector3.SqrMagnitude(hitVector) > distance) continue;
                stack = interactableStack;
                distance = hitVector.sqrMagnitude;
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