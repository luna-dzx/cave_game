using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ThisIsTheActualProjectIPromise
{
    public class Map
    {
        private OpenSimplexNoise _simplex;
        private Texture2D[] _textures;
        private Block[,] _blocks;
        
        public Map(Texture2D[] blockTextures, int width, int height,float zoom)
        {
            _simplex = new OpenSimplexNoise();
            _blocks = new Block[height, width];
            _textures = blockTextures;

            float noiseValue;
            BlockType blockType;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    noiseValue = (float)_simplex.Evaluate(x / zoom, y / zoom);

                    if (noiseValue < 0.12)
                    {
                        blockType = BlockType.Stone;
                    }
                    else
                    {
                        blockType = BlockType.Wall;
                    }

                    _blocks[x, y] = new Block(blockType);
                }
            }
        }

        //draws the visible area of the map, what is visible is defined by the center, zoom, and screenres
        public void Draw(SpriteBatch spriteBatch, Camera cam)
        {
            //gets visible range
            (Vector2 a, Vector2 b) = cam.VisibleRange();
            //loop and render only blocks in visible range, keeping within maximum range of blocks
            for (int x = (int)MathF.Max(MathF.Floor(a.X),0.0f); x <= (int)MathF.Min(MathF.Floor(b.X),_blocks.GetLength(1)-1); x++)
            {
                for (int y = (int)MathF.Max(MathF.Floor(a.Y),0.0f); y <= (int)MathF.Min(MathF.Floor(b.Y),_blocks.GetLength(0)-1); y++)
                {
                    DrawBlock(spriteBatch, _blocks[x, y], cam, new Vector2(x, y));
                }
            }
        }
        
        private void DrawBlock(SpriteBatch spriteBatch, Block block, Camera cam, Vector2 blockPos)
        {
            cam.Draw(spriteBatch,_textures[(int)block.BlockType],blockPos,Vector2.One, Color.White);
        }

        
        public void Mine(Vector2 blockPos)
        {
            _blocks[(int)MathF.Floor(blockPos.X),(int)MathF.Floor(blockPos.Y)].BlockType = BlockType.Wall;
        }

        /*public Physics[] GetPhysics()
        {

        }*/
    }
}