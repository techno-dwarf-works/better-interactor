using System;
using UnityEngine;

namespace Better.Interactor.Runtime.Test
{
    struct Matrix3
    {
        float[,] Elements;
        
        public Matrix3(float f)
        {
            Elements = new float[3, 3];
        }

        public Matrix3(Matrix4x4 m)
        {
            Elements = new float[3, 3];

            Elements[0, 0] = m.m00;
            Elements[0, 1] = m.m01;
            Elements[0, 2] = m.m02;

            Elements[1, 0] = m.m10;
            Elements[1, 1] = m.m11;
            Elements[1, 2] = m.m12;

            Elements[2, 0] = m.m20;
            Elements[2, 1] = m.m21;
            Elements[2, 2] = m.m22;
        }

        public Matrix3(Matrix3 m)
        {
            Elements = new float[3, 3];

            Elements[0, 0] = m[0, 0];
            Elements[0, 1] = m[0, 1];
            Elements[0, 2] = m[0, 2];

            Elements[1, 0] = m[1, 0];
            Elements[1, 1] = m[1, 1];
            Elements[1, 2] = m[1, 2];

            Elements[2, 0] = m[2, 0];
            Elements[2, 1] = m[2, 1];
            Elements[2, 2] = m[2, 2];
        }

        public float this[int row, int column]
        {
            get
            {
                if (row > 2 || column > 2)
                    throw new IndexOutOfRangeException();

                return Elements[row, column];
            }
            set
            {
                if (row > 2 || column > 2)
                    throw new IndexOutOfRangeException();

                Elements[row, column] = value;
            }
        }


        public Vector3 Row(int row)
        {
            return new Vector3(Elements[row, 0],
                               Elements[row, 1],
                               Elements[row, 2]);
        }

        public Vector3 Column(int column)
        {
            return new Vector3(Elements[0, column],
                               Elements[1, column],
                               Elements[2, column]);
        }
    }
}