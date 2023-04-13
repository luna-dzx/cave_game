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
        private List<Block[]> _blocks;
        private int generationSize = 0;
        private int _width;
        private float _zoom;


        /// <param name="value">noise value between 0 and 1</param>
        public BlockType GetBlockType(float value)
        {
            if (value < 0.3)
            {
               return BlockType.Wall;
            }
            if (value < 0.6)
            {
                return BlockType.Stone;
            }

            return BlockType.Nigel;
        }


        public Map(Texture2D[] blockTextures, int width, int height, float zoom)
        {
            _simplex = new OpenSimplexNoise();
            _blocks = new List<Block[]>();//new Block[height, width];
            _textures = blockTextures;
            _width = width;
            _zoom = zoom;

            float noiseValue;
            BlockType blockType;

            for (int y = 0; y < height; y++)
            {
                Block[] currentBlocks = new Block[width];
                for (int x = 0; x < width; x++)
                {
                    noiseValue = (float)_simplex.Evaluate(x / zoom, y / zoom);
                    currentBlocks[x] = new Block(GetBlockType(noiseValue));
                }

                _blocks.Add(currentBlocks);
            }

            generationSize = height - 1;
        }

        //draws the visible area of the map, what is visible is defined by the center, zoom, and screenres
        public void Draw(SpriteBatch spriteBatch, Camera cam)
        {
            //gets visible range
            (Vector2 a, Vector2 b) = cam.VisibleRange();
            //loop and render only blocks in visible range, keeping within maximum range of blocks
            for (int x = (int)MathF.Max(MathF.Floor(a.X), 0.0f); x <= (int)MathF.Min(MathF.Floor(b.X), _blocks[0].Length - 1); x++)
            {
                for (int y = Math.Max((int)a.Y,0); y <= (int)(b.Y); y++)
                {
                    DrawBlock(spriteBatch, _blocks[y][x], cam, new Vector2(x, y));
                }
            }
        }

        private void DrawBlock(SpriteBatch spriteBatch, Block block, Camera cam, Vector2 blockPos)
        {
            cam.Draw(spriteBatch, _textures[(int)block.BlockType], blockPos, Vector2.One, Color.White);
        }

        private bool IsWithinMap(Vector2 pos) => pos.X >= 0 && pos.Y > 0.0 && pos.X <= _blocks[0].Length;
        

        public Vector2 Mine(Vector2 blockPos) // returns the position of the mined block
        {
            if (!IsWithinMap(blockPos)) return new Vector2(-1,-1);
            int y = (int)MathF.Floor(blockPos.Y);
            int x = (int)MathF.Floor(blockPos.X);

            bool blockBroken = (_blocks[y][x].BlockType != BlockType.Wall);

            _blocks[y][x].BlockType = BlockType.Wall;

            return blockBroken ? new Vector2(x, y) : new Vector2(-1, -1);
        }

        //returns physics objects of all blocks that can be collided with, within a given range
        public Physics[] GetPhysicsInRange(Vector2 start, Vector2 end)
        {
            List<Physics> rects = new List<Physics>();
            for (int x = (int)MathF.Max(MathF.Floor(start.X), 0.0f); x <= (int)MathF.Min(MathF.Floor(end.X), _blocks[0].Length - 1); x++)
            {
                for (int y = Math.Max((int)(start.Y),0); y <= (int)(end.Y); y++)
                {
                    //turn into rectangle if collidable
                    if (_blocks[y][x].BlockType != BlockType.Wall)
                    {
                        rects.Add(new Physics(new Vector2(x, y), new Vector2(1, 1)));
                    }
                }
            }
            return rects.ToArray();
        }

        public void GenerateRows(Vector2 playerPosition, Camera cam)
        {
            (Vector2 a, Vector2 b) = cam.VisibleRange();
            int currentHeight = (int)Math.Ceiling(playerPosition.Y + b.Y);
            if (currentHeight > generationSize)
            {

                float noiseValue;
                BlockType blockType;

                // loop through new area to generate
                for (int y = generationSize; y < currentHeight; y++)
                {
                    Block[] currentBlocks = new Block[_width];
                    for (int x = 0; x < _width; x++)
                    {
                        noiseValue = (float)_simplex.Evaluate(x / _zoom, y / _zoom);
                        currentBlocks[x] = new Block(GetBlockType(noiseValue));
                    }

                    _blocks.Add(currentBlocks);
                }

                generationSize = currentHeight;
            }
        }

    }
}
