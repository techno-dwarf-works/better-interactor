﻿using System;
using Better.Interactor.Runtime.Models;
using Better.Interactor.Runtime.Test;
using UnityEngine;

namespace Better.Interactor.Runtime
{
    public class DefaultInteractable : Interactable
    {
        [SerializeField] private TransformOBB boundingBox;

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

        public override OrientedBoundingBox GetBounds()
        {
            return boundingBox;
        }
    }
}