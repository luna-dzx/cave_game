using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CaveGame;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Camera _cam;
    private Map _map;
    private Player _player;

    const int MapWidth = 30;
    const int MapHeight = 30;


    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        Texture2D[] textures = {
            Content.Load<Texture2D>("stonebgtxt"),
            Content.Load<Texture2D>("stonetxt"),
            Content.Load<Texture2D>("nigel")
        };
        _map = new Map(textures, MapWidth, MapHeight, 5.0f);
        _player = new Player(GraphicsDevice, new Vector2(2, 1), new Vector2(0.75f, 1.69f), Content.Load<Texture2D>("MycJoe1"), 5f);
        _cam = new Camera(new Vector2(50, 50), new Vector2(800, 600), _player.Physics.Position, 3.5f);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        var mouseState = Mouse.GetState();

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

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

        Physics[] surroundings = _map.GetPhysicsInRange(_player.Physics.Position, _player.Physics.Position + _player.Physics.Size);
        _player.Update(gameTime, surroundings);

        _cam.Target = _player.Physics.Position + _player.Physics.Size / 2;
        _cam.Update(gameTime, MapWidth);

        _map.GenerateRows(_player.Physics.Position, _cam);


        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin(samplerState: SamplerState.PointWrap, blendState: BlendState.AlphaBlend, sortMode: SpriteSortMode.Immediate);

        _map.Draw(_spriteBatch, _cam);

        _player.Draw(_spriteBatch, _cam, gameTime);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}