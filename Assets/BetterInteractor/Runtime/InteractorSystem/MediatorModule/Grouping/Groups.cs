using System.Collections.Generic;
using System.Linq;
using Better.Interactor.Runtime.BoundingBox;
using Better.Interactor.Runtime.Interface;

namespace Better.Interactor.Runtime.MediatorModule
{
    public class Groups
    {
        private static StacksGroup _interactableStacks = new StacksGroup();
        private readonly Dictionary<int, List<Group>> _groups = new Dictionary<int, List<Group>>(MaskUtility.Comparer);
        private readonly float _sqrDistance;

        public Groups()
        {
            _sqrDistance = 100f;
        }
        
        public int Count => _groups.Count;

        public List<Group> GetGroups()
        {
            return _groups.Values.SelectMany(x => x).ToList();
        }

        public StacksGroup GetIntersecting(int mask, OrientedBoundingBox bounds)
        {
            _interactableStacks.Clear();
            if (!_groups.TryGetValue(mask, out var groups)) return _interactableStacks;
            foreach (var group in groups)
            {
                if (!group.Intersects(bounds)) continue;
                foreach (var stack in group)
                {
                    if (stack.Intersects(bounds))
                    {
                        _interactableStacks.AddInRange(stack);
                    }
                    else
                    {
                        _interactableStacks.AddOutRange(stack);
                    }
                }
            }

            return _interactableStacks;
        }

        public void AddInteractable(IInteractable interactable)
        {
            Group selectedGroup = null;

            var mask = interactable.Mask;
            if (!_groups.TryGetValue(mask, out var groups))
            {
                groups = new List<Group>();
                _groups.Add(mask, groups);
            }

            foreach (var group in groups)
            {
                var currentDistance = group.SqrDistanceTo(interactable.Bounds);
                if (!(currentDistance <= _sqrDistance)) continue;
                selectedGroup = group;
            }

            if (selectedGroup == null)
            {
                selectedGroup = new Group(mask);
                groups.Add(selectedGroup);
            }

            selectedGroup.Add(interactable);
        }
        

        public void RemoveInteractable(IInteractable interactable)
        {
            
        }
    }
}