using System.Linq;
using Better.Interactor.Runtime.Interface;
using Better.Interactor.Runtime.Models;
using Better.Interactor.Runtime.Test;
using UnityEditor;
using UnityEngine;

namespace Better.Interactor.Runtime
{
    public static class TestRunner
    {
        [MenuItem("Test Runner/Randomize")]
        private static void Randomize()
        {
            foreach (var transform in Selection.transforms)
            {
                transform.SetPositionAndRotation(Random.insideUnitSphere * 50f, Random.rotation);
            }
        }

        [MenuItem("Test Runner/Run Test")]
        private static void Test()
        {
            var tester = new SmallTester();
            var findObjects = Object.FindObjectsOfType<MonoBehaviour>().OfType<IInteractable>().ToArray();
            var players = Object.FindObjectsOfType<MonoBehaviour>().OfType<IPlayerContainer>().ToArray();


            //RunGroup(tester, findObjects);

            RunIntersects(tester, findObjects, players);
            
            RunGetIntersectionPoints(tester, findObjects, players);

            RunGetClosestPointOnBounds(tester, findObjects, players);
            
            RunRaycast(tester, findObjects, players);

            tester.ReportTotal();
        }

        private static void RunGroup(SmallTester tester, IInteractable[] findObjects)
        {
            tester.Start();
            var group = new InteractableGroups();
            foreach (var interactable in findObjects)
            {
                group.AddInteractable(interactable);
            }

            tester.Stop();

            tester.Report(nameof(InteractableGroups.AddInteractable), findObjects.Length);
            tester.Reset();
        }

        private static void RunIntersects(SmallTester tester, IInteractable[] findObjects, IPlayerContainer[] players)
        {
            tester.Start();
            foreach (var findObject in findObjects)
            {
                foreach (var player in players)
                {
                    var objectBounds = findObject.Bounds;
                    var playerBounds = player.Bounds;
                    objectBounds.Intersects(playerBounds);
                }
            }

            tester.Stop();

            tester.Report(nameof(OrientedBoundingBox.Intersects), findObjects.Length + players.Length);
            tester.Reset();
        }

        private static void RunGetIntersectionPoints(SmallTester tester, IInteractable[] findObjects, IPlayerContainer[] players)
        {
            tester.Start();
            foreach (var findObject in findObjects)
            {
                foreach (var player in players)
                {
                    var objectBounds = findObject.Bounds;
                    var playerBounds = player.Bounds;
                    objectBounds.GetIntersectionPoints(playerBounds);
                }
            }

            tester.Stop();

            tester.Report(nameof(OrientedBoundingBox.GetIntersectionPoints), findObjects.Length + players.Length);
            tester.Reset();
        }

        private static void RunGetClosestPointOnBounds(SmallTester tester, IInteractable[] findObjects, IPlayerContainer[] players)
        {
            tester.Start();
            foreach (var findObject in findObjects)
            {
                foreach (var player in players)
                {
                    var objectBounds = findObject.Bounds;
                    var playerBounds = player.Bounds;
                    objectBounds.GetClosestPointOnBounds(playerBounds.GetWorldCenter());
                }
            }

            tester.Stop();

            tester.Report(nameof(OrientedBoundingBox.GetClosestPointOnBounds), findObjects.Length + players.Length);
            tester.Reset();
        }
        
        private static void RunRaycast(SmallTester tester, IInteractable[] findObjects, IPlayerContainer[] players)
        {
            tester.Start();
            foreach (var findObject in findObjects)
            {
                foreach (var player in players)
                {
                    var objectBounds = findObject.Bounds;
                    var playerBounds = player.Bounds;
                    objectBounds.Raycast(new Ray(playerBounds.GetWorldCenter(), player.transform.forward), out _);
                }
            }

            tester.Stop();

            tester.Report(nameof(OrientedBoundingBox.Raycast), findObjects.Length + players.Length);
            tester.Reset();
        }
    }
}