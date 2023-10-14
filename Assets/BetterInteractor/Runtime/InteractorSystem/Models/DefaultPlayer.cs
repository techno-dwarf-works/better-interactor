using Better.Interactor.Runtime.BoundingBox;
using Better.Interactor.Runtime.Interface;
using UnityEngine;

namespace Better.Interactor.Runtime.Models
{
    public class DefaultPlayer : MonoBehaviour, IPlayerContainer
    {
        [SerializeField] private TransformOBB bounds;
        
        public OrientedBoundingBox Bounds => bounds;

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