﻿using NewEngine.Engine.components;
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
            List<Vector2> updatedChunks = new List<Vector2>();


            var minX = MathHelper.Clamp(posX - size, 0, int.MaxValue);
            var maxX = MathHelper.Clamp(posX + size, 0, int.MaxValue);

            var minY = MathHelper.Clamp(posY - size, 0, int.MaxValue);
            var maxY = MathHelper.Clamp(posY + size, 0, int.MaxValue);


            for (var x = minX; x < maxX; x++)
            {
                for (var y = minY; y < maxY; y++)
                {
                    var distance = new Vector3(posX, 0, posY).Distance(new Vector3(x, 0, y));
                    if (distance <= size)
                    {
                        int chunkX = (int)(x / (DrawableChunk.ChunkSizeX + 1));
                        int chunkY = (int)(y / (DrawableChunk.ChunkSizeY + 1));

                        float posInChunkX = (x % (DrawableChunk.ChunkSizeX + 1));
                        float posInChunkY = (y % (DrawableChunk.ChunkSizeY + 1));

                        float falloff = size - (float)distance;

                        if(!updatedChunks.Contains(new Vector2(chunkX, chunkY)))
                            updatedChunks.Add(new Vector2(chunkX, chunkY));

                        GetDrawableChunk(chunkX, chunkY)?.DrawOnTerrain(posInChunkX, posInChunkY, size, falloff * strength);
                    }
                }
            }

            foreach (var chunk in updatedChunks)
            {
                // TODO: Stitch chunks
            }


            foreach (var chunk in updatedChunks)
            {
                GetDrawableChunk((int)chunk.X, (int)chunk.Y).UpdateTerrain();
            }

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