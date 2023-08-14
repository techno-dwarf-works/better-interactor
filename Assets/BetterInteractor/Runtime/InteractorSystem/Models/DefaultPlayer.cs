﻿using System;
using Better.Interactor.Runtime.Interface;
using Better.Interactor.Runtime.Test;
using UnityEngine;

namespace Better.Interactor.Runtime.Models
{
    public class DefaultPlayer : MonoBehaviour, IPlayerContainer
    {
        [SerializeField] private TransformOBB bounds;

        private void Awake()
        {
            bounds.SetTransform(transform);
        }

        private void Start()
        {
            InteractorSystem.AssignPlayer(this);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            bounds.SetTransform(transform);
            bounds.DrawGizmos();
        }
#endif
        
        public OrientedBoundingBox Bounds => bounds;
    }
}