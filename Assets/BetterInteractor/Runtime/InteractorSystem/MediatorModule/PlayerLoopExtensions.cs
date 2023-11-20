using System;
using System.Collections.Generic;
using UnityEngine.LowLevel;

namespace Better.Interactor.Runtime.MediatorModule
{
    public static class PlayerLoopExtensions
    {
        public static Type GetParentSystemType<T>(this PlayerLoopSystem playerLoop)
        {
            var systems = new Stack<PlayerLoopSystem>();
            systems.Push(playerLoop);

            while (systems.Count > 0)
            {
                var parent = systems.Pop();
                if (parent.subSystemList != null)
                {
                    var subSystemCount = parent.subSystemList.Length;
                    for (var i = 0; i < subSystemCount; i++)
                    {
                        var subSystem = parent.subSystemList[i];
                        if (subSystem.type == typeof(T))
                        {
                            return parent.type;
                        }

                        systems.Push(subSystem);
                    }
                }
            }

            return null;
        }

        public static ref PlayerLoopSystem GetSystem(this ref PlayerLoopSystem playerLoop, Type type)
        {
            return ref GetSystemRecursive(ref playerLoop, type);
        }

        private static ref PlayerLoopSystem GetSystemRecursive(ref PlayerLoopSystem playerLoop, Type type)
        {
            if (playerLoop.type == type)
            {
                return ref playerLoop;
            }

            var subSystems = playerLoop.subSystemList;

            if (subSystems != null)
            {
                for (int i = 0; i < subSystems.Length; i++)
                {
                    ref var result = ref GetSystemRecursive(ref subSystems[i], type);
                    if (result.type == type)
                    {
                        return ref result;
                    }
                }
            }

            return ref playerLoop;
        }

        public static ref PlayerLoopSystem GetParentSystem<T>(this ref PlayerLoopSystem playerLoop)
        {
            var parentType = GetParentSystemType<T>(playerLoop);
            if (parentType != null)
            {
                return ref GetSystem(ref playerLoop, parentType);
            }

            throw new Exception($"Player loop parent type {typeof(T)} doesn't exist");
        }

        public static bool InsertAfter<T>(this PlayerLoopSystem playerLoop, PlayerLoopSystem.UpdateFunction method, Type systemType)
        {
            ref var parentSystem = ref GetParentSystem<T>(ref playerLoop);
            var length = parentSystem.subSystemList.Length;

            for (var i = 0; i < length; i++)
            {
                ref var subSystem = ref parentSystem.subSystemList[i];
                if (subSystem.type == systemType)
                {
                    return false;
                }
            }

            for (var i = 0; i < length; i++)
            {
                ref var subSystem = ref parentSystem.subSystemList[i];
                if (subSystem.type != typeof(T)) continue;
                var customSystem = new PlayerLoopSystem
                {
                    type = systemType,
                    updateDelegate = method
                };

                var subSystemList = new List<PlayerLoopSystem>(parentSystem.subSystemList);
                if (i >= length)
                {
                    subSystemList.Add(customSystem);
                }
                else
                {
                    subSystemList.Insert(i + 1, customSystem);
                }

                parentSystem.subSystemList = subSystemList.ToArray();
                return true;
            }

            return false;
        }
    }
}