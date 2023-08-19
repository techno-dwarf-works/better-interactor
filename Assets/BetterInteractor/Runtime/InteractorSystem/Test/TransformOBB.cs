using System;
using UnityEngine;

namespace Better.Interactor.Runtime.Test
{
    [Serializable]
    public class TransformOBB : OrientedBoundingBox
    {
        [SerializeField] private Transform transform;
        [SerializeField] private Vector3 offset;
        [SerializeField] private Vector3 size;

        public override Vector3 LocalCenter
        {
            get => offset;
            protected set => offset = value;
        }

        public override Vector3 LocalExtents
        {
            get => size / 2f;
            protected set => size = value * 2f;
        }

        public override Matrix4x4 Transforms
        {
            get => Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            protected set { }
        }

        public void SetTransform(Transform transform)
        {
            this.transform = transform;
        }
    }
}