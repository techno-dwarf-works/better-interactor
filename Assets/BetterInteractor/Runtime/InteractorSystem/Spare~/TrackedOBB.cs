using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Better.Interactor.Runtime.Models
{
    [Serializable]
    public class TrackedOBB : OrientedBoundingBox
    {
        [SerializeReference] private List<OrientedBoundingBox> boxes = new List<OrientedBoundingBox>();
        private Vector3 _center;
        private Vector3 _size;
        private Quaternion _orientation = Quaternion.identity;

        public override Vector3 Center
        {
            get
            {
                Update();
                return _center;
            }
            protected set => _center = value;
        }

        public override Vector3 Size
        {
            get
            {
                Update();
                return _size;
            }
            protected set => _size = value;
        }

        public override Quaternion Orientation
        {
            get
            {
                Update();
                return _orientation;
            }
            protected set => _orientation = value;
        }

        private void Update()
        {
            // Calculate the combined bounds of the tracked boxes
            if (boxes.Count <= 0) return;
            var box = boxes[0];
            var center = box.Center;
            var size = box.Size;
            var minPoint = center - size * Half;
            var maxPoint = center + size * Half;

            if (boxes.Count > 1)
            {
                for (int i = 1; i < boxes.Count; i++)
                {
                    var boundingBox = boxes[i];
                    var boxCenter = boundingBox.Center;
                    var boxSize = boundingBox.Size;
                    var boxMin = boxCenter - boxSize * Half;
                    var boxMax = boxCenter + boxSize * Half;

                    minPoint = Vector3.Min(minPoint, boxMin);
                    maxPoint = Vector3.Max(maxPoint, boxMax);
                }
            }

            // Calculate the new center and size of the tracking OBB
            _center = (minPoint + maxPoint) * Half;
            _size = maxPoint - minPoint;
            _orientation = Quaternion.identity;
        }

        public override void Encapsulate(OrientedBoundingBox other)
        {
            if (!boxes.Contains(other))
            {
                boxes.Add(other);
                Update();
            }
        }
    }
}