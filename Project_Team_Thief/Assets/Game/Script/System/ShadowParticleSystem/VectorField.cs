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
        public void Set(Vector2 vector)
        {
            this.vector = vector;
            flag = true;
        }
    }

    public class VectorField
    {
        private readonly int _xLength, _yLength;
        private VectorCell[,] _field;
        private Vector2Int _cellPixelSize;

        private void Swap(ref int a, ref int b)
        {
            a = a ^ b;
            b = a ^ b;
            a = a ^ b;
        }
        private void Swap(ref float a, ref float b)
        {
            float temp = a;
            a = b;
            b = temp;
        }

        public VectorField(int xLength, int yLength, Vector2Int cellPixelSize)
        {
            _cellPixelSize = cellPixelSize;
            _xLength = xLength / cellPixelSize.x;
            _yLength = yLength / cellPixelSize.y;
            _field = new VectorCell[_yLength, _xLength];
        }

        public ref VectorCell GetVector(int xCoor, int yCoor)
        {
            xCoor = xCoor < 0 ? 0 : xCoor >= _xLength ? _xLength - 1 : xCoor;
            yCoor = yCoor < 0 ? 0 : yCoor >= _yLength ? _yLength - 1 : yCoor;

            return ref _field[yCoor, xCoor];
        }
        
        public ref VectorCell GetVectorWithScreenPos(float xScreenCoor, float yScreenCoor)
        {
            int xCoor = (int)xScreenCoor / _cellPixelSize.x;
            int yCoor = (int)yScreenCoor / _cellPixelSize.y;
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
            left /= _cellPixelSize.x;
            right /= _cellPixelSize.x;
            down /= _cellPixelSize.y;
            up /= _cellPixelSize.y;

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
            left /= _cellPixelSize.x;
            right /= _cellPixelSize.x;
            down /= _cellPixelSize.y;
            up /= _cellPixelSize.y;

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

        public void MoveField(int horizonal, int vertical)
        {
            if (horizonal > 0)
            {
                int i = _xLength - 1;
                while (i >= horizonal)
                {
                    for(int j = 0; j < _yLength; j++)
                    {
                        GetVector(i, j).vector = GetVector(i - horizonal, j).vector;
                    }
                    i--;
                }
                while(i >= 0)
                {
                    for (int j = 0; j < _yLength; j++)
                    {
                        GetVector(i, j).vector = Vector2.zero;
                    }
                    i--;
                }
            }
            else if (horizonal < 0)
            {
                int i = 0;
                while (i < _xLength + horizonal)
                {
                    for (int j = 0; j < _yLength; j++)
                    {
                        GetVector(i, j).vector = GetVector(i - horizonal, j).vector;
                    }
                    i++;
                }
                while (i < _xLength)
                {
                    for (int j = 0; j < _yLength; j++)
                    {
                        GetVector(i, j).vector = Vector2.zero;
                    }
                    i++;
                }
            }

            if (vertical > 0)
            {
                int j = _yLength - 1;
                while (j >= vertical)
                {
                    for (int i = 0; i < _xLength; i++)
                    {
                        GetVector(i, j).vector = GetVector(i, j - vertical).vector;
                    }
                    j--;
                }
                while (j >= 0)
                {
                    for (int i = 0; i < _xLength; i++)
                    {
                        GetVector(i, j).vector = Vector2.zero;
                    }
                    j--;
                }
            }
            else if (vertical < 0)
            {
                int j = 0;
                while (j < _yLength + vertical)
                {
                    for (int i = 0; i < _xLength; i++)
                    {
                        GetVector(i, j).vector = GetVector(i, j - vertical).vector;
                    }
                    j++;
                }
                while (j < _yLength)
                {
                    for (int i = 0; i < _xLength; i++)
                    {
                        GetVector(i, j).vector = Vector2.zero;
                    }
                    j++;
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