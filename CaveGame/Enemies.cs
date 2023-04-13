using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CaveGame;

public abstract class Enemy
{
    public Physics Physics;
    protected Texture2D _texture;
    public float Speed;
    public float JumpForce;

    public Enemy(Vector2 pos, Vector2 size)
    {
        Physics = new Physics(pos, size);
    }

    public virtual void Update(GameTime gameTime, Physics[] testSubjects, int mapWidth, Map map)
    {
        Physics.Update(gameTime, testSubjects, mapWidth);
    }

    public virtual void Draw(SpriteBatch spriteBatch, Camera cam, GameTime gameTime)
    {
        cam.Draw(spriteBatch, _texture, Physics.Position, Physics.Size, Color.White);
    }
    
}

public class Roomba : Enemy
{
    public const string ImageName = "sky";
    private int direction;

    public Roomba(Texture2D texture, Vector2 pos) : base(pos, new Vector2(0.8f,0.8f))
    {
        _texture = texture;
        Physics.Velocity.X = 1f;
        direction = 1;
        Physics.Gravity = true;
        JumpForce = 6f;
    }
    
    public override void Update(GameTime gameTime, Physics[] testSubjects, int mapWidth, Map map)
    {
        float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
        int x = (int)(Physics.Position.X + (Physics.Velocity.X > 0f ? Physics.Size.X * 1.5f : -0.5f * Physics.Size.X) + Physics.Velocity.X * seconds);
        int y = (int)(Physics.Position.Y + Physics.Velocity.Y * seconds);
        
        if (map.GetBlockType(x,y) == BlockType.Wall)
        {
            if (map.GetBlockType(x, y+1) == BlockType.Wall)
            {
                if (map.GetBlockType(x, y+2) == BlockType.Wall) // big drop
                {
                    direction *= -1;
                }
                else { } // can drop
            }
            else { } // free walkable path
        }
        else
        {
            if (map.GetBlockType(x, y-1) == BlockType.Wall) // can jump
            {
                Physics.Velocity.Y = -JumpForce;
            }
            else // hit big wall
            {
                direction *= -1;
            }
        }

        Physics.Velocity.X = direction;

        base.Update(gameTime, testSubjects, mapWidth, map);
    }
    
    public override void Draw(SpriteBatch spriteBatch, Camera cam, GameTime gameTime)
    {
        base.Draw(spriteBatch,cam,gameTime);

    }
}