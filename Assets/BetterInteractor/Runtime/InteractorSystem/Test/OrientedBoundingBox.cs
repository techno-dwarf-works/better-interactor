using System;
using System.Collections.Generic;
using Better.Interactor.Runtime.Models;
using UnityEngine;

namespace Better.Interactor.Runtime.Test
{
    public abstract class OrientedBoundingBox
    {
        protected const float Half = 0.5f;
        private const int AxesCount = 3;
        protected const int CornersCount = 8;
        
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
        
        public Vector3 GetClosestPointOnBounds(Vector3 point)
        {
            var localPoint = Transforms.inverse.MultiplyPoint3x4(point - LocalCenter);
            var halfExtents = LocalExtents;

            var closestLocalPoint = localPoint;

            for (int i = 0; i < AxesCount; i++)
            {
                closestLocalPoint[i] = Mathf.Clamp(closestLocalPoint[i], -halfExtents[i], halfExtents[i]);
            }

            var closestPoint = Transforms.MultiplyPoint3x4(closestLocalPoint);

            return closestPoint;
        }

        public bool Intersects(OrientedBoundingBox other)
        {
            var myCorners = new Vector3[CornersCount];
            GetWorldBoxCornersNonAlloc(myCorners);
            var otherCorners = new Vector3[CornersCount];
            other.GetWorldBoxCornersNonAlloc(otherCorners);

            var axes = Transforms.rotation.ToAxes();
            var otherAxes = other.Transforms.rotation.ToAxes();

            // Check for separation along my axes
            for (var i = 0; i < AxesCount; i++)
            {
                if (!AxisTest(myCorners, otherCorners, axes[i]))
                {
                    return false; // No intersection on this axis
                }
            }

            // Check for separation along other's axes
            for (var i = 0; i < AxesCount; i++)
            {
                if (!AxisTest(myCorners, otherCorners, otherAxes[i]))
                {
                    return false; // No intersection on this axis
                }
            }

            // Check for cross products of pairs of edges
            for (var i = 0; i < AxesCount; i++)
            {
                for (var j = 0; j < AxesCount; j++)
                {
                    var axis = Vector3.Cross(axes[i], otherAxes[j]);
                    if (!AxisTest(myCorners, otherCorners, axis))
                    {
                        return false; // No intersection on this axis
                    }
                }
            }

            return true; // Intersection detected on all axes
        }

        private bool AxisTest(IReadOnlyList<Vector3> corners1, IReadOnlyList<Vector3> corners2, Vector3 axis)
        {
            var min1 = float.MaxValue;
            var max1 = float.MinValue;
            var min2 = float.MaxValue;
            var max2 = float.MinValue;

            for (var i = 0; i < CornersCount; i++)
            {
                var projection1 = Vector3.Dot(corners1[i], axis);
                var projection2 = Vector3.Dot(corners2[i], axis);

                min1 = Mathf.Min(min1, projection1);
                max1 = Mathf.Max(max1, projection1);
                min2 = Mathf.Min(min2, projection2);
                max2 = Mathf.Max(max2, projection2);
            }

            var intervalDistance = Mathf.Max(min1, min2) - Mathf.Min(max1, max2);
            return intervalDistance <= 0;
        }

        public void GetLocalBoxCornersNonAlloc(Vector3[] corners)
        {
            var halfSize = LocalExtents;

            for (var i = 0; i < CornersCount; i++)
            {
                corners[i] = new Vector3(
                    (i & 1) == 0 ? -halfSize.x : halfSize.x,
                    (i & 2) == 0 ? -halfSize.y : halfSize.y,
                    (i & 4) == 0 ? -halfSize.z : halfSize.z
                );
            }
        }

        public void GetWorldBoxCornersNonAlloc(Vector3[] corners)
        {
            GetLocalBoxCornersNonAlloc(corners);

            for (var i = 0; i < CornersCount; i++)
            {
                corners[i] = Transforms.MultiplyPoint3x4(corners[i] + LocalCenter);
            }
        }

        public List<Vector3> GetIntersectionPoints(OrientedBoundingBox other)
        {
            var intersectionPoints = new List<Vector3>();

            var myCorners = new Vector3[CornersCount];
            var otherCorners = new Vector3[CornersCount];
            GetWorldBoxCornersNonAlloc(myCorners);
            other.GetWorldBoxCornersNonAlloc(otherCorners);

            // Check for intersection on each edge of my box
            for (var i = 0; i < myCorners.Length; i++)
            {
                var point = myCorners[i];

                if (other.ContainsPoint(point))
                {
                    intersectionPoints.Add(point);
                }
            }

            // Check for intersection on each edge of other box
            for (var i = 0; i < otherCorners.Length; i++)
            {
                var point = otherCorners[i];

                if (ContainsPoint(point))
                {
                    intersectionPoints.Add(point);
                }
            }

            return intersectionPoints;
        }

        private bool ContainsPoint(Vector3 point)
        {
            var localPoint = Transforms.inverse.MultiplyPoint3x4(point - LocalCenter);
            var halfExtents = LocalExtents;

            return Mathf.Abs(localPoint.x) <= halfExtents.x &&
                   Mathf.Abs(localPoint.y) <= halfExtents.y &&
                   Mathf.Abs(localPoint.z) <= halfExtents.z;
        }
    }
}