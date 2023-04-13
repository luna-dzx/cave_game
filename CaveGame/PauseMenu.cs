using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CaveGame;

public class PauseMenu : GameState
{
    private Dictionary<string, Button> _buttons;

    private (int, int) _screenSize;

    // button functions
    public void ButtonResume()
    {
        Game1.InitializeCaveGame = false;
        Game1.ResetCaveGame = false;
        Exit("game");
    }

    public void ButtonSave()
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter handler = new BinaryWriter(stream);
        
        handler.Write(Game1.LoadedSeed);
        handler.Write(Game1.LoadedPosition.X);
        handler.Write(Game1.LoadedPosition.Y);
        
        handler.Close();
        File.WriteAllBytes("save-data.balls", stream.ToArray());
    }
    public void ButtonExit() => Exit("title");
    

    public override void LoadContent(GraphicsDevice graphics, ContentManager content)
    {
        if (Loaded) return;
        
        _buttons = new Dictionary<string, Button>();
        
        _buttons.Add("New Game", new Button(
            content.Load<Texture2D>("ballsoodman"),
            (0.25f,0.1f,0.5f,0.2f), ButtonResume)
        );
        _buttons.Add("Continue", new Button(
            content.Load<Texture2D>("stonetxt"),
            (0.25f,0.4f,0.5f,0.2f), ButtonSave)
        );
        _buttons.Add("Exit", new Button(
            content.Load<Texture2D>("nigel"),
            (0.25f,0.7f,0.5f,0.2f), ButtonExit)
        );
        
        base.LoadContent(graphics, content);
    }


    private MouseState _lastMouseState;

    public bool Clicked()
    {
        var mouseState = Mouse.GetState();
        return mouseState.LeftButton == ButtonState.Pressed && _lastMouseState.LeftButton != ButtonState.Pressed;
    }

    public override void Update(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        var mouseState = Mouse.GetState();

        var (w, h) = _screenSize;
        
        
        foreach (var button in _buttons.Values)
        {
            button.Hover(button.GetRectangle(w, h).Contains(mouseState.X, mouseState.Y));
            if (button.Hovering && Clicked()) button.OnClick();
        }

        _lastMouseState = mouseState;
    }

    public override void Draw(GraphicsDeviceManager graphicsManager, GameTime gameTime, SpriteBatch spriteBatch)
    {
        int w = graphicsManager.PreferredBackBufferWidth;
        int h = graphicsManager.PreferredBackBufferHeight;

        _screenSize = (w, h);

        foreach (var button in _buttons.Values)
        {
            var rect = button.GetRectangle(w, h);
            spriteBatch.Draw(button.Texture, rect, Color.White * (button.Hovering? 1f: 0.5f));
        }
    }
}