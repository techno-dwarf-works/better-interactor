using System;
using UnityEngine;

namespace Better.Interactor.Runtime.Models
{
    [Serializable]
    public abstract class OrientedBoundingBox
    {
        public abstract Vector3 Center { get; protected set; }
        public abstract Vector3 Size { get; protected set; }
        public abstract Quaternion Orientation { get; protected set; }
        protected const float Half = 0.5f;
        private const int AxesCount = 3;

        public OrientedBoundingBox()
        {
        }

        public bool Intersects(OrientedBoundingBox other)
        {
            Matrix4x4 thisToWorld = Matrix4x4.TRS(Center, Orientation, Vector3.one);
            Matrix4x4 otherToWorld = Matrix4x4.TRS(other.Center, other.Orientation, Vector3.one);

            Matrix4x4 worldToThis = thisToWorld.inverse;
            Matrix4x4 worldToOther = otherToWorld.inverse;

            // Corners of the other OBB in this OBB's local space
            Vector3[] otherCorners = new Vector3[8];
            for (int i = 0; i < 8; i++)
            {
                Vector3 corner = otherToWorld.MultiplyPoint(other.GetCorner(i));
                otherCorners[i] = worldToThis.MultiplyPoint(corner);
            }

            // SAT check using axes of this OBB
            var axes = thisToWorld.ToAxes();
            for (int i = 0; i < 3; i++)
            {
                Vector3 axis = axes[i];

                float thisProjection = ProjectToAxis(axis, Center, Size);
                float otherProjection = ProjectToAxis(axis, other.Center, otherCorners);

                if (Mathf.Abs(Vector3.Dot(axis, other.Center - Center)) > thisProjection + otherProjection)
                {
                    return false;
                }
            }

            var otherAxes = thisToWorld.ToAxes();
            // SAT check using axes of the other OBB
            for (int i = 0; i < 3; i++)
            {
                Vector3 axis = otherAxes[i];

                float thisProjection = ProjectToAxis(axis, Center, Size);
                float otherProjection = ProjectToAxis(axis, other.Center, otherCorners);

                if (Mathf.Abs(Vector3.Dot(axis, other.Center - Center)) > thisProjection + otherProjection)
                {
                    return false;
                }
            }

            return true;
        }

        private float ProjectToAxis(Vector3 axis, Vector3 boxCenter, Vector3 boxSize)
        {
            var projection = Mathf.Abs(Vector3.Dot(boxCenter, axis));

            var extentX = Mathf.Abs(Vector3.Dot(boxSize.x * Half * axis, axis));
            var extentY = Mathf.Abs(Vector3.Dot(boxSize.y * Half * axis, axis));
            var extentZ = Mathf.Abs(Vector3.Dot(boxSize.z * Half * axis, axis));

            return projection + extentX + extentY + extentZ;
        }

        private float ProjectToAxis(Vector3 axis, Vector3 boxCenter, Vector3[] boxCorners)
        {
            float projection = Mathf.Abs(Vector3.Dot(boxCenter, axis));
            float maxExtent = float.MinValue;

            for (int i = 0; i < 8; i++)
            {
                float extent = Vector3.Dot(boxCorners[i], axis);
                if (extent > maxExtent)
                {
                    maxExtent = extent;
                }
            }

            return projection + maxExtent;
        }

        private Vector3 GetCorner(int index)
        {
            Vector3 halfSize = Size * 0.5f;
            Vector3 localCorner = new Vector3(
                (index & 1) == 0 ? -halfSize.x : halfSize.x,
                (index & 2) == 0 ? -halfSize.y : halfSize.y,
                (index & 4) == 0 ? -halfSize.z : halfSize.z
            );

            return Orientation * localCorner + Center;
        }

        public void Encapsulate(Vector3 point)
        {
            // Transform the point to local space of the OBB
            var inverseRotationMatrix = Matrix4x4.Rotate(Quaternion.Inverse(Orientation));
            var localPoint = inverseRotationMatrix.MultiplyPoint(point - Center);


            // Calculate new half extents
            var halfExtents = Size * Half;
            var axes = Orientation.ToAxes();
            for (var i = 0; i < AxesCount; i++)
            {
                var distance = Mathf.Abs(Vector3.Dot(axes[i], localPoint));
                halfExtents[i] = Mathf.Max(halfExtents[i], distance);
            }

            Size = halfExtents * 2f;
        }

        public virtual void Encapsulate(OrientedBoundingBox other)
        {
            // Transform the other OBB's center to local space of this OBB
            var inverseRotationMatrix = Matrix4x4.Rotate(Quaternion.Inverse(Orientation));
            var localCenter = inverseRotationMatrix.MultiplyPoint(other.Center - Center);

            // Calculate new half extents
            var halfExtents = Size * Half;
            var axes = Orientation.ToAxes();

            var otherAxes = other.Orientation.ToAxes();

            for (var i = 0; i < AxesCount; i++)
            {
                var axis = axes[i];
                var distance1 = Mathf.Abs(Vector3.Dot(axis, localCenter));
                var distance2 = other.Size.x * Half * Mathf.Abs(Vector3.Dot(otherAxes[0], axis))
                                + other.Size.y * Half * Mathf.Abs(Vector3.Dot(otherAxes[1], axis))
                                + other.Size.z * Half * Mathf.Abs(Vector3.Dot(otherAxes[2], axis));

                halfExtents[i] = Mathf.Max(halfExtents[i], distance1 + distance2);
            }

            Size = halfExtents * 2f;
        }

#if UNITY_EDITOR
        public void DrawGizmos()
        {
            var originalGizmoMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(Center, Orientation, Vector3.one);

            var halfSize = Size * 0.5f;

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