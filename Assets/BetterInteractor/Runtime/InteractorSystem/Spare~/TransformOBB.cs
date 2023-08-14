using System;
using UnityEngine;

namespace Better.Interactor.Runtime.Models
{
    [Serializable]
    public class TransformOBB : OrientedBoundingBox
    {
        [SerializeField] private Transform transform;
        [SerializeField] private Vector3 size;

        public override Vector3 Center
        {
            get => transform.position;
            protected set => transform.position = value;
        }

        public override Vector3 Size
        {
            get => size;
            protected set => size = value;
        }

        public override Quaternion Orientation
        {
            get => transform.rotation;
            protected set => transform.rotation = value;
        }

        public void SetTransform(Transform transform)
        {
            this.transform = transform;
        }
    }
}