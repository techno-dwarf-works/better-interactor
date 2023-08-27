using System;
using System.Linq;
using Better.Interactor.Runtime.Interface;
using Better.Interactor.Runtime.Models;
using Better.Interactor.Runtime.Test;
using UnityEngine;

namespace Better.Interactor.Runtime
{
    public class InteractorGizmoManager : MonoBehaviour
    {
        [Min(0)] 
        [SerializeField] private float intersectionPointsSize = 0.1f;
        
        [Range(4, 32)] 
        [SerializeField] private int coneSegments = 16;
        
        [ColorUsage(false, false)]
        [SerializeField] private Color boundsColor = Color.yellow;
        
        [ColorUsage(false, false)]
        [SerializeField] private Color intersectionColor = Color.red;
        [ColorUsage(false, false)]
        [SerializeField] private Color closestColor = Color.green;

        private const float Two = 2f;

#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            var color = Gizmos.color;
            var findObjects = FindObjectsOfType<MonoBehaviour>().OfType<IInteractable>().ToArray();

            DrawObjects(findObjects);

            var group = new InteractableGroups();
            foreach (var interactable in findObjects)
            {
                group.AddInteractable(interactable);
            }

            foreach (var groupGroup in group.Groups)
            {
                DrawBounds(groupGroup.Bounds);
            }

            var players = FindObjectsOfType<MonoBehaviour>().OfType<IPlayerContainer>().ToArray();

            DrawPlayers(players);

            DrawIntersections(findObjects, players);

            Gizmos.color = color;
        }

        private void DrawObjects(IInteractable[] findObjects)
        {
            foreach (var bounds in findObjects)
            {
                DrawBounds(bounds.Bounds);
            }
        }

        private void DrawPlayers(IPlayerContainer[] players)
        {
            foreach (var player in players)
            {
                DrawBounds(player.Bounds);
                DrawViewCone(player);
            }
        }

        private void DrawIntersections(IInteractable[] findObjects, IPlayerContainer[] players)
        {
            foreach (var findObject in findObjects)
            {
                foreach (var player in players)
                {
                    DrawIntersection(player, findObject);
                    DrawClosestOnBounds(player, findObject);
                }
            }
        }

        private void DrawIntersection(IPlayerContainer player, IInteractable findObject)
        {
            Gizmos.color = intersectionColor;
            var objectBounds = findObject.Bounds;
            var playerBounds = player.Bounds;
            if (objectBounds.Intersects(playerBounds))
            {
                var intersections = objectBounds.GetIntersectionPoints(playerBounds);
                foreach (var intersection in intersections)
                {
                    Gizmos.DrawWireSphere(intersection, intersectionPointsSize);
                }
            }
        }

        private void DrawClosestOnBounds(IPlayerContainer playerBounds, IInteractable findObject)
        {
            Gizmos.color = closestColor;
            var position = playerBounds.Bounds.GetWorldCenter();
            var onBounds = findObject.Bounds.GetClosestPointOnBounds(position);
            Gizmos.DrawLine(position, onBounds);
        }

        private void DrawViewCone(IPlayerContainer player)
        {
            Gizmos.color = Color.blue;
            var halfAngleRadians = Mathf.Deg2Rad * player.ViewAngle / Two;
            var viewConeHeight = player.Bounds.LocalExtents.z;
            var radius = Mathf.Tan(halfAngleRadians) * viewConeHeight;

            var containerTransform = player.transform;
            var apex = containerTransform.position;

            var forward = containerTransform.forward;
            var right = containerTransform.right;
            var up = containerTransform.up;

            var baseCenter = apex + forward * viewConeHeight;

            var prevPoint = apex + right * radius + forward * viewConeHeight;

            for (var i = 1; i <= coneSegments; i++)
            {
                var angleStep = Two * Mathf.PI / coneSegments;
                var angle = angleStep * i;

                var circlePoint = baseCenter + (Mathf.Cos(angle) * right + Mathf.Sin(angle) * up) * radius;

                Gizmos.DrawLine(prevPoint, circlePoint);

                prevPoint = circlePoint;
            }

            // Draw lines from apex to base points
            Gizmos.DrawLine(apex, baseCenter + right * radius);
            Gizmos.DrawLine(apex, baseCenter - right * radius);
            Gizmos.DrawLine(apex, baseCenter + up * radius);
            Gizmos.DrawLine(apex, baseCenter - up * radius);
        }

        private void DrawBounds(OrientedBoundingBox bounds)
        {
            Gizmos.color = boundsColor;
            var originalGizmoMatrix = Gizmos.matrix;
            Gizmos.matrix = bounds.Transforms;

            var halfSize = bounds.LocalExtents;

            // Draw the wireframe box using Gizmos.DrawLine
            var center = bounds.LocalCenter;
            var p0 = new Vector3(-halfSize.x + center.x, -halfSize.y + center.y, -halfSize.z + center.z);
            var p1 = new Vector3(-halfSize.x + center.x, -halfSize.y + center.y, halfSize.z + center.z);
            var p2 = new Vector3(halfSize.x + center.x, -halfSize.y + center.y, halfSize.z + center.z);
            var p3 = new Vector3(halfSize.x + center.x, -halfSize.y + center.y, -halfSize.z + center.z);
            var p4 = new Vector3(-halfSize.x + center.x, halfSize.y + center.y, -halfSize.z + center.z);
            var p5 = new Vector3(-halfSize.x + center.x, halfSize.y + center.y, halfSize.z + center.z);
            var p6 = new Vector3(halfSize.x + center.x, halfSize.y + center.y, halfSize.z + center.z);
            var p7 = new Vector3(halfSize.x + center.x, halfSize.y + center.y, -halfSize.z + center.z);

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