using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ThisIsTheActualProjectIPromise
{
    public class Player
    {
        public Physics physics;
        private Texture2D _texture;
        public float Speed;

        private List<Orb> orbs;
        Texture2D orbTexture;

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
            physics = new Physics(pos, size);
            physics.Gravity = true;
            _texture = texture;
            Speed = speed;
            orbs = new List<Orb>();
            orbTexture = new Texture2D(graphics, 1, 1);
            orbTexture.SetData(new Color[] { Color.CornflowerBlue });
        }

        public void Update(GameTime gameTime, Physics[] physicsObjects)
        {
            physics.Update(gameTime, physicsObjects, new Vector2(100, 100));
        }

        public void SpawnOrb(Vector2 clickPos)
        {
            orbs.Add(new Orb(clickPos - new Vector2(0f,1f), 1f));
        }

        public void Draw(SpriteBatch spriteBatch, Camera cam, GameTime gameTime)
        {
            cam.Draw(spriteBatch, _texture, physics.Position, physics.Size, Color.White);
            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int o = 0; o < orbs.Count; o++)
            {
                Vector2 lineDirection = orbs[o].EndPos - physics.Position + physics.Size / 2;
                float lineLength = lineDirection.Length();
                Vector2 perpendicular = new Vector2(-lineDirection.Y, lineDirection.X);
                perpendicular.Normalize();

                Random rand = new Random();

                for (float i = 0f; i < 1f; i += 0.05f / lineLength)
                {
                    Vector2 position = physics.Position + physics.Size / 2 + lineDirection * i + ((float)rand.NextDouble() - 0.5f) * 0.2f * perpendicular;

                    cam.Draw(spriteBatch, orbTexture, position, Vector2.One / 50f, Color.White * orbs[o].Opacity);
                }

                orbs[o].Opacity -= time;
                if (orbs[o].Opacity < 0f)
                {
                    orbs.RemoveAt(o);
                    o--;
                }
            }

        }
    }
}

