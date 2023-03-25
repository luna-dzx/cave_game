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

        public Player(Vector2 pos, Vector2 size, Texture2D texture)
        {
            physics = new Physics(pos, size);
            physics.Gravity = true;
            _texture = texture;
        }

        public void Update(GameTime gameTime, Physics[] physicsObjects)
        {
            physics.Update(gameTime, physicsObjects, new Vector2(100, 100));
        }

        public void Draw(SpriteBatch spriteBatch, Camera cam)
        {
            if (physics.Contains(Vector2.One))
            {
                cam.Draw(spriteBatch, _texture, physics.Position, physics.Size, Color.Red);
            }
            else
            {
                cam.Draw(spriteBatch, _texture, physics.Position, physics.Size, Color.White);
            }

        }
    }
}

