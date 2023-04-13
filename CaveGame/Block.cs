using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ThisIsTheActualProjectIPromise
{
    public enum BlockType
    {
        Wall = 0,
        Stone = 1,
        Nigel = 2,
    };
    
    struct Block
    {
        public BlockType BlockType;
        public int Health;

        public Block(BlockType block)
        {
            BlockType = block;
            Health = 100;
            
        }
    }

}
