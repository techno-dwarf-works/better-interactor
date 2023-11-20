using System;
using UnityEngine;

namespace Better.Interactor.Runtime
{
    [Serializable]
    public struct NamedMask
    {
        [SerializeField] private string name;
        [SerializeField] private int mask;

        public NamedMask(string name, int mask)
        {
            this.mask = mask;
            this.name = name;
        }

        public int Mask => mask;
        public string Name => name;
    }
}