using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace PS.Shadow
{
    public struct VectorCell
    {
        public Vector2 vector;
        public bool flag;
        public VectorCell(float x, float y)
        {
            vector = new Vector2(x, y);
            flag = false;
        }
        public void Set(float x, float y)
        {
            vector.Set(x, y);
            flag = true;
        }
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
            xCoor = xCoor < 0 ? 0 : xCoor >= _xLength ? _xLength - 1 : xCoor;
            yCoor = yCoor < 0 ? 0 : yCoor >= _yLength ? _yLength - 1 : yCoor;

            return ref _field[yCoor, xCoor];
        }

        public ref VectorCell[,] GetField()
        {
            return ref _field;
        }

        public VectorCell[,] GetField(int left, int down, int right, int up)
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
                    returnValue[i, j] = GetVector(x, y);
                }
            }

            return returnValue;
        }

        public void SetField(int left, int down, int right, int up, VectorCell cell)
        {
            if (left > right)
                Swap(ref left, ref right);
            if (down > up)
                Swap(ref down, ref up);

            for (int y = down; y < up; y++)
            {
                for (int x = left; x < right; x++)
                {
                    GetVector(x, y).Set(cell.vector.x, cell.vector.y);
                }
            }
        }

        public IEnumerator FieldRecoveryCoroutine()
        {
            bool loop = true;

            while (loop)
            {
                for (int y = 0; y < _yLength; y++)
                    for (int x = 0; x < _xLength; x++)
                        if (_field[y, x].flag)
                        {
                            if (_field[y, x].vector.sqrMagnitude < 0.001f)
                            {
                                _field[y, x].flag = false;
                                _field[y, x].vector = Vector2.zero;
                            }
                            else
                            {
                                _field[y, x].vector *= 0.9f;
                            }
                        }

                yield return new WaitForSeconds(0.02f);
            }

        }
    }
}