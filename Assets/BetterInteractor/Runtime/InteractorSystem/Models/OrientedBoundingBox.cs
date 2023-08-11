using System;
using UnityEngine;

namespace Better.Interactor.Runtime.Models
{
    [Serializable]
    public class OrientedBoundingBox
    {
        [SerializeField] private Vector3 center;
        [SerializeField] private Vector3 size;
        [SerializeField] private Quaternion orientation;
        private const float Half = 0.5f;
        private const int AxesCount = 3;
        
        public Vector3 Center => center;
        public Vector3 Size => size;
        public Quaternion Orientation => orientation;

        public OrientedBoundingBox(Vector3 center, Vector3 size, Quaternion orientation)
        {
            this.center = center;
            this.size = size;
            this.orientation = orientation;
        } 
        
        public OrientedBoundingBox()
        {
            this.center = Vector3.zero;
            this.size = Vector3.zero;
            this.orientation = Quaternion.identity;
        }

        public bool Intersects(OrientedBoundingBox other)
        {
            // Convert quaternions to rotation matrices
            var thisRotationMatrix = Matrix4x4.Rotate(orientation);
            var otherRotationMatrix = Matrix4x4.Rotate(other.orientation);

            // Transform centers to world space
            var thisWorldCenter = thisRotationMatrix.MultiplyPoint(center);
            var otherWorldCenter = otherRotationMatrix.MultiplyPoint(other.center);

            // Calculate axes in world space
            var thisWorldAxes = thisRotationMatrix.ToAxes();

            var otherWorldAxes = otherRotationMatrix.ToAxes();

            // Separate Axis Theorem (SAT) check
            for (var i = 0; i < AxesCount; i++)
            {
                for (var j = 0; j < AxesCount; j++)
                {
                    var axis = Vector3.Cross(thisWorldAxes[i], otherWorldAxes[j]);
                    var projection1 = ProjectToAxis(axis, thisWorldCenter, size);
                    var projection2 = ProjectToAxis(axis, otherWorldCenter, other.size);

                    var distance = Mathf.Abs(Vector3.Dot(axis, thisWorldCenter - otherWorldCenter));

                    if (distance > projection1 + projection2)
                    {
                        return false; // No intersection on this axis
                    }
                }
            }

            return true; // Intersection detected on all axes
        }

        public void Encapsulate(Vector3 point)
        {
            // Transform the point to local space of the OBB
            var inverseRotationMatrix = Matrix4x4.Rotate(Quaternion.Inverse(orientation));
            var localPoint = inverseRotationMatrix.MultiplyPoint(point - center);


            // Calculate new half extents
            var halfExtents = size * Half;
            var axes = orientation.ToAxes();
            for (var i = 0; i < AxesCount; i++)
            {
                var distance = Mathf.Abs(Vector3.Dot(axes[i], localPoint));
                halfExtents[i] = Mathf.Max(halfExtents[i], distance);
            }

            size = halfExtents * 2f;
        }

        //TODO: Fix, not working properly
        public void Encapsulate(OrientedBoundingBox other)
        {
            // Transform the other OBB's center to local space of this OBB
            var inverseRotationMatrix = Matrix4x4.Rotate(Quaternion.Inverse(orientation));
            var localCenter = inverseRotationMatrix.MultiplyPoint(other.center - center);

            // Calculate new half extents
            var halfExtents = size * Half;
            var axes = orientation.ToAxes();

            var otherAxes = other.orientation.ToAxes();

            for (var i = 0; i < AxesCount; i++)
            {
                var axis = axes[i];
                var distance1 = Mathf.Abs(Vector3.Dot(axis, localCenter));
                var distance2 = other.size.x * Half * Mathf.Abs(Vector3.Dot(otherAxes[0], axis))
                                + other.size.y * Half * Mathf.Abs(Vector3.Dot(otherAxes[1], axis))
                                + other.size.z * Half * Mathf.Abs(Vector3.Dot(otherAxes[2], axis));

                halfExtents[i] = Mathf.Max(halfExtents[i], distance1 + distance2);
            }

            size = halfExtents * 2f;
        }

        private float ProjectToAxis(Vector3 axis, Vector3 boxCenter, Vector3 boxSize)
        {
            var projection = Mathf.Abs(Vector3.Dot(boxCenter, axis));

            var extentX = Mathf.Abs(Vector3.Dot(boxSize.x * Half * axis, axis));
            var extentY = Mathf.Abs(Vector3.Dot(boxSize.y * Half * axis, axis));
            var extentZ = Mathf.Abs(Vector3.Dot(boxSize.z * Half * axis, axis));

            return projection + extentX + extentY + extentZ;
        }

#if UNITY_EDITOR
        public void DrawGizmos()
        {
            var originalGizmoMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(center, orientation, Vector3.one);

            var halfSize = size * 0.5f;

            Gizmos.color = Color.yellow;

            // Draw the wireframe box using Gizmos.DrawLine
            var p0 = new Vector3(-halfSize.x, -halfSize.y, -halfSize.z);
            var p1 = new Vector3(-halfSize.x, -halfSize.y, halfSize.z);
            var p2 = new Vector3(halfSize.x, -halfSize.y, halfSize.z);
            var p3 = new Vector3(halfSize.x, -halfSize.y, -halfSize.z);
            var p4 = new Vector3(-halfSize.x, halfSize.y, -halfSize.z);
            var p5 = new Vector3(-halfSize.x, halfSize.y, halfSize.z);
            var p6 = new Vector3(halfSize.x, halfSize.y, halfSize.z);
            var p7 = new Vector3(halfSize.x, halfSize.y, -halfSize.z);

            Gizmos.DrawLine(p0, p1);
            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p0);

            Gizmos.DrawLine(p4, p5);
            Gizmos.DrawLine(p5, p6);
            Gizmos.DrawLine(p6, p7);
            Gizmos.DrawLine(p7, p4);

            Gizmos.DrawLine(p0, p4);
            Gizmos.DrawLine(p1, p5);
            Gizmos.DrawLine(p2, p6);
            Gizmos.DrawLine(p3, p7);

            Gizmos.matrix = originalGizmoMatrix;
        }

#endif
    }
}