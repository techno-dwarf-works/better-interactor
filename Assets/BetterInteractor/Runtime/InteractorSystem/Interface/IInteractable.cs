using System;
using Better.Interactor.Runtime.Models;
using Better.Interactor.Runtime.Test;
using UnityEngine;

namespace Better.Interactor.Runtime.Interface
{
    public interface IInteractable
    {
        public void InvokeGaze();

        public Transform transform { get; }
        public OrientedBoundingBox Bounds { get; }
    }
}
