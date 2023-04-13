using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CaveGame;

class Button
{
    public Texture2D Texture;
    public (float,float,float,float) RelativeRect;
    public bool Hovering;
    public Action OnClick;

    public Button(Texture2D texture, (float,float,float,float) relativeRect, Action onClick)
    {
        Texture = texture;
        RelativeRect = relativeRect;
        OnClick = onClick;
        Hovering = false;
    }

    public void Hover(bool hover = true) => Hovering = hover;

    public Rectangle GetRectangle(int screenWidth, int screenHeight) => new(
        (int)(RelativeRect.Item1 * screenWidth),
        (int)(RelativeRect.Item2 * screenHeight), 
        (int)(RelativeRect.Item3 * screenWidth),
        (int)(RelativeRect.Item4 * screenHeight));
}