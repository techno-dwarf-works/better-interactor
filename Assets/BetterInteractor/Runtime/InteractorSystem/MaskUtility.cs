using System.Collections.Generic;

namespace Better.Interactor.Runtime
{
    public static class MaskUtility
    {
        public static bool CompareMask(int left, int right)
        {
            return left.Equals(right);
        }

        public static MaskComparer Comparer { get; } = new MaskComparer();
        
        public class MaskComparer : IEqualityComparer<int>
        {
            public bool Equals(int x, int y)
            {
                return x.Equals(y);
            }

            public int GetHashCode(int obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}