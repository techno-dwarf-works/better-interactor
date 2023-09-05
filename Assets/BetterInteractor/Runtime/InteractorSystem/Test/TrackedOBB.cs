using System;
using System.Collections.Generic;
using UnityEngine;

namespace Better.Interactor.Runtime.Test
{
    [Serializable]
    public class TrackedOBB : OrientedBoundingBox
    {
        [SerializeReference] private List<OrientedBoundingBox> boxes = new List<OrientedBoundingBox>();
        private Vector3 _extents;
        private Matrix4x4 _transforms = Matrix4x4.identity;

        public override Vector3 LocalCenter
        {
            get => Vector3.zero;
            protected set { }
        }

        public override Vector3 LocalExtents
        {
            get => _extents;
            protected set => _extents = value;
        }

        public override Matrix4x4 Transforms
        {
            get => _transforms;
            protected set => _transforms = value;
        }

        public void TrackBoxes()
        {
            var minPoint = Vector3.positiveInfinity;
            var maxPoint = Vector3.negativeInfinity;

            var corners = new Vector3[CornersCount];
            for (var i = 0; i < boxes.Count; i++)
            {
                // Transform the box corners to world space
                boxes[i].GetWorldBoxCornersNonAlloc(corners);
                for (var j = 0; j < CornersCount; j++)
                {
                    var cornerWorld = corners[j];
                    minPoint = Vector3.Min(minPoint, cornerWorld);
                    maxPoint = Vector3.Max(maxPoint, cornerWorld);
                }
            }

            // Calculate the new center and size of the tracking OBB
            var center = (minPoint + maxPoint) * Half;
            Transforms = Matrix4x4.TRS(center, Quaternion.identity, Vector3.one);
            LocalExtents = (maxPoint - minPoint) * Half;
        }

        public override void Encapsulate(OrientedBoundingBox other)
        {
            if (!boxes.Contains(other))
            {
                boxes.Add(other);
            }
        }
    }
}