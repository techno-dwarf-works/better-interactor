using UnityEngine;

namespace Better.Interactor.Runtime.BoundingBox
{
    public static class OBBUtility
    {
        public const float Half = 0.5f;
        public const int AxesCount = 3;
        public const int CornersCount = 8;

        public static void GetMinMax(Vector3[] corners, ref Vector3 minPoint, ref Vector3 maxPoint)
        {
            if (corners.Length < CornersCount) return;
            for (var i = 0; i < CornersCount; i++)
            {
                var cornerWorld = corners[i];
                minPoint = Vector3.Min(minPoint, cornerWorld);
                maxPoint = Vector3.Max(maxPoint, cornerWorld);
            }
        }
    }
}