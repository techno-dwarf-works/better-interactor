using System;
using System.Linq;
using Better.Interactor.Runtime.Interface;
using Better.Interactor.Runtime.Test;
using UnityEngine;

namespace Better.Interactor.Runtime
{
    public class InteractorGizmoManager : MonoBehaviour
    {
#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            var findObjects = FindObjectsOfType<MonoBehaviour>().OfType<IInteractable>().Select(x => x.GetBounds()).ToArray();

            foreach (var bounds in findObjects)
            {
                DrawBounds(bounds);
            }

            var players = FindObjectsOfType<MonoBehaviour>().OfType<IPlayerContainer>().Select(x => x.Bounds).ToArray();

            foreach (var bounds in players)
            {
                DrawBounds(bounds);
            }

            var color = Gizmos.color;
            Gizmos.color = Color.red;
            foreach (var findObject in findObjects)
            {
                foreach (var player in players)
                {
                    if (findObject.Intersects(player))
                    {
                        var intersections = findObject.GetIntersectionPoints(player);
                        foreach (var intersection in intersections)
                        {
                            Gizmos.DrawWireSphere(intersection, 0.1f);
                        }
                    }
                }
            }

            Gizmos.color = color;
        }

        private static void DrawBounds(OrientedBoundingBox bounds)
        {
            var originalGizmoMatrix = Gizmos.matrix;
            Gizmos.matrix = bounds.Transforms;

            var halfSize = bounds.LocalExtents;

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