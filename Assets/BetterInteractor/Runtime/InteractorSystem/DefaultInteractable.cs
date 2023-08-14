using System;
using Better.Interactor.Runtime.Models;
using Better.Interactor.Runtime.Test;
using UnityEngine;

namespace Better.Interactor.Runtime
{
    public class DefaultInteractable : Interactable
    {
        [SerializeField] private TransformOBB boundingBox;

        private void Awake()
        {
            boundingBox.SetTransform(transform);
        }

        public override void InvokeGaze()
        {
        }

#if UNITY_EDITOR 
        private void OnDrawGizmos()
        {
            boundingBox.SetTransform(transform);
            boundingBox.DrawGizmos();
        }
#endif

        public override OrientedBoundingBox GetBounds()
        {
            return boundingBox;
        }
    }
}