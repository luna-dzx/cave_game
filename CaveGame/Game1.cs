using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CaveGame;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private string _currentState = "title";
    private Dictionary<string,GameState> _gameStates;
    
    private TitleScreen _titleScreen;
    private CaveGame _caveGame;

    public static bool ResetCaveGame;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _gameStates = new Dictionary<string, GameState>();
        _gameStates.Add("title", new TitleScreen());
        _gameStates.Add("game", new CaveGame());

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _gameStates[_currentState].LoadContent(GraphicsDevice, Content);
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        _gameStates[_currentState].Update(gameTime);
        if (_gameStates[_currentState].Exiting)
        {
            _gameStates[_currentState].Exiting = false;
            _currentState = _gameStates[_currentState].NextState;
            if (_currentState == "") Exit();
            else _gameStates[_currentState].LoadContent(GraphicsDevice, Content);
        }
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin(samplerState: SamplerState.PointWrap, blendState: BlendState.AlphaBlend, sortMode: SpriteSortMode.Immediate);

        _gameStates[_currentState].Draw(_graphics,gameTime,_spriteBatch);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}