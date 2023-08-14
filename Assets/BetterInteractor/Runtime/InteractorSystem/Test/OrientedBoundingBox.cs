using System;
using Better.Interactor.Runtime.Models;
using UnityEngine;

namespace Better.Interactor.Runtime.Test
{
    public abstract class OrientedBoundingBox
    {
        public OrientedBoundingBox()
        {
        }

        public abstract Vector3 LocalCenter { get; protected set; }

        public abstract Vector3 LocalExtents { get; protected set; }

        public abstract Matrix4x4 Transforms { get; protected set; }


        public Vector3 GetWorldCenter()
        {
            return Transforms.MultiplyPoint3x4(LocalCenter);
        }

        protected const float Half = 0.5f;
        private const int AxesCount = 3;

        //======================================================================

        public void Encapsulate(Vector3 point)
        {
            // Transform the point to local space of the OBB
            var inverseRotationMatrix = Matrix4x4.Inverse(Transforms);
            var localPoint = inverseRotationMatrix.MultiplyPoint(point - LocalCenter);


            // Calculate new half extents
            var halfExtents = LocalExtents;
            var axes = Transforms.ToAxes();
            for (var i = 0; i < AxesCount; i++)
            {
                var distance = Mathf.Abs(Vector3.Dot(axes[i], localPoint));
                halfExtents[i] = Mathf.Max(halfExtents[i], distance);
            }

            LocalExtents = halfExtents * 2f;
        }

        public virtual void Encapsulate(OrientedBoundingBox other)
        {
            // Transform the other OBB's center to local space of this OBB
            var inverseRotationMatrix = Matrix4x4.Inverse(Transforms);
            var localCenter = inverseRotationMatrix.MultiplyPoint(other.LocalCenter - LocalCenter);

            // Calculate new half extents
            var halfExtents = LocalExtents;
            var axes = Transforms.ToAxes();

            var otherAxes = other.Transforms.ToAxes();

            for (var i = 0; i < AxesCount; i++)
            {
                var axis = axes[i];
                var distance1 = Mathf.Abs(Vector3.Dot(axis, localCenter));
                var otherExtents = other.LocalExtents;
                var distance2 = otherExtents.x * Mathf.Abs(Vector3.Dot(otherAxes[0], axis))
                                + otherExtents.y * Mathf.Abs(Vector3.Dot(otherAxes[1], axis))
                                + otherExtents.z * Mathf.Abs(Vector3.Dot(otherAxes[2], axis));

                halfExtents[i] = Mathf.Max(halfExtents[i], distance1 + distance2);
            }

            LocalExtents = halfExtents * 2f;
        }

        public bool Intersects(OrientedBoundingBox other)
        {
            var myCorners = GetWorldBoxCorners();
            var otherCorners = other.GetWorldBoxCorners();

            var axes = Transforms.rotation.ToAxes();
            var otherAxes = other.Transforms.rotation.ToAxes();

            // Check for separation along my axes
            for (int i = 0; i < 3; i++)
            {
                if (!AxisTest(myCorners, otherCorners, axes[i]))
                {
                    return false; // No intersection on this axis
                }
            }

            // Check for separation along other's axes
            for (int i = 0; i < 3; i++)
            {
                if (!AxisTest(myCorners, otherCorners, otherAxes[i]))
                {
                    return false; // No intersection on this axis
                }
            }

            // Check for cross products of pairs of edges
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vector3 axis = Vector3.Cross(axes[i], otherAxes[j]);
                    if (!AxisTest(myCorners, otherCorners, axis))
                    {
                        return false; // No intersection on this axis
                    }
                }
            }

            return true; // Intersection detected on all axes
        }

        private bool AxisTest(Vector3[] corners1, Vector3[] corners2, Vector3 axis)
        {
            float min1 = float.MaxValue;
            float max1 = float.MinValue;
            float min2 = float.MaxValue;
            float max2 = float.MinValue;

            for (int i = 0; i < 8; i++)
            {
                float projection1 = Vector3.Dot(corners1[i], axis);
                float projection2 = Vector3.Dot(corners2[i], axis);

                min1 = Mathf.Min(min1, projection1);
                max1 = Mathf.Max(max1, projection1);
                min2 = Mathf.Min(min2, projection2);
                max2 = Mathf.Max(max2, projection2);
            }

            float intervalDistance = Mathf.Max(min1, min2) - Mathf.Min(max1, max2);
            return intervalDistance <= 0;
        }


        public Vector3[] GetBoxCorners()
        {
            var corners = new Vector3[8];
            var halfSize = LocalExtents;

            for (var i = 0; i < 8; i++)
            {
                corners[i] = new Vector3(
                    (i & 1) == 0 ? -halfSize.x : halfSize.x,
                    (i & 2) == 0 ? -halfSize.y : halfSize.y,
                    (i & 4) == 0 ? -halfSize.z : halfSize.z
                );
            }

            return corners;
        }

        public Vector3[] GetWorldBoxCorners()
        {
            Vector3[] corners = new Vector3[8];
            Vector3 halfSize = LocalExtents;

            for (int i = 0; i < 8; i++)
            {
                corners[i] = new Vector3(
                    (i & 1) == 0 ? -halfSize.x : halfSize.x,
                    (i & 2) == 0 ? -halfSize.y : halfSize.y,
                    (i & 4) == 0 ? -halfSize.z : halfSize.z
                );
            }

            for (int i = 0; i < 8; i++)
            {
                corners[i] = Transforms.MultiplyPoint3x4(corners[i] + LocalCenter);
            }

            return corners;
        }

#if UNITY_EDITOR
        public void DrawGizmos()
        {
            var originalGizmoMatrix = Gizmos.matrix;
            Gizmos.matrix = Transforms;

            var halfSize = LocalExtents;

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