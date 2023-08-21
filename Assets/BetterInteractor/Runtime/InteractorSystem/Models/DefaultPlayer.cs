using System;
using Better.Interactor.Runtime.Interface;
using Better.Interactor.Runtime.Test;
using UnityEngine;

namespace Better.Interactor.Runtime.Models
{
    public class DefaultPlayer : MonoBehaviour, IPlayerContainer
    {
        [Range(0,360)]
        [SerializeField] private float viewAngle = 20;
        [SerializeField] private TransformOBB bounds;
        
        public OrientedBoundingBox Bounds => bounds;
        public float ViewAngle => viewAngle;

        private void Awake()
        {
            bounds.SetTransform(transform);
        }

        private void Start()
        {
            InteractorSystem.AssignPlayer(this);
        }

        private void OnValidate()
        {
            bounds.SetTransform(transform);
        }
    }
}