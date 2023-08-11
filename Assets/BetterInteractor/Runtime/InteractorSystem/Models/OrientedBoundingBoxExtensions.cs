using System.Collections.Generic;
using UnityEngine;

namespace Better.Interactor.Runtime.Models
{
    public static class OrientedBoundingBoxExtensions
    {
        public static IReadOnlyList<Vector3> ToAxes(this Quaternion orientation)
        {
            var axes = new Vector3[]
            {
                orientation * Vector3.right,
                orientation * Vector3.up,
                orientation * Vector3.forward
            };

            return axes;
        }

        public static IReadOnlyList<Vector3> ToAxes(this Matrix4x4 matrix)
        {
            var axes = new Vector3[]
            {
                matrix.MultiplyVector(Vector3.right),
                matrix.MultiplyVector(Vector3.up),
                matrix.MultiplyVector(Vector3.forward)
            };

            return axes;
        }
    }
}