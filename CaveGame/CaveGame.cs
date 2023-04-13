using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CaveGame;

public class CaveGame: GameState
{
    private Camera _cam;
    private Map _map;
    private Player _player;

    const int MapWidth = 30;
    const int MapHeight = 30;

    public override void LoadContent(GraphicsDevice graphics, ContentManager content)
    {
        //if (Loaded) return;
        
        Texture2D[] textures = {
            content.Load<Texture2D>("stonebgtxt"),
            content.Load<Texture2D>("stonetxt"),
            content.Load<Texture2D>("nigel")
        };
        _map = new Map(textures, MapWidth, MapHeight, 5.0f);

        if (!Game1.ResetCaveGame)
        {
            Console.WriteLine("Loady File");
        }
        
        _player = new Player(graphics, new Vector2(2, 1), new Vector2(0.75f, 1.69f), content.Load<Texture2D>("MycJoe1"), 5f);
        _cam = new Camera(new Vector2(50, 50), new Vector2(800, 600), _player.Physics.Position, 3.5f);
        
        //base.LoadContent(graphics, content);
    }

    public override void Update(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        var mouseState = Mouse.GetState();

        //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        //    Exit();

        if (keyboardState.IsKeyDown(Keys.Escape)) Exit("title");

        Vector2 tempVelocity = Vector2.Zero;

        if (keyboardState.IsKeyDown(Keys.W))
        {
            tempVelocity += new Vector2(0, -1f);
        }
        if (keyboardState.IsKeyDown(Keys.S))
        {
            tempVelocity += new Vector2(0, 1f);
        }
        if (keyboardState.IsKeyDown(Keys.A))
        {
            tempVelocity += new Vector2(-1f, 0);
        }
        if (keyboardState.IsKeyDown(Keys.D))
        {
            tempVelocity += new Vector2(1f, 0);
        }

        float velocityLength = tempVelocity.Length();
        if (velocityLength > 0f) tempVelocity /= velocityLength;
        _player.Physics.Velocity = _player.Speed * tempVelocity;


        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            Point mousePos = mouseState.Position;
                
            Console.WriteLine("click coords:{0}, block {1} clicked", mousePos, _cam.ScreenToGameUnits(mousePos.ToVector2()));
            Vector2 mined = _map.Mine(_cam.ScreenToGameUnits(mousePos.ToVector2()));
            if (mined.Y > 0)
            {
                _player.SpawnOrb(mined);
            }
        }

        float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Vector2 absVelocity = new Vector2(MathF.Abs(_player.Physics.Velocity.X),MathF.Abs(_player.Physics.Velocity.Y)) * seconds;

        Physics[] surroundings = _map.GetPhysicsInRange(_player.Physics.Position - absVelocity, _player.Physics.Position + _player.Physics.Size + absVelocity);
        _player.Update(gameTime, surroundings, MapWidth);

        _cam.Target = _player.Physics.Position + _player.Physics.Size / 2;
        _cam.Update(gameTime, MapWidth);

        _map.GenerateRows(_player.Physics.Position, _cam);
        
        //base.Update(gameTime);
    }

    public override void Draw(GraphicsDeviceManager graphicsManager, GameTime gameTime, SpriteBatch spriteBatch)
    {
        _map.Draw(spriteBatch, _cam);
        _player.Draw(spriteBatch, _cam, gameTime);
    }
    
    
}