using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CaveGame;

public class Player
{
    public Physics Physics;
    private Texture2D _texture;
    public float Speed;

    private List<Orb> _orbs;
    private Texture2D _orbTexture;

    class Orb
    {
        public Vector2 EndPos;
        public float Opacity;

        public Orb(Vector2 endPos, float opacity)
        {
            EndPos = endPos;
            Opacity = opacity;
        }
    }

    public Player(GraphicsDevice graphics, Vector2 pos, Vector2 size, Texture2D texture, float speed)
    {
        Physics = new Physics(pos, size);
        Physics.Gravity = true;
        _texture = texture;
        Speed = speed;
        _orbs = new List<Orb>();
        _orbTexture = new Texture2D(graphics, 1, 1);
        _orbTexture.SetData(new[] { Color.CornflowerBlue });
    }

    public void Update(GameTime gameTime, Physics[] physicsObjects)
    {
        Physics.Update(gameTime, physicsObjects, new Vector2(100, 100));
    }

    public void SpawnOrb(Vector2 clickPos)
    {
        _orbs.Add(new Orb(clickPos - new Vector2(0f,1f), 1f));
    }

    public void Draw(SpriteBatch spriteBatch, Camera cam, GameTime gameTime)
    {
        cam.Draw(spriteBatch, _texture, Physics.Position, Physics.Size, Color.White);
        float time = (float)gameTime.ElapsedGameTime.TotalSeconds;

        for (int o = 0; o < _orbs.Count; o++)
        {
            Vector2 lineDirection = _orbs[o].EndPos - Physics.Position + Physics.Size / 2;
            float lineLength = lineDirection.Length();
            Vector2 perpendicular = new Vector2(-lineDirection.Y, lineDirection.X);
            perpendicular.Normalize();

            Random rand = new Random();

            for (float i = 0f; i < 1f; i += 0.05f / lineLength)
            {
                Vector2 position = Physics.Position + Physics.Size / 2 + lineDirection * i + ((float)rand.NextDouble() - 0.5f) * 0.2f * perpendicular;

                cam.Draw(spriteBatch, _orbTexture, position, Vector2.One / 50f, Color.White * _orbs[o].Opacity);
            }

            _orbs[o].Opacity -= time;
            if (_orbs[o].Opacity < 0f)
            {
                _orbs.RemoveAt(o);
                o--;
            }
        }

    }
}