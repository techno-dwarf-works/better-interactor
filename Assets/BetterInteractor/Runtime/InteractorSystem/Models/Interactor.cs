using System;
using Better.Interactor.Runtime.Attributes;
using Better.Interactor.Runtime.BoundingBox;
using Better.Interactor.Runtime.Interface;
using Better.Interactor.Runtime.MediatorModule;
using UnityEngine;

namespace Better.Interactor.Runtime.Models
{
    public class Interactor : MonoBehaviour, IInteractorContainer
    {
        [SerializeField] private TransformOBB bounds;
        
        [Mask]
        [SerializeField] private int mask;

        public OrientedBoundingBox Bounds => bounds;

        public int Mask => mask;

        private void Awake()
        {
            bounds.SetTransform(transform);
        }

        private void Start()
        {
            InteractorSystem.Register(this);
        }

        private void OnDestroy()
        {
            InteractorSystem.Unregister(this);
        }

        private void OnValidate()
        {
            bounds.SetTransform(transform);
        }
    }
}