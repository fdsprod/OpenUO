using OpenUO.Core.Patterns;
using OpenUO.Ultima.Adapters;

using SiliconStudio.Paradox.Graphics;

namespace OpenUO.Ultima.Paradox3d.Adapters
{
    public abstract class Paradox3dStorageAdapterBase : StorageAdapterBase
    {
        protected readonly GraphicsDevice GraphicsDevice;

        protected Paradox3dStorageAdapterBase(IContainer container)
        {
            var graphicsDeviceService = container.Resolve<IGraphicsDeviceService>();

            GraphicsDevice = graphicsDeviceService.GraphicsDevice;
        }
    }
}