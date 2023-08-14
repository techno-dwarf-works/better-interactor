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
            get { return Vector3.zero; }
            protected set { }
        }

        public override Vector3 LocalExtents
        {
            get { return _extents; }
            protected set => _extents = value;
        }

        public override Matrix4x4 Transforms
        {
            get { return _transforms; }
            protected set => _transforms = value;
        }

        public void TrackBoxes()
        {
            // Calculate the combined bounds of the tracked boxes
            var minPoint = Vector3.zero;
            var maxPoint = Vector3.zero;

            for (var i = 0; i < boxes.Count; i++)
            {
                // Transform the box corners to world space
                var corners = boxes[i].GetBoxCorners();
                for (var j = 0; j < corners.Length; j++)
                {
                    var box = boxes[i];
                    var cornerWorld = box.Transforms.rotation * corners[j] + box.GetWorldCenter();
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
                TrackBoxes();
            }
        }
    }
}