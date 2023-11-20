using System;
using System.Linq;
using Better.EditorTools.SettingsTools;
using Better.Interactor.Runtime;
using Better.Interactor.Runtime.BoundingBox;
using Better.Interactor.Runtime.Interface;
using Better.Interactor.Runtime.MediatorModule;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Better.Interactor.EditorAddons
{
    [InitializeOnLoad]
    public static class GizmoDrawer
    {
        private static InteractionSettings _settings;
        private static IInteractable[] _interactables = new IInteractable[0];
        private static IInteractorContainer[] _containers = new IInteractorContainer[0];
        private static Groups _group = null;

        static GizmoDrawer()
        {
            EditorApplication.delayCall += DelayCall;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        //TODO: Rework it
        private static void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        {
            SceneView.duringSceneGui -= OnDuringSceneGui;
            
            switch (stateChange)
            {
                case PlayModeStateChange.EnteredPlayMode:
                case PlayModeStateChange.EnteredEditMode:
                    GatherReferences();
                    SceneView.duringSceneGui += OnDuringSceneGui;
                    break;
                case PlayModeStateChange.ExitingEditMode:
                case PlayModeStateChange.ExitingPlayMode:
                    ClearReferences();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stateChange), stateChange, null);
            }
        }

        private static void DelayCall()
        {
            _settings = ProjectSettingsToolsContainer<InteractionSettingsTool>.Instance.LoadOrCreateScriptableObject();
            GatherReferences();
            SceneView.duringSceneGui += OnDuringSceneGui;
        }

        private static void OnDuringSceneGui(SceneView obj)
        {
            OnDrawGizmos();
        }

        [MenuItem(InteractionSettingsTool.MenuItemPrefix + "/" + "Gather References")]
        private static void GatherReferences()
        {
            _interactables = Object.FindObjectsOfType<MonoBehaviour>(false).OfType<IInteractable>().ToArray();
            _containers = Object.FindObjectsOfType<MonoBehaviour>(false).OfType<IInteractorContainer>().ToArray();

            _group = new Groups();
            foreach (var interactable in _interactables)
            {
                _group.AddInteractable(interactable);
            }
        }

        private static void ClearReferences()
        {
            _interactables = null;
            _containers = null;
            _group = null;
        }

        private static void OnDrawGizmos()
        {
            var color = Handles.color;

            DrawObjects(_interactables);

            DrawGroups(_group);

            DrawPlayers(_containers);

            DrawIntersections(_interactables, _containers);

            Handles.color = color;
        }

        private static void DrawGroups(Groups group)
        {
            if (group == null) return;
            Handles.color = _settings.GroupsColor;
            foreach (var groupGroup in group.GetGroups())
            {
                var boundingBox = groupGroup.Bounds;
                var pos = boundingBox.Transforms.GetPosition() + boundingBox.LocalCenter;
                Handles.Label(pos, $"Group with mask: {groupGroup.Mask}");
                DrawBounds(groupGroup.Bounds);
            }
        }

        private static void DrawObjects(IInteractable[] findObjects)
        {
            Handles.color = _settings.ObjectsColor;
            foreach (var bounds in findObjects)
            {
                DrawBounds(bounds.Bounds);
            }
        }

        private static void DrawPlayers(IInteractorContainer[] players)
        {
            Handles.color = _settings.PlayerColor;
            foreach (var player in players)
            {
                DrawBounds(player.Bounds);
            }
        }

        private static void DrawIntersections(IInteractable[] findObjects, IInteractorContainer[] players)
        {
            foreach (var findObject in findObjects)
            {
                foreach (var player in players)
                {
                    if (!MaskUtility.CompareMask(findObject.Mask, player.Mask)) continue;
                    DrawIntersection(player, findObject);
                    DrawRayCasts(player, findObject);
                }
            }
        }

        private static void DrawRayCasts(IInteractorContainer interactor, IInteractable findObject)
        {
            var worldCenter = interactor.Bounds.GetWorldCenter();
            if (findObject.Bounds.Raycast(new Ray(worldCenter, interactor.transform.forward), out var hitPoint))
            {
                Handles.color = _settings.RaycastColor;
                Handles.DrawLine(worldCenter, hitPoint);
                Handles.color = _settings.IntersectionColor;
                DrawSphere(hitPoint, _settings.IntersectionPointsSize);
            }
        }

        private static void DrawIntersection(IInteractorContainer interactor, IInteractable findObject)
        {
            Handles.color = _settings.IntersectionColor;
            var objectBounds = findObject.Bounds;
            var playerBounds = interactor.Bounds;
            if (objectBounds.Intersects(playerBounds))
            {
                var intersections = objectBounds.GetIntersectionPoints(playerBounds);
                foreach (var intersection in intersections)
                {
                    DrawSphere(intersection, _settings.IntersectionPointsSize);
                }

                DrawClosestOnBounds(interactor, findObject);
            }
        }

        private static void DrawSphere(Vector3 position, float radius)
        {
            Handles.DrawWireDisc(position, Vector3.up, radius, 0.1f); // Draw a wire disc
            Handles.DrawWireDisc(position, Vector3.right, radius, 0.1f); // Draw another wire disc
            Handles.DrawWireDisc(position, Vector3.forward, radius, 0.1f); // Draw a third wire disc
        }

        private static void DrawClosestOnBounds(IInteractorContainer interactorBounds, IInteractable findObject)
        {
            Handles.color = _settings.ClosestColor;
            var position = interactorBounds.Bounds.GetWorldCenter();
            var onBounds = findObject.Bounds.GetClosestPointOnBounds(position);
            Handles.DrawLine(position, onBounds);
        }

        private static void DrawBounds(OrientedBoundingBox bounds)
        {
            var originalGizmoMatrix = Handles.matrix;
            Handles.matrix = bounds.Transforms;

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

            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p1, p2);
            Handles.DrawLine(p2, p3);
            Handles.DrawLine(p3, p0);

            Handles.DrawLine(p4, p5);
            Handles.DrawLine(p5, p6);
            Handles.DrawLine(p6, p7);
            Handles.DrawLine(p7, p4);

            Handles.DrawLine(p0, p4);
            Handles.DrawLine(p1, p5);
            Handles.DrawLine(p2, p6);
            Handles.DrawLine(p3, p7);

            Handles.matrix = originalGizmoMatrix;
        }
    }
}