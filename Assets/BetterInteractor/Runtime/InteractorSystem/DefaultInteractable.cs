﻿using System;
using Better.Interactor.Runtime.BoundingBox;
using Better.Interactor.Runtime.Models;
using UnityEngine;

namespace Better.Interactor.Runtime
{
    public class DefaultInteractable : Interactable
    {
        [SerializeField] private TransformOBB boundingBox;

        public override OrientedBoundingBox Bounds => boundingBox;

        private void Awake()
        {
            boundingBox.SetTransform(transform);
        }

        public override void InvokeGaze()
        {
        }

        private void OnValidate()
        {
            boundingBox.SetTransform(transform);
        }
    }
}