using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ThisIsTheActualProjectIPromise
{
    //used for drawing objects in the game, lets game use different coords than screen coords
    public class Camera
    {
        //location in game coord at center of screen
        public Vector2 _pos;
        //target position that the camera will move towards
        public Vector2 _target;
        //speed at which the camera follows the target
        public float _speed;
        //resolution of screen being rendered too
        private Vector2 _screenResolution;
        //the dimensions of one in-game unit of measurement in pixels
        public Vector2 UnitScale;

        public Camera(Vector2 unitScale, Vector2 screenResolution, Vector2 pos,float speed)
        {
            UnitScale = unitScale;
            _screenResolution = screenResolution;
            _pos = pos;
            _target = pos;
            _speed = speed;
        }

        public void Update(GameTime gameTime, int mapWidth)
        {
            Vector2 difference = _target - _pos;
            //increase pos to move towards target at set speed
            _pos += difference * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            float screenOffset = _screenResolution.X / (2f * UnitScale.X);
            if (_pos.X - screenOffset < 0f) _pos.X = screenOffset;
            if (_pos.X + screenOffset > mapWidth) _pos.X = mapWidth - screenOffset;
        }

        public Vector2 GetPlayerScreenPos(Vector2 pos)
        {
            return ((pos * UnitScale) + _screenResolution / 2);
        }

        //takes texture & measurements in in-game units, draws on screen
        public void Draw(SpriteBatch spriteBatch,Texture2D texture,Vector2 pos,Vector2 dimensions,Color color)
        {
            spriteBatch.Draw(texture,
                //offset coords then scale to screen coords
                new Rectangle((((pos - _pos) * UnitScale) + _screenResolution / 2).ToPoint(),
                    (dimensions* UnitScale).ToPoint()), color);
        }

        //takes screen coords, return the index of the block at that point
        public Vector2 ScreenToGameUnits(Vector2 screenPos)
        {
            //get vector from screen center to screenPos
            Vector2 toScreenPos = _screenResolution / 2 - screenPos;
            //scale vector to game units 
            toScreenPos /= UnitScale;
            //take _pos from vector
            return _pos - toScreenPos;
        }

        //returns the range of visible units
        public (Vector2,Vector2) VisibleRange()
        {
            //get the dimensions of the screen in game units and halfs it
            Vector2 unitResolution = (_screenResolution/ UnitScale) /2.0f;
            //returns pos-block res and pos+block res as that range covers the entire screen in game units
            return (_pos - unitResolution,_pos + unitResolution);
        }
    }
}