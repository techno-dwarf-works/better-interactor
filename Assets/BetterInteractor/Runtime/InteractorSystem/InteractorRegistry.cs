using System;
using System.Collections.Generic;
using System.Linq;
using Better.Interactor.Runtime.Interface;
using Better.Interactor.Runtime.Models;
using UnityEngine;

namespace Better.Interactor.Runtime
{
    /// <summary>
    /// Class responsible for interaction with objects 
    /// </summary>
    public class InteractorRegistry : MonoBehaviour
    {
        [SerializeField] private bool useMathfCalculation = true;
        private IPlayerHead _playerCamera;

        private List<OnInteract> _onInteract;
        private List<OnInteract> _onPreInteract;
        private List<OnInteract> _onPostInteract;

        private Stack<InteractableStack> _interactable = new();
        private bool _isPause;

        /// <summary>
        /// Event receive state to show and neediness to show.
        /// </summary>
        //private event CustomEventTypes.HighlightableStateDelegate HighlightInteractable;
        private delegate ReturnState OnInteract(IInteractable interactor);

        [Serializable]
        private class InteractableStack
        {
            public InteractableStack(IInteractable interactable)
            {
                Interactable = interactable;
            }

            public IInteractable Interactable { get; }
            public Vector3 TrackedPosition => Interactable.TrackedPosition;
        }

        private void Awake()
        {
            InteractorContainer.RegisterInteractorRegistry(this);
        }

        private void OnDestroy()
        {
            InteractorContainer.UnregisterInteractorRegistry(this);
        }

        internal void AssignPlayerHead(IPlayerHead player)
        {
            _playerCamera = player;
        }

        internal void RegisterInteractor(IInteractor interactor)
        {
            _onPostInteract.Add(interactor.PostInteract);
            _onInteract.Add(interactor.Interact);
            _onPreInteract.Add(interactor.PreInteract);
        }
        
        internal void UnregisterInteractor(IInteractor interactor)
        {
            _onPostInteract.Add(interactor.PostInteract);
            _onInteract.Add(interactor.Interact);
            _onPreInteract.Add(interactor.PreInteract);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<IInteractable>(out var controller)) return;
            if (_interactable.Any(i => i.Interactable.Equals(controller))) return;
            var item = new InteractableStack(controller);
            _interactable.Push(item);
            PreInteractInternal(item);
        }

        /// <summary>
        /// Internal PreInteract behaviour.
        /// </summary>
        /// <param name="item"></param>
        private void PreInteractInternal(InteractableStack item)
        {
            InvokeDelegate(item.Interactable, _onPreInteract);
        }

        private void Update()
        {
            if (_isPause) return;
            if (_interactable.Count <= 0) return;
            var item = IsLookingAt(out var stack) ? stack : _interactable.Peek();
            PreInteractInternal(item);
            //TODO: Move to New Input System
            //User calls interaction with object
            if (!Input.GetKeyDown(KeyCode.E)) return;
            InvokeDelegate(item.Interactable, _onInteract);
        }

        /// <summary>
        /// Check if user currently looks on some interactable object
        /// </summary>
        /// <param name="stack"></param>
        /// <returns></returns>
        private bool IsLookingAt(out InteractableStack stack)
        {
            stack = null;
            if (!(_interactable.Count > 1)) return false;
            var playerHead = _playerCamera == null ? transform : _playerCamera.HeadTransform;

            if (!useMathfCalculation)
            {
                RaycastIsLookingAt(ref stack, playerHead);
            }
            else
            {
                MathfIsLookingAt(ref stack, playerHead);
            }

            if (ReferenceEquals(stack, null)) return false;
            return true;
        }

        /// <summary>
        /// Mathematical calculation if user looks on some object
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="playerHead"></param>
        private void MathfIsLookingAt(ref InteractableStack stack, Transform playerHead)
        {
            var dot = float.MinValue;
            foreach (var interactableStack in _interactable)
            {
                var position = interactableStack.TrackedPosition;
                var playerHeadPosition = playerHead.position;
                var buffer = Vector3.Dot(playerHead.forward, (position - playerHeadPosition).normalized);
                if (buffer <= 0 || buffer < dot) continue;
                stack = interactableStack;
                dot = buffer;
            }
        }

        /// <summary>
        /// Raycast calculation if user looks on some object
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="playerHead"></param>
        private void RaycastIsLookingAt(ref InteractableStack stack, Transform playerHead)
        {
            var ray = new Ray(playerHead.position, playerHead.forward);
            if (!Physics.Raycast(ray, out var hit, 10, LayerMask.GetMask("Default"), QueryTriggerInteraction.Collide)) return;

            if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
                stack = _interactable.FirstOrDefault(x => x.Interactable.Equals(interactable));
        }

        /// <summary>
        /// Invoking delegate with all subscribed events to it, with passing IInteractable.
        /// </summary>
        /// <param name="interactable"></param>
        /// <param name="onInteract"></param>
        private void InvokeDelegate(IInteractable interactable, List<OnInteract> onInteract)
        {
            for (int i = 0; i < onInteract.Count; i++)
            {
                var returnState = onInteract[i].Invoke(interactable);
                if (!returnState.Value)
                {
                    continue;
                }
                
                returnState.InvokeGaze();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent<IInteractable>(out var interactable)) return;
            if (!(_interactable.Count > 0)) return;

            //Checking if list contains exited interactable 
            if (_interactable.Any(x => x.Interactable.Equals(interactable)))
            {
                //Storing for post interact
                var item = _interactable.FirstOrDefault(x => x.Interactable.Equals(interactable));
                var list = _interactable.ToList();
                list.RemoveAll(x => x.Interactable.Equals(interactable));
                _interactable = new Stack<InteractableStack>(list);
                if (item == null) return;
                InvokeDelegate(item.Interactable, _onPostInteract);
            }

            if (_interactable.Count <= 0) return;
            PreInteractInternal(_interactable.Peek());
        }

        private void OnPauseState(bool pause)
        {
            _isPause = pause;
        }
    }
}