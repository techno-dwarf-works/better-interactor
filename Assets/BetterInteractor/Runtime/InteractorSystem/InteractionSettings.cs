using System;
using System.Collections.Generic;
using System.Linq;
using Better.Attributes.Runtime.Headers;
using Better.Tools.Runtime.Settings;
using UnityEditor;
using UnityEngine;

namespace Better.Interactor.Runtime
{
    public class InteractionSettings : ProjectSettings
    {
        [SerializeField] private NamedMask[] namedMasks = new NamedMask[]
        {
            new NamedMask("Default", 0)
        };
        
        [SettingsHeader(nameof(Editor))]
        [Min(0)] 
        [SerializeField] private float intersectionPointsSize = 0.1f;

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

        public Color GroupsColor => groupsColor;

        public Color ObjectsColor => objectsColor;

        public Color PlayerColor => playerColor;

        public Color RaycastColor => raycastColor;

        public Color IntersectionColor => intersectionColor;

        public Color ClosestColor => closestColor;

        public float IntersectionPointsSize => intersectionPointsSize;

        public static List<Tuple<string, int>> GetNamedMasks()
        {
            var settings = Resources.Load<InteractionSettings>(nameof(InteractionSettings));

            if (settings == null) return new List<Tuple<string, int>>();

            return settings.namedMasks.Select(x => new Tuple<string, int>(x.Name, x.Mask)).ToList();
        }
    }
}