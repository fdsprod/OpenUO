using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenUO.Ultima
{
    public class TileComparer : IComparer<Tile>
    {
        private TileData _tileData;

        public TileComparer(TileData tileData)
        {
            _tileData = tileData;
        }

        public int Compare(Tile x, Tile y)
        {
            if (x._z != y._z)
            {
                return x._z.CompareTo(y._z);
            }

            ItemData xData = _tileData.ItemTable[x._id & 0x3FFF];
            ItemData yData = _tileData.ItemTable[y._id & 0x3FFF];

            if (xData._height != yData._height)
            {
                return xData._height.CompareTo(yData._height);
            }

            if (xData._height != yData._height)
            {
                return xData._height.CompareTo(yData._height);
            }

            if (xData.Background != yData.Background)
            {
                xData.Background.CompareTo(yData.Background);
            }

            return 0;
        }
    }
}
