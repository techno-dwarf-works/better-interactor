using Better.Interactor.Runtime.Attributes;
using Better.Interactor.Runtime.BoundingBox;
using UnityEngine;

namespace Better.Interactor.Runtime.Interactables
{
    public class DefaultInteractable : Interactable
    {
        [SerializeField] private TransformOBB boundingBox;
        [Mask]
        [SerializeField] private int mask;

        public override OrientedBoundingBox Bounds => boundingBox;

        public override int Mask => mask;

        private void Awake()
        {
            boundingBox.SetTransform(transform);
        }

        public override void InvokeGaze()
        {
        }

        private void OnValidate()
        {
            boundingBox.SetTransform(transform);
        }
    }
}