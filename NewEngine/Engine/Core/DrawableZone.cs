using NewEngine.Engine.components;
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

        public int ZoneSizeX = 50;
        public int ZoneSizeY = 50;

        public Dictionary<Vector2, DrawableChunk> cachedChunks;

        public DrawableZone(int zoneId)
        {
            cachedChunks = new Dictionary<Vector2, DrawableChunk>();
            ZoneId = zoneId;
        }

        public DrawableChunk GetDrawableChunk(int x, int y) {
            if (cachedChunks.ContainsKey(new Vector2(x, y)))
            {
                return cachedChunks[new Vector2(x, y)];
            }
            else
            {
                var drawableChunk = new DrawableChunk(ZoneId, x, y);
                if (drawableChunk.Loaded == true)
                {
                    cachedChunks.Add(new Vector2(x, y), drawableChunk);

                    return cachedChunks[new Vector2(x, y)];
                }

                return null;
            }
        }

        public void Draw(float originX, float originY, int range, ICoreEngine coreEngine)
        {
            for (int x = 0; x < ZoneSizeX; x++)
            {
                for (int y = 0; y < ZoneSizeY; y++)
                {
                    if (new Vector3(x, 0, y).Distance(new Vector3(originX, 0, originY)) <= range)
                    {
                        GetDrawableChunk(x, y)?.Draw(coreEngine);
                    }
                }
            }

        }


        public void DrawOnTerrain(DrawBrush brush, float posX, float posY, int size, float strength)
        {
            int x = (int)(posX / DrawableChunk.ChunkSizeX);
            int y = (int)(posY / DrawableChunk.ChunkSizeY);

            float posInChunkX = (posX % DrawableChunk.ChunkSizeX);
            float posInChunkY = (posY % DrawableChunk.ChunkSizeY);


            GetDrawableChunk(x, y)?.DrawOnTerrain(brush, posInChunkX, posInChunkY, size, strength);
        }

        public void Save()
        {
            foreach (var chunk in cachedChunks)
            {
                chunk.Value.Save();
            }
        }

        public void AddObject(GameObject gObj)
        {
            int x = (int)(gObj.Transform.Position.X / DrawableChunk.ChunkSizeX);
            int y = (int)(gObj.Transform.Position.Z / DrawableChunk.ChunkSizeY);

            GetDrawableChunk(x, y)?.AddObject(gObj);
        }

        public static void CreateNewZone(int zoneId, int width, int height)
        {
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
