using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CaveGame;

//used for drawing objects in the game, lets game use different coords than screen coords
public class Camera
{
    //location in game coord at center of screen
    public Vector2 Pos;
    //target position that the camera will move towards
    public Vector2 Target;
    //speed at which the camera follows the target
    public float Speed;
    //resolution of screen being rendered too
    private Vector2 _screenResolution;
    //the dimensions of one in-game unit of measurement in pixels
    public Vector2 UnitScale;

    public Camera(Vector2 unitScale, Vector2 screenResolution, Vector2 pos, float speed)
    {
        UnitScale = unitScale;
        _screenResolution = screenResolution;
        Pos = pos;
        Target = pos;
        Speed = speed;
    }

    public void Update(GameTime gameTime, int mapWidth)
    {
        Vector2 difference = Target - Pos;
        //increase pos to move towards target at set speed
        Pos += difference * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        float screenOffset = _screenResolution.X / (2f * UnitScale.X);
        if (Pos.X - screenOffset < 0f) Pos.X = screenOffset;
        if (Pos.X + screenOffset > mapWidth) Pos.X = mapWidth - screenOffset;
    }

    public Vector2 GetPlayerScreenPos(Vector2 pos) => pos * UnitScale + _screenResolution / 2;
    

    //takes texture & measurements in in-game units, draws on screen
    public void Draw(SpriteBatch spriteBatch,Texture2D texture,Vector2 pos,Vector2 dimensions,Color color)
    {
        spriteBatch.Draw(texture,
            //offset coords then scale to screen coords
            new Rectangle((((pos - Pos) * UnitScale) + _screenResolution / 2).ToPoint(),
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
        return Pos - toScreenPos;
    }

    //returns the range of visible units
    public (Vector2,Vector2) VisibleRange()
    {
        //get the dimensions of the screen in game units and halves it
        Vector2 unitResolution = (_screenResolution/ UnitScale) /2.0f;
        //returns pos-block res and pos+block res as that range covers the entire screen in game units
        return (Pos - unitResolution,Pos + unitResolution);
    }
}