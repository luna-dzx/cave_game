using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CaveGame;

public static class Program
{
    [STAThread]
    static void Main()
    {
        using (var game = new Game1()) game.Run();
    }
}