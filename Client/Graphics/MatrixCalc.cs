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


namespace Client.Graphics
{
    enum MatrixOp
    {
        Multiply, Inverse, Transpose
    }

    sealed class MatrixCalc : MatrixSource
    {
        private MatrixSource provider;
        private int providerIndex;
        private MatrixOp op;
        private MatrixSource source;
        private int sourceIndex;
        private int frame;
#if DEBUG
        DrawState state;
#endif

        public MatrixCalc(MatrixOp op, MatrixSource provider, MatrixSource source, DrawState state)
        {
#if DEBUG
            this.state = state;
#endif
            this.op = op;
            this.provider = provider;
            this.source = source;
        }

        public override sealed void UpdateValue(int frame)
        {
            //if (frame != this.frame)
            //{
                this.frame = frame;
                provider.UpdateValue(frame);

                if (op == MatrixOp.Multiply)
                {
                    source.UpdateValue(frame);

                    if (provider.Changed(ref providerIndex) ||
                        source.Changed(ref sourceIndex))
                    {
#if DEBUG
                        state.Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.ShaderConstantMatrixMultiplyCalculateCount);
#endif
                        Value.M11 = (((provider.Value.M11 * source.Value.M11) + (provider.Value.M12 * source.Value.M21)) + (provider.Value.M13 * source.Value.M31)) + (provider.Value.M14 * source.Value.M41);
                        Value.M12 = (((provider.Value.M11 * source.Value.M12) + (provider.Value.M12 * source.Value.M22)) + (provider.Value.M13 * source.Value.M32)) + (provider.Value.M14 * source.Value.M42);
                        Value.M13 = (((provider.Value.M11 * source.Value.M13) + (provider.Value.M12 * source.Value.M23)) + (provider.Value.M13 * source.Value.M33)) + (provider.Value.M14 * source.Value.M43);
                        Value.M14 = (((provider.Value.M11 * source.Value.M14) + (provider.Value.M12 * source.Value.M24)) + (provider.Value.M13 * source.Value.M34)) + (provider.Value.M14 * source.Value.M44);
                        Value.M21 = (((provider.Value.M21 * source.Value.M11) + (provider.Value.M22 * source.Value.M21)) + (provider.Value.M23 * source.Value.M31)) + (provider.Value.M24 * source.Value.M41);
                        Value.M22 = (((provider.Value.M21 * source.Value.M12) + (provider.Value.M22 * source.Value.M22)) + (provider.Value.M23 * source.Value.M32)) + (provider.Value.M24 * source.Value.M42);
                        Value.M23 = (((provider.Value.M21 * source.Value.M13) + (provider.Value.M22 * source.Value.M23)) + (provider.Value.M23 * source.Value.M33)) + (provider.Value.M24 * source.Value.M43);
                        Value.M24 = (((provider.Value.M21 * source.Value.M14) + (provider.Value.M22 * source.Value.M24)) + (provider.Value.M23 * source.Value.M34)) + (provider.Value.M24 * source.Value.M44);
                        Value.M31 = (((provider.Value.M31 * source.Value.M11) + (provider.Value.M32 * source.Value.M21)) + (provider.Value.M33 * source.Value.M31)) + (provider.Value.M34 * source.Value.M41);
                        Value.M32 = (((provider.Value.M31 * source.Value.M12) + (provider.Value.M32 * source.Value.M22)) + (provider.Value.M33 * source.Value.M32)) + (provider.Value.M34 * source.Value.M42);
                        Value.M33 = (((provider.Value.M31 * source.Value.M13) + (provider.Value.M32 * source.Value.M23)) + (provider.Value.M33 * source.Value.M33)) + (provider.Value.M34 * source.Value.M43);
                        Value.M34 = (((provider.Value.M31 * source.Value.M14) + (provider.Value.M32 * source.Value.M24)) + (provider.Value.M33 * source.Value.M34)) + (provider.Value.M34 * source.Value.M44);
                        Value.M41 = (((provider.Value.M41 * source.Value.M11) + (provider.Value.M42 * source.Value.M21)) + (provider.Value.M43 * source.Value.M31)) + (provider.Value.M44 * source.Value.M41);
                        Value.M42 = (((provider.Value.M41 * source.Value.M12) + (provider.Value.M42 * source.Value.M22)) + (provider.Value.M43 * source.Value.M32)) + (provider.Value.M44 * source.Value.M42);
                        Value.M43 = (((provider.Value.M41 * source.Value.M13) + (provider.Value.M42 * source.Value.M23)) + (provider.Value.M43 * source.Value.M33)) + (provider.Value.M44 * source.Value.M43);
                        Value.M44 = (((provider.Value.M41 * source.Value.M14) + (provider.Value.M42 * source.Value.M24)) + (provider.Value.M43 * source.Value.M34)) + (provider.Value.M44 * source.Value.M44);

                        _index++;
                    }
                }
                else
                {
                    if (provider.Changed(ref providerIndex))
                    {
                        if (op == MatrixOp.Transpose)
                        {
#if DEBUG
                        state.Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.ShaderConstantMatrixTransposeCalculateCount);
#endif
                            this.Value.M11 = provider.Value.M11;
                            this.Value.M12 = provider.Value.M21;
                            this.Value.M13 = provider.Value.M31;
                            this.Value.M14 = provider.Value.M41;
                            this.Value.M21 = provider.Value.M12;
                            this.Value.M22 = provider.Value.M22;
                            this.Value.M23 = provider.Value.M32;
                            this.Value.M24 = provider.Value.M42;
                            this.Value.M31 = provider.Value.M13;
                            this.Value.M32 = provider.Value.M23;
                            this.Value.M33 = provider.Value.M33;
                            this.Value.M34 = provider.Value.M43;
                            this.Value.M41 = provider.Value.M14;
                            this.Value.M42 = provider.Value.M24;
                            this.Value.M43 = provider.Value.M34;
                            this.Value.M44 = provider.Value.M44;

                            _index++;
                        }
                        else
                        {
                            //invert
#if DEBUG
                            state.Context.PerformanceMonitor.IncreaseCounter(DeviceCounters.ShaderConstantMatrixInverseCalculateCount);
#endif
                            float num23 = (provider.Value.M33 * provider.Value.M44) - (provider.Value.M34 * provider.Value.M43);
                            float num22 = (provider.Value.M32 * provider.Value.M44) - (provider.Value.M34 * provider.Value.M42);
                            float num21 = (provider.Value.M32 * provider.Value.M43) - (provider.Value.M33 * provider.Value.M42);
                            float num20 = (provider.Value.M31 * provider.Value.M44) - (provider.Value.M34 * provider.Value.M41);
                            float num19 = (provider.Value.M31 * provider.Value.M43) - (provider.Value.M33 * provider.Value.M41);
                            float num18 = (provider.Value.M31 * provider.Value.M42) - (provider.Value.M32 * provider.Value.M41);
                            float num39 = ((provider.Value.M22 * num23) - (provider.Value.M23 * num22)) + (provider.Value.M24 * num21);
                            float num38 = -(((provider.Value.M21 * num23) - (provider.Value.M23 * num20)) + (provider.Value.M24 * num19));
                            float num37 = ((provider.Value.M21 * num22) - (provider.Value.M22 * num20)) + (provider.Value.M24 * num18);
                            float num36 = -(((provider.Value.M21 * num21) - (provider.Value.M22 * num19)) + (provider.Value.M23 * num18));
                            float num = 1f / ((((provider.Value.M11 * num39) + (provider.Value.M12 * num38)) + (provider.Value.M13 * num37)) + (provider.Value.M14 * num36));
                            this.Value.M11 = num39 * num;
                            this.Value.M21 = num38 * num;
                            this.Value.M31 = num37 * num;
                            this.Value.M41 = num36 * num;
                            this.Value.M12 = -(((provider.Value.M12 * num23) - (provider.Value.M13 * num22)) + (provider.Value.M14 * num21)) * num;
                            this.Value.M22 = (((provider.Value.M11 * num23) - (provider.Value.M13 * num20)) + (provider.Value.M14 * num19)) * num;
                            this.Value.M32 = -(((provider.Value.M11 * num22) - (provider.Value.M12 * num20)) + (provider.Value.M14 * num18)) * num;
                            this.Value.M42 = (((provider.Value.M11 * num21) - (provider.Value.M12 * num19)) + (provider.Value.M13 * num18)) * num;
                            float num35 = (provider.Value.M23 * provider.Value.M44) - (provider.Value.M24 * provider.Value.M43);
                            float num34 = (provider.Value.M22 * provider.Value.M44) - (provider.Value.M24 * provider.Value.M42);
                            float num33 = (provider.Value.M22 * provider.Value.M43) - (provider.Value.M23 * provider.Value.M42);
                            float num32 = (provider.Value.M21 * provider.Value.M44) - (provider.Value.M24 * provider.Value.M41);
                            float num31 = (provider.Value.M21 * provider.Value.M43) - (provider.Value.M23 * provider.Value.M41);
                            float num30 = (provider.Value.M21 * provider.Value.M42) - (provider.Value.M22 * provider.Value.M41);
                            this.Value.M13 = (((provider.Value.M12 * num35) - (provider.Value.M13 * num34)) + (provider.Value.M14 * num33)) * num;
                            this.Value.M23 = -(((provider.Value.M11 * num35) - (provider.Value.M13 * num32)) + (provider.Value.M14 * num31)) * num;
                            this.Value.M33 = (((provider.Value.M11 * num34) - (provider.Value.M12 * num32)) + (provider.Value.M14 * num30)) * num;
                            this.Value.M43 = -(((provider.Value.M11 * num33) - (provider.Value.M12 * num31)) + (provider.Value.M13 * num30)) * num;
                            float num29 = (provider.Value.M23 * provider.Value.M34) - (provider.Value.M24 * provider.Value.M33);
                            float num28 = (provider.Value.M22 * provider.Value.M34) - (provider.Value.M24 * provider.Value.M32);
                            float num27 = (provider.Value.M22 * provider.Value.M33) - (provider.Value.M23 * provider.Value.M32);
                            float num26 = (provider.Value.M21 * provider.Value.M34) - (provider.Value.M24 * provider.Value.M31);
                            float num25 = (provider.Value.M21 * provider.Value.M33) - (provider.Value.M23 * provider.Value.M31);
                            float num24 = (provider.Value.M21 * provider.Value.M32) - (provider.Value.M22 * provider.Value.M31);
                            this.Value.M14 = -(((provider.Value.M12 * num29) - (provider.Value.M13 * num28)) + (provider.Value.M14 * num27)) * num;
                            this.Value.M24 = (((provider.Value.M11 * num29) - (provider.Value.M13 * num26)) + (provider.Value.M14 * num25)) * num;
                            this.Value.M34 = -(((provider.Value.M11 * num28) - (provider.Value.M12 * num26)) + (provider.Value.M14 * num24)) * num;
                            this.Value.M44 = (((provider.Value.M11 * num27) - (provider.Value.M12 * num25)) + (provider.Value.M13 * num24)) * num;

                            _index++;
                        }
                    }
                }
            //}
        }
    }
}
