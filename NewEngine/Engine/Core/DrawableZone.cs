using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewEngine.Engine.Core
{
    // will draw the chunks around a certain X, Y
    public class DrawableZone
    {
        public int ZoneId { get; set; }

        public Dictionary<Vector2, DrawableChunk> cachedChunks;

        public DrawableZone(int zoneId)
        {
            cachedChunks = new Dictionary<Vector2, DrawableChunk>();
            ZoneId = zoneId;
        }

        public void Draw(int x, int y, int range, ICoreEngine coreEngine) {
            if (cachedChunks.ContainsKey(new Vector2(x, y)))
            {
                cachedChunks[new Vector2(x, y)].Draw(coreEngine);
            }
            else {
                var drawableChunk = new DrawableChunk(ZoneId, x, y);
                cachedChunks.Add(new Vector2(x, y), drawableChunk);
                cachedChunks[new Vector2(x, y)].Draw(coreEngine);
            }
        }

        public static void CreateNewZone(int zoneId, int width, int height) {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    DrawableChunk.CreateNew(zoneId, x, y);
                }
            }
        }
    }
}
