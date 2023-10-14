using System;
using System.Linq;
using Better.Interactor.Runtime.BoundingBox;
using Better.Interactor.Runtime.Interface;
using Better.Interactor.Runtime.Models;
using UnityEngine;

namespace Better.Interactor.Runtime
{
    [ExecuteAlways]
    public class InteractorGizmoManager : MonoBehaviour
    {
        [Min(0)] [SerializeField] private float intersectionPointsSize = 0.1f;

        [ColorUsage(false, false)] [SerializeField]
        private Color objectsColor = Color.yellow;

        [ColorUsage(false, false)] [SerializeField]
        private Color raycastColor = Color.cyan;

        [ColorUsage(false, false)] [SerializeField]
        private Color intersectionColor = Color.red;

        [ColorUsage(false, false)] [SerializeField]
        private Color closestColor = Color.green;

        [ColorUsage(false, false)] [SerializeField]
        private Color groupsColor = Color.blue;

        [ColorUsage(false, false)] [SerializeField]
        private Color playerColor = Color.magenta;

        private IInteractable[] _findObjects = new IInteractable[0];
        private IPlayerContainer[] _players = new IPlayerContainer[0];
        private InteractableGroups _group = null;

#if UNITY_EDITOR

        private void OnEnable()
        {
            GatherReferences();
        }

        [ContextMenu("Gather References")]
        private void GatherReferences()
        {
            _findObjects = FindObjectsOfType<MonoBehaviour>(false).OfType<IInteractable>().ToArray();
            _players = FindObjectsOfType<MonoBehaviour>(false).OfType<IPlayerContainer>().ToArray();

            _group = new InteractableGroups();
            foreach (var interactable in _findObjects)
            {
                _group.AddInteractable(interactable);
            }
        }

        public void OnDrawGizmos()
        {
            var color = Gizmos.color;

            DrawObjects(_findObjects);

            DrawGroups(_group);

            DrawPlayers(_players);

            DrawIntersections(_findObjects, _players);

            Gizmos.color = color;
        }

        private void DrawGroups(InteractableGroups group)
        {
            if (group == null) return;
            Gizmos.color = groupsColor;
            foreach (var groupGroup in group)
            {
                DrawBounds(groupGroup.Bounds);
            }
        }

        private void DrawObjects(IInteractable[] findObjects)
        {
            Gizmos.color = objectsColor;
            foreach (var bounds in findObjects)
            {
                DrawBounds(bounds.Bounds);
            }
        }

        private void DrawPlayers(IPlayerContainer[] players)
        {
            Gizmos.color = playerColor;
            foreach (var player in players)
            {
                DrawBounds(player.Bounds);
            }
        }

        private void DrawIntersections(IInteractable[] findObjects, IPlayerContainer[] players)
        {
            foreach (var findObject in findObjects)
            {
                foreach (var player in players)
                {
                    DrawIntersection(player, findObject);
                    DrawRayCasts(player, findObject);
                }
            }
        }

        private void DrawRayCasts(IPlayerContainer player, IInteractable findObject)
        {
            var worldCenter = player.Bounds.GetWorldCenter();
            if (findObject.Bounds.Raycast(new Ray(worldCenter, player.transform.forward), out var hitPoint))
            {
                Gizmos.color = raycastColor;
                Gizmos.DrawLine(worldCenter, hitPoint);
                Gizmos.color = intersectionColor;
                Gizmos.DrawWireSphere(hitPoint, intersectionPointsSize);
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

                DrawClosestOnBounds(player, findObject);
            }
        }

        private void DrawClosestOnBounds(IPlayerContainer playerBounds, IInteractable findObject)
        {
            Gizmos.color = closestColor;
            var position = playerBounds.Bounds.GetWorldCenter();
            var onBounds = findObject.Bounds.GetClosestPointOnBounds(position);
            Gizmos.DrawLine(position, onBounds);
        }

        private void DrawBounds(OrientedBoundingBox bounds)
        {
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