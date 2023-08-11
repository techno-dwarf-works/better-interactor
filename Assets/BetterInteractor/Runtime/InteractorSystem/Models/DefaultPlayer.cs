using System;
using Better.Interactor.Runtime.Interface;
using UnityEngine;

namespace Better.Interactor.Runtime.Models
{
    public class DefaultPlayer : MonoBehaviour, IPlayerContainer
    {
        [SerializeField] private OrientedBoundingBox bounds;

        private void Start()
        {
            InteractorSystem.AssignPlayer(this);
        }

        public OrientedBoundingBox Bounds => bounds;
    }
}