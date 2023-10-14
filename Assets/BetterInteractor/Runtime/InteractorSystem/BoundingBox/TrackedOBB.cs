using System;
using System.Collections.Generic;
using UnityEngine;

namespace Better.Interactor.Runtime.BoundingBox
{
    [Serializable]
    public class TrackedOBB : OrientedBoundingBox
    {
        // Define a node structure for the AABB tree
        private class TreeNode
        {
            private OrientedBoundingBox _box;

            public TreeNode(OrientedBoundingBox box, Vector3 min, Vector3 max)
            {
                _box = box;
                Min = min;
                Max = max;
            }

            public Vector3 Min { get; }
            public Vector3 Max { get; }
            public List<TreeNode> Children { get; } = new();

            public bool ContainsPoint(Vector3 point)
            {
                return _box.ContainsPoint(point);
            }

            public void GetWorldBoxCorners(Vector3[] corners)
            {
                _box.GetWorldBoxCornersNonAlloc(corners);
            }
        }

        private List<OrientedBoundingBox> boxInfos = new List<OrientedBoundingBox>();

        private TreeNode root; // The root of the AABB tree
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

        private void BuildAABBTree()
        {
            if (boxInfos.Count == 0)
            {
                root = null;
                return;
            }

            // Create an AABB tree from the stored boxes
            var aabbTree = new List<TreeNode>();
            foreach (var boxInfo in boxInfos)
            {
                var min = boxInfo.GetWorldCenter() - boxInfo.LocalExtents;
                var max = boxInfo.GetWorldCenter() + boxInfo.LocalExtents;
                aabbTree.Add(new TreeNode(boxInfo, min, max));
            }

            // Build the AABB tree recursively
            root = BuildAABBTreeRecursive(aabbTree, 0, aabbTree.Count);
        }

        private TreeNode BuildAABBTreeRecursive(List<TreeNode> aabbTree, int start, int end)
        {
            if (start == end)
                return null;

            // Find the axis with the maximum extent
            var splitAxis = 0;
            var maxExtent = float.MinValue;
            for (var axis = 0; axis < OBBUtility.AxesCount; axis++)
            {
                var axisMin = float.MaxValue;
                var axisMax = float.MinValue;

                for (var i = start; i < end; i++)
                {
                    axisMin = Mathf.Min(axisMin, aabbTree[i].Min[axis]);
                    axisMax = Mathf.Max(axisMax, aabbTree[i].Max[axis]);
                }

                var axisExtent = axisMax - axisMin;
                if (axisExtent > maxExtent)
                {
                    maxExtent = axisExtent;
                    splitAxis = axis;
                }
            }

            // Sort the boxes along the selected axis
            aabbTree.Sort((a, b) => a.Min[splitAxis].CompareTo(b.Min[splitAxis]));

            var mid = (start + end) / 2;
            var node = aabbTree[mid];

            // Recursively build left and right subtrees
            var buildBegin = BuildAABBTreeRecursive(aabbTree, start, mid);
            if (buildBegin != null)
            {
                node.Children.Add(buildBegin);
            }

            var buildEnd = BuildAABBTreeRecursive(aabbTree, mid + 1, end);
            if (buildEnd != null)
            {
                node.Children.Add(buildEnd);
            }

            // Calculate the new _extents and _transforms
            if (start == 0 && end == aabbTree.Count)
            {
                // This is the root node; update _extents and _transforms
                var minPoint = Vector3.positiveInfinity;
                var maxPoint = Vector3.negativeInfinity;
                
                var corners = new Vector3[OBBUtility.CornersCount];
                
                node.GetWorldBoxCorners(corners);
                OBBUtility.GetMinMax(corners, ref minPoint, ref maxPoint);
                
                foreach (var child in node.Children)
                {
                    child.GetWorldBoxCorners(corners);
                    OBBUtility.GetMinMax(corners, ref minPoint, ref maxPoint);
                }

                var center = (minPoint + maxPoint) * OBBUtility.Half;
                _transforms = Matrix4x4.TRS(center, Quaternion.identity, Vector3.one);
                _extents = (maxPoint - minPoint) * OBBUtility.Half;
            }

            return node;
        }


        public override void Encapsulate(OrientedBoundingBox other)
        {
            if (!boxInfos.Contains(other))
            {
                boxInfos.Add(other);
                // Rebuild the AABB tree
                BuildAABBTree();
            }
        }

        public override bool ContainsPoint(Vector3 point)
        {
            if (root == null)
                return false;

            return ContainsPointInAABBTree(root, point);
        }

        private bool ContainsPointInAABBTree(TreeNode node, Vector3 point)
        {
            if (node == null)
                return false;

            // Check if the point is inside the AABB of this node
            if (!(point.x >= node.Min.x) || !(point.x <= node.Max.x) ||
                !(point.y >= node.Min.y) || !(point.y <= node.Max.y) ||
                !(point.z >= node.Min.z) || !(point.z <= node.Max.z)) return false;
            // Check if the point is inside the OBB of this node
            if (node.ContainsPoint(point))
                return true;

            // Recursively check the children
            foreach (var child in node.Children)
            {
                if (ContainsPointInAABBTree(child, point))
                    return true;
            }

            return false;
        }
    }
}