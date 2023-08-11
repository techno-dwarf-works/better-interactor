using Better.Interactor.Runtime.Models;
using UnityEngine;

namespace Better.Interactor.Runtime
{
    public class DefaultInteractable : Interactable
    {
        [SerializeField] private OrientedBoundingBox boundingBox;
        
        public override void InvokeGaze()
        {
        }

        public override OrientedBoundingBox GetBounds()
        {
            return boundingBox;
        }
    }
}