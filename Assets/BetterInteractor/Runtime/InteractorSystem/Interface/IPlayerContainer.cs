using Better.Interactor.Runtime.Models;
using UnityEngine;

namespace Better.Interactor.Runtime.Interface
{
    public interface IPlayerContainer
    {
        public Transform transform { get; }
        public OrientedBoundingBox Bounds { get; }
    }
}