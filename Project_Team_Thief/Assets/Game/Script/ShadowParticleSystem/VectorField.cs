using System.Collections;
using System.Collections.Generic;
using System;

namespace PS.Shadow
{
    public struct VectorCell
    {
        public float x, y;
    }

    public class VectorField
    {
        private readonly int _xLength, _yLength;
        private VectorCell[,] _field;

        private void Swap(ref int a, ref int b)
        {
            a = a ^ b;
            b = a ^ b;
            a = a ^ b;
        }

        public VectorField(int xLength, int yLength)
        {
            _xLength = xLength;
            _yLength = yLength;
            _field = new VectorCell[_yLength, _xLength];
        }

        public ref VectorCell GetVector(int xCoor, int yCoor)
        {
            return ref _field[yCoor, xCoor];
        }

        public ref VectorCell[,] GetField()
        {
            return ref _field;
        }

        public VectorCell[,] GetField(int left, int right, int up, int down)
        {
            if (left > right)
                Swap(ref left, ref right);
            if (down > up)
                Swap(ref down, ref up);

            VectorCell[,] returnValue = new VectorCell[up - down, right - left];
            for (int y = down, i = 0; y < up; y++, i++)
            {
                for (int x = left, j = 0; x < right; x++, j++)
                {
                    returnValue[i, j] = _field[y, x];
                }
            }

            return returnValue;
        }
    }
}