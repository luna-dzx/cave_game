using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CaveGame;

public abstract class GameState
{
    public bool Exiting;
    public string NextState;
    public bool Loaded;

    public virtual void LoadContent(GraphicsDevice graphics, ContentManager content)
    {
        Loaded = true;
    }
    public abstract void Update(GameTime gameTime);
    public abstract void Draw(GraphicsDeviceManager graphicsManager, GameTime gameTime, SpriteBatch spriteBatch);

    protected void Exit(string nextState = "")
    {
        NextState = nextState;
        Exiting = true;
    }
}