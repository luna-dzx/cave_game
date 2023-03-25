using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace ThisIsTheActualProjectIPromise
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D _background;

        private Camera _cam;
        private Map _map;
        private Player _player;

        private Physics tempBlockDELETE;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Texture2D[] textures = {
                Content.Load<Texture2D>("sky"),
                Content.Load<Texture2D>("stone")
            };
            _map = new Map(textures, 400,400,5.0f);
            _player = new Player(new Vector2(2, 1),new Vector2(0.75f, 1.69f), Content.Load<Texture2D>("ballsoodman"));
            _cam = new Camera(new Vector2(50,50), new Vector2(800,600), _player.physics.Position,3.5f);

            tempBlockDELETE = new Physics(Vector2.Zero,Vector2.One);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _background = Content.Load<Texture2D>("ballsoodman");
        }

        protected override void Update(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (keyboardState.IsKeyDown(Keys.W))
            {
                _player.physics.Velocity += new Vector2(0, -0.5f)*seconds;
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                _player.physics.Velocity += new Vector2(0, 0.5f)*seconds;
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                _player.physics.Velocity += new Vector2(-0.5f, 0)*seconds;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                _player.physics.Velocity += new Vector2(0.5f, 0)*seconds;
            }

            if (keyboardState.IsKeyDown(Keys.Space))
            {
                _player.physics.Velocity += new Vector2(0.0f,-15.0f)*seconds;
            }
            
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                Point mousePos = mouseState.Position;
                Console.WriteLine("click coords:{0}, block {1} clicked",mousePos, _cam.ScreenToGameUnits(mousePos.ToVector2()) );
                _map.Mine(_cam.ScreenToGameUnits(mousePos.ToVector2()));
            }

            _player.Update(gameTime, new Physics[] {tempBlockDELETE});

            _cam._target= _player.physics.Position+_player.physics.Size/2;
            _cam.Update(gameTime);
            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin(samplerState: SamplerState.PointWrap);
            
            _map.Draw(_spriteBatch, _cam);

            _player.Draw(_spriteBatch, _cam);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
