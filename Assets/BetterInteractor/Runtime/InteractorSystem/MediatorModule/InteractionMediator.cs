using System.Collections.Generic;
using Better.Interactor.Runtime.Interface;
using UnityEngine;

namespace Better.Interactor.Runtime.MediatorModule
{
    /// <summary>
    /// Event receive state to show and neediness to show.
    /// </summary>
    //private event CustomEventTypes.HighlightableStateDelegate HighlightInteractable;
    public delegate ReturnState OnInteract(IInteractable interactor);

    /// <summary>
    /// Class responsible for interaction with objects 
    /// </summary>
    public class InteractionMediator
    {
        private Dictionary<int, List<IInteractorContainer>> _interactorContainers = new Dictionary<int, List<IInteractorContainer>>(MaskUtility.Comparer);

        private List<OnInteract> _onInteract = new List<OnInteract>();
        private List<OnInteract> _onPreInteract = new List<OnInteract>();
        private List<OnInteract> _onPostInteract = new List<OnInteract>();

        private Groups _groups = new Groups();
        private bool _isPause;

        internal void RegisterContainer(IInteractorContainer interactorContainer)
        {
            if (!_interactorContainers.TryGetValue(interactorContainer.Mask, out var list))
            {
                list = new List<IInteractorContainer>();
                _interactorContainers.Add(interactorContainer.Mask, list);
            }

            list.Add(interactorContainer);
        }

        internal void UnregisterContainer(IInteractorContainer interactorContainer)
        {
            if (_interactorContainers.TryGetValue(interactorContainer.Mask, out var list))
            {
                list.Remove(interactorContainer);
            }
        }

        internal void RemoveInteractorContainer(IInteractorContainer interactorContainer)
        {
            if (_interactorContainers.TryGetValue(interactorContainer.Mask, out var list))
            {
                list.Remove(interactorContainer);
            }
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
        private void PreInteractInternal(StateContainer item)
        {
            var interactable = item.Interactable;
            Debug.Log($"{nameof(PreInteractInternal)}: {interactable.transform.name}", interactable.transform);
            item.SetCurrentState(InteractionState.Interact);
            InvokeDelegate(interactable, _onPreInteract);
        }

        /// <summary>
        /// Internal PreInteract behaviour.
        /// </summary>
        /// <param name="item"></param>
        private void PostInteractInternal(StateContainer item)
        {
            var interactable = item.Interactable;
            Debug.Log($"{nameof(PostInteractInternal)}: {interactable.transform.name}", interactable.transform);
            item.SetCurrentState(InteractionState.None);
            InvokeDelegate(interactable, _onPostInteract);
        }

        /// <summary>
        /// Internal PreInteract behaviour.
        /// </summary>
        /// <param name="item"></param>
        private void InteractInternal(StateContainer item)
        {
            var interactable = item.Interactable;
            Debug.Log($"{nameof(InteractInternal)}: {interactable.transform.name}", interactable.transform);
            item.SetCurrentState(InteractionState.PostInteract);
            InvokeDelegate(interactable, _onInteract);
        }

        internal void Update()
        {
            if (_isPause) return;
            if (_groups.Count <= 0) return;
            if (_interactorContainers.Count <= 0) return;

            foreach (var (mask, value) in _interactorContainers)
            {
                foreach (var interactorContainer in value)
                {
                    //TODO: Add layer check before getting group. Split Items by layers. Layers are matrix and IPlayerContainer can Interact only with same layer IInteractable or overlap in mask
                    var info = _groups.GetIntersecting(mask, interactorContainer.Bounds);

                    var inRangeStacks = info.GetInRangeStacks(InteractionState.PreInteract);
                    Debug.Log($"{nameof(inRangeStacks)}: {inRangeStacks.Count}");
                    var item = FindLookingAt(inRangeStacks, interactorContainer);

                    ProcessItem(item, info);

                    var outRangeStacks = info.GetOutRangeStacks(InteractionState.PostInteract);
                    Debug.Log($"{nameof(outRangeStacks)}: {outRangeStacks.Count}");
                    foreach (var interactableStack in outRangeStacks)
                    {
                        PostInteractInternal(interactableStack);
                    }
                }
            }
        }

        private void ProcessItem(StateContainer item, StacksGroup info)
        {
            if (item != null)
            {
                item.Interactable.InvokeGaze();

                if (item.CurrenctState != InteractionState.Interact)
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
        }

        /// <summary>
        /// Check if user currently looks on some interactable object
        /// </summary>
        /// <param name="interactables"></param>
        /// <param name="interactorContainer"></param>
        /// <returns></returns>
        private StateContainer FindLookingAt(List<StateContainer> interactables, IInteractorContainer interactorContainer)
        {
            if (interactables == null || interactables.Count <= 0) return null;

            var stack = MathfIsLookingAt(interactables, interactorContainer);

            if (ReferenceEquals(stack, null)) return null;
            return stack;
        }

        /// <summary>
        /// Mathematical calculation if user looks on some object
        /// </summary>
        /// <param name="interactables"></param>
        /// <param name="interactorContainer"></param>
        private StateContainer MathfIsLookingAt(List<StateContainer> interactables, IInteractorContainer interactorContainer)
        {
            StateContainer container = null;
            var playerHead = interactorContainer.transform;
            var playerHeadForward = playerHead.forward;
            var distance = float.MaxValue;
            foreach (var interactableStack in interactables)
            {
                if (!interactableStack.Intersects(interactorContainer.Bounds)) continue;

                var position = interactorContainer.Bounds.GetWorldCenter();
                if (!interactableStack.Raycast(new Ray(position, playerHeadForward), out var hitPoint)) continue;

                var hitVector = position - hitPoint;
                if (Vector3.SqrMagnitude(hitVector) > distance) continue;
                container = interactableStack;
                distance = hitVector.sqrMagnitude;
            }

            return container;
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
            _groups.RemoveInteractable(interactable);
        }
    }
}