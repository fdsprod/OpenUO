using OpenUO.Core.Patterns;

using SiliconStudio.Core;
using SiliconStudio.Paradox.Games;

namespace ParadoxUO.Net
{
    public abstract class ClientSystemBase : GameSystemBase
    {
        protected ClientSystemBase(IContainer container) : base(container.Resolve<IServiceRegistry>())
        {
        }
    }
}