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

using SharpDX;
using SharpDX.Direct3D9;

namespace Client.Graphics
{
    public abstract class TextureBase : DeviceResource
    {
        private Texture _texture;
        private Vector2 _size;
        private int _width;
        private int _height;
        private int _levelCount;
        private Usage _usage;
        private Format _format;
        private Pool _pool;

        protected bool RecreateOnReset = true;

        public Vector2 Size
        {
            get { return _size; }
        }

        public Format Format
        {
            get { return _format; }
        }

        internal override bool InUse
        {
            get { return _texture != null; }
        }

        internal override bool IsDisposed
        {
            get { return _texture == null; }
        }

        internal override ResourceType ResourceType
        {
            get { return ResourceType.Texture; }
        }

        protected TextureBase(DeviceContext context, int width, int height, int levelCount, Usage usage, Format format, Pool pool)
            : base(context)
        {
            _size = new Vector2(width, height);
            _width = width;
            _height = height;
            _levelCount = levelCount;
            _usage = usage;
            _format = format;
            _pool = pool;

            _texture = new Texture(context, width, height, levelCount, usage, format, pool);
#if DEBUG
            Context.PerformanceMonitor.IncreaseLifetimeCounter(LifetimeCounters.TextureCount);
#endif
        }

        public TextureBase(DeviceContext context, Texture texture)
            : base(context)
        {
            _texture = texture;

            SurfaceDescription desc = _texture.GetLevelDescription(0);

            _width = desc.Width;
            _height = desc.Height;
            _size = new Vector2(_width, _height);
            _levelCount = _texture.LevelCount;
            _usage = desc.Usage;
            _format = desc.Format;
            _pool = desc.Pool;

#if DEBUG
            Context.PerformanceMonitor.IncreaseLifetimeCounter(LifetimeCounters.TextureCount);
#endif
        }

        protected override void OnContextLost(DeviceContext context)
        {
            if (_texture != null && _pool != Pool.Managed)
            {
                _texture.Dispose();
                _texture = null;
#if DEBUG
                Context.PerformanceMonitor.DecreaseLifetimeCounter(LifetimeCounters.TextureCount);
#endif
            }
        }

        protected override void OnContextReset(DeviceContext context)
        {
            base.OnContextReset(context);

            if (_pool != Pool.Managed && RecreateOnReset)
            {
                _texture = new Texture(context, _width, _height, _levelCount, _usage, _format, _pool);
#if DEBUG
                Context.PerformanceMonitor.DecreaseLifetimeCounter(LifetimeCounters.TextureCount);
#endif
            }
        }

        internal override int GetAllocatedDeviceBytes()
        {
            int byteCount = 0;

            //TODO: Figure this shit out

            return byteCount;
        }

        internal override int GetAllocatedManagedBytes()
        {
            int byteCount = 0;

            //TODO: Figure this shit out

            return byteCount;
        }

        public Result AddDirtyRectangle()
        {
            return _texture.AddDirtyRectangle();
        }

        public Result AddDirtyRectangle(Rectangle dirtyRectRef)
        {
            return _texture.AddDirtyRectangle(dirtyRectRef);
        }

        public SurfaceDescription GetLevelDescription(int level)
        {
            return _texture.GetLevelDescription(level);
        }

        public Surface GetSurfaceLevel(int level)
        {
            return _texture.GetSurfaceLevel(level);
        }

        public DataRectangle LockRectangle(int level, LockFlags flags)
        {
            return _texture.LockRectangle(level, flags);
        }

        public DataRectangle LockRectangle(int level, LockFlags flags, out DataStream stream)
        {
            return _texture.LockRectangle(level, flags, out stream);
        }

        public DataRectangle LockRectangle(int level, Rectangle rectangle, LockFlags flags)
        {
            return _texture.LockRectangle(level, rectangle, flags);
        }

        public DataRectangle LockRectangle(int level, Rectangle rectangle, LockFlags flags, out DataStream stream)
        {
            return _texture.LockRectangle(level, rectangle, flags, out stream);
        }

        public Result UnlockRectangle(int level)
        {
            return _texture.UnlockRectangle(level);
        }

        public Result Fill(Fill2DCallback callback)
        {
            return _texture.Fill(callback);
        }

        public Result Fill(TextureShader shader)
        {
            return _texture.Fill(shader);
        }

        public static implicit operator Texture(TextureBase a)
        {
            if (a == null)
                return null;

            return a._texture;
        }

        internal override void WarmOverride(DrawState state)
        {

        }
    }
}
