#region License Header
/***************************************************************************
 *   Copyright (c) 2011 OpenUO Software Team.
 *   All Right Reserved.
 *
 *   $Id: $:
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 ***************************************************************************/
 #endregion

using System;
using SharpDX;

namespace Client.Graphics
{
    sealed class WorldStackProvider : MatrixSource
    {
        private const bool TestMatrixEquality = false;

        private readonly Matrix[] _stack;
        private readonly int[] _stackIndex;
        private readonly bool[] _stackIdentity;
        private int _highpoint = 1;
        internal uint _top;

        internal bool _isIdentity = true;
#if DEBUG
        private DrawState _state;
#endif

        public WorldStackProvider(int stackSize, DrawState state)
        {
#if DEBUG
            _state = state;
#endif
            _stack = new Matrix[stackSize];
            _stackIndex = new int[stackSize];
            _stackIdentity = new bool[stackSize];
        }

        public void Set(ref Matrix matrix)
        {
#if DEBUG
            _state.Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.SetWorldMatrix);
#endif
            if (_top == 0)
                throw new InvalidOperationException("World matrix at the bottom of the stack must stay an Identity Matrix, Please use PushWorldMatrix()");

            Value = matrix;
            _index = ++_highpoint;
            _isIdentity = false;
        }

        public void Push(ref Matrix matrix)
        {
#if DEBUG
            _state.Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.PushWorldMatrix);
#endif
            if (_top == 0)
            {
                Value = matrix;
                _index = ++_highpoint;
                _isIdentity = false;
            }
            else
            {
                if (_index != _stackIndex[_top])
                {
                    _stack[_top] = matrix;
                    _stackIndex[_top] = _index;
                    _stackIdentity[_top] = _isIdentity;
                }

                Value = matrix;
                _index = ++_highpoint;
                _isIdentity = false;
            }

            _top++;
        }

        public void PushTrans(ref Vector3 translate)
        {
#if DEBUG
            _state.Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.PushTranslateWorldMatrix);
#endif
            if (_top == 0)
            {
                if (translate.X != 0 || translate.Y != 0 || translate.Z != 0)
                {
                    Value.M41 = translate.X;
                    Value.M42 = translate.Y;
                    Value.M43 = translate.Z;
                    _isIdentity = false;

                    _index = ++_highpoint;
                }
            }
            else
            {
                if (_index != _stackIndex[_top])
                {
                    _stack[_top] = Value;
                    _stackIndex[_top] = _index;
                    _stackIdentity[_top] = _isIdentity;
                }

                Value.M11 = 1;
                Value.M12 = 0;
                Value.M13 = 0;
                Value.M14 = 0;

                Value.M21 = 0;
                Value.M22 = 1;
                Value.M23 = 0;
                Value.M24 = 0;

                Value.M31 = 0;
                Value.M32 = 0;
                Value.M33 = 1;
                Value.M34 = 0;

                Value.M41 = translate.X;
                Value.M42 = translate.Y;
                Value.M43 = translate.Z;
                Value.M44 = 1;

                _index = ++_highpoint;
                _isIdentity = false;
            }

            _top++;
        }

        public void PushMultTrans(ref Vector3 translate)
        {
#if DEBUG
            _state.Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.PushMultiplyTranslateWorldMatrix);
#endif
            if (_top == 0)
            {
                if (
                    translate.X != 0 || translate.Y != 0 || translate.Z != 0
                    )//prevents recalcuating shader constants later if not changed now
                {
                    Value.M41 = translate.X;
                    Value.M42 = translate.Y;
                    Value.M43 = translate.Z;

                    _isIdentity = false;
                    _index = ++_highpoint;
                }
            }
            else
            {
                if (_index != _stackIndex[_top])
                {
                    _stack[_top] = Value;
                    _stackIndex[_top] = _index;
                    _stackIdentity[_top] = _isIdentity;
                }

                if (translate.X != 0 || translate.Y != 0 || translate.Z != 0)
                {
                    if (_isIdentity)
                    {
                        Value.M41 = translate.X;
                        Value.M42 = translate.Y;
                        Value.M43 = translate.Z;
                    }
                    else
                    {
                        Value.M41 += translate.X * Value.M11 +
                                            translate.Y * Value.M21 +
                                            translate.Z * Value.M31;

                        Value.M42 += translate.X * Value.M12 +
                                            translate.Y * Value.M22 +
                                            translate.Z * Value.M32;

                        Value.M43 += translate.X * Value.M13 +
                                            translate.Y * Value.M23 +
                                            translate.Z * Value.M33;
                    }

                    _isIdentity = false;
                    _index = ++_highpoint;
                }
            }

            _top++;
        }

        public void PushMult(ref Matrix matrix)
        {
#if DEBUG
            _state.Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.PushMultiplyWorldMatrix);
#endif
            if (_top == 0)
            {
                Value = matrix;
                _index = ++_highpoint;
                _isIdentity = false;
            }
            else
            {
                if (_index != _stackIndex[_top])
                {
                    _stack[_top] = Value;
                    _stackIndex[_top] = _index;
                    _stackIdentity[_top] = _isIdentity;
                }

                if (_isIdentity)
                    Value = matrix;
                else
                {
#if DEBUG
                    _state.Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.MatrixMultiplyCalculate);
#endif
                    float num16 = (((matrix.M11 * Value.M11) + (matrix.M12 * Value.M21)) + (matrix.M13 * Value.M31)) + (matrix.M14 * Value.M41);
                    float num15 = (((matrix.M11 * Value.M12) + (matrix.M12 * Value.M22)) + (matrix.M13 * Value.M32)) + (matrix.M14 * Value.M42);
                    float num14 = (((matrix.M11 * Value.M13) + (matrix.M12 * Value.M23)) + (matrix.M13 * Value.M33)) + (matrix.M14 * Value.M43);
                    float num13 = (((matrix.M11 * Value.M14) + (matrix.M12 * Value.M24)) + (matrix.M13 * Value.M34)) + (matrix.M14 * Value.M44);
                    float num12 = (((matrix.M21 * Value.M11) + (matrix.M22 * Value.M21)) + (matrix.M23 * Value.M31)) + (matrix.M24 * Value.M41);
                    float num11 = (((matrix.M21 * Value.M12) + (matrix.M22 * Value.M22)) + (matrix.M23 * Value.M32)) + (matrix.M24 * Value.M42);
                    float num10 = (((matrix.M21 * Value.M13) + (matrix.M22 * Value.M23)) + (matrix.M23 * Value.M33)) + (matrix.M24 * Value.M43);
                    float num9 = (((matrix.M21 * Value.M14) + (matrix.M22 * Value.M24)) + (matrix.M23 * Value.M34)) + (matrix.M24 * Value.M44);
                    float num8 = (((matrix.M31 * Value.M11) + (matrix.M32 * Value.M21)) + (matrix.M33 * Value.M31)) + (matrix.M34 * Value.M41);
                    float num7 = (((matrix.M31 * Value.M12) + (matrix.M32 * Value.M22)) + (matrix.M33 * Value.M32)) + (matrix.M34 * Value.M42);
                    float num6 = (((matrix.M31 * Value.M13) + (matrix.M32 * Value.M23)) + (matrix.M33 * Value.M33)) + (matrix.M34 * Value.M43);
                    float num5 = (((matrix.M31 * Value.M14) + (matrix.M32 * Value.M24)) + (matrix.M33 * Value.M34)) + (matrix.M34 * Value.M44);
                    float num4 = (((matrix.M41 * Value.M11) + (matrix.M42 * Value.M21)) + (matrix.M43 * Value.M31)) + (matrix.M44 * Value.M41);
                    float num3 = (((matrix.M41 * Value.M12) + (matrix.M42 * Value.M22)) + (matrix.M43 * Value.M32)) + (matrix.M44 * Value.M42);
                    float num2 = (((matrix.M41 * Value.M13) + (matrix.M42 * Value.M23)) + (matrix.M43 * Value.M33)) + (matrix.M44 * Value.M43);
                    float num = (((matrix.M41 * Value.M14) + (matrix.M42 * Value.M24)) + (matrix.M43 * Value.M34)) + (matrix.M44 * Value.M44);
                    Value.M11 = num16;
                    Value.M12 = num15;
                    Value.M13 = num14;
                    Value.M14 = num13;
                    Value.M21 = num12;
                    Value.M22 = num11;
                    Value.M23 = num10;
                    Value.M24 = num9;
                    Value.M31 = num8;
                    Value.M32 = num7;
                    Value.M33 = num6;
                    Value.M34 = num5;
                    Value.M41 = num4;
                    Value.M42 = num3;
                    Value.M43 = num2;
                    Value.M44 = num;
                }

                _index = ++_highpoint;
                _isIdentity = false;
            }

            _top++;
        }

        public void Pop()
        {
            if (checked(--_top) != 0)
            {
                if (_index != _stackIndex[_top])
                {
                    Value = _stack[_top];
                    _index = _stackIndex[_top];
                    _isIdentity = _stackIdentity[_top];
                }
            }
            else
            {
                _index = 1;
                _isIdentity = true;

                Value.M11 = 1;
                Value.M12 = 0;
                Value.M13 = 0;
                Value.M14 = 0;
                Value.M21 = 0;
                Value.M22 = 1;
                Value.M23 = 0;
                Value.M24 = 0;
                Value.M31 = 0;
                Value.M32 = 0;
                Value.M33 = 1;
                Value.M34 = 0;
                Value.M41 = 0;
                Value.M42 = 0;
                Value.M43 = 0;
                Value.M44 = 1;
            }
        }

        public override sealed void UpdateValue(int frame)
        {
        }
        
        public void Reset()
        {
            _top = 0;
            _index = 0;
            _isIdentity = true;
            Value = Matrix.Identity;
        }
    }
}
