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
    private Roomba _roomba;

    const int MapWidth = 30;
    const int MapHeight = 30;

    private int _mcJoeFrame;

    private long _seed;

    private float _orbTimer = 0f;

    public override void LoadContent(GraphicsDevice graphics, ContentManager content)
    {
        if (!Game1.InitializeCaveGame) return;

        Texture2D[] textures = {
            content.Load<Texture2D>("stonebgtxt"),
            content.Load<Texture2D>("stonetxt"),
            content.Load<Texture2D>("nigel")
        };

        _player = new Player(graphics, new Vector2(15, 1),
            new Vector2(0.75f, 1.69f),
            new []
            {
                content.Load<Texture2D>("MycJoe1"),
                content.Load<Texture2D>("MycJoe2"),
                content.Load<Texture2D>("MycJoe3"),
                content.Load<Texture2D>("MycJoe4"),
                content.Load<Texture2D>("MycJoe5"),
                content.Load<Texture2D>("MycJoe6"),
                content.Load<Texture2D>("MycJoe7")
            },
            5f, 10f);
        Random random = new Random();
        _seed = random.NextInt64();
        
        if (!Game1.ResetCaveGame) // use data loaded from file
        {
            _player.Physics.Position = Game1.LoadedPosition;
            _seed = Game1.LoadedSeed;
        }

        _roomba = new Roomba(content.Load<Texture2D>("sky"), new Vector2(11, 16));
        _map = new Map(textures, MapWidth, MapHeight + (int)_player.Physics.Position.Y, 5.0f, _seed);
        _cam = new Camera(new Vector2(50, 50), new Vector2(800, 600), _player.Physics.Position, 3.5f);
    }

    private float _totalTime = 0f;

    public override void Update(GameTime gameTime)
    {
        float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _totalTime += seconds;
        _orbTimer += seconds;
        
        _map.GenerateRows(_player.Physics.Position, _cam);
        
        var keyboardState = Keyboard.GetState();
        var mouseState = Mouse.GetState();

        //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        //    Exit();

        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            Game1.LoadedPosition = _player.Physics.Position;
            Game1.LoadedSeed = _seed;
            Exit("pause");
        }

        float tempVelocity = 0f;

        if (_mcJoeFrame < 5 || _orbTimer > 1f) _mcJoeFrame = 0;

        if (keyboardState.IsKeyDown(Keys.A))
        {
            tempVelocity -= 1f;
            if (_mcJoeFrame < 5)
            {
                if ((int)((_totalTime * 5f) % 2f) == 0) _mcJoeFrame = 3;
                else _mcJoeFrame = 4;
            }
        }
        if (keyboardState.IsKeyDown(Keys.D))
        {
            tempVelocity += 1f;
            if (_mcJoeFrame < 5)
            {
                if ((int)((_totalTime * 5f) % 2f) == 0) _mcJoeFrame = 1;
                else _mcJoeFrame = 2;
            }
        }
        _player.Physics.Velocity.X = _player.Speed * tempVelocity;

        if (keyboardState.IsKeyDown(Keys.Space) && _player.Physics.Grounded)
        {
            _player.Physics.Velocity.Y = -_player.JumpForce;
            _player.Physics.Grounded = false;
        }


        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            Point mousePos = mouseState.Position;
                
            Console.WriteLine("click coords:{0}, block {1} clicked", mousePos, _cam.ScreenToGameUnits(mousePos.ToVector2()));
            Vector2 mined = _map.Mine(_cam.ScreenToGameUnits(mousePos.ToVector2()));
            if (mined.Y > 0)
            {
                var pos = _player.Physics.Position + _player.Physics.Size / 2;
                _player.SpawnOrb(mined);
                _mcJoeFrame = mined.X > pos.X ? 5 : 6;
                _orbTimer = 0f;
            }
        }
        
        Vector2 absVelocity = new Vector2(MathF.Abs(_player.Physics.Velocity.X),MathF.Abs(_player.Physics.Velocity.Y)) * seconds;

        Physics[] surroundings = _map.GetPhysicsInRange(_player.Physics.Position - absVelocity, _player.Physics.Position + _player.Physics.Size + absVelocity);
        _player.Update(gameTime, surroundings, MapWidth);

        _cam.Target = _player.Physics.Position + _player.Physics.Size / 2;
        _cam.Update(gameTime, MapWidth);

        absVelocity = new Vector2(MathF.Abs(_roomba.Physics.Velocity.X),MathF.Abs(_roomba.Physics.Velocity.Y)) * seconds;
        surroundings = _map.GetPhysicsInRange(_roomba.Physics.Position - absVelocity, _roomba.Physics.Position + _roomba.Physics.Size + absVelocity);
        _roomba.Update(gameTime, surroundings, MapWidth, _map);
        //base.Update(gameTime);
    }

    public override void Draw(GraphicsDeviceManager graphicsManager, GameTime gameTime, SpriteBatch spriteBatch)
    {
        _map.Draw(spriteBatch, _cam);
        _player.Draw(spriteBatch, _cam, gameTime, _mcJoeFrame);
        _roomba.Draw(spriteBatch, _cam, gameTime);
    }
    
    
}