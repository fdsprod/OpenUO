using OpenUO.Ultima;

namespace Client.Ultima
{
    public class Facet
    {
        private World _world;
        private Map _map;

        public Map Map
        {
            get { return _map; }
        }

        public Facet(InstallLocation install, World world, int fileIndex, int mapID, int width, int height)
        {
            _world = world;
            _map = new Map(install, fileIndex, mapID, width, height);
        }
    }
}
