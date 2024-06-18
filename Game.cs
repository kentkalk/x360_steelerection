#region File Description
// Game.cs
// Initializes the game, this is where the execution actually starts
//
// Steel Erection
// Copyright (C) 2014
#endregion

#region Using
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
#endregion

namespace SteelErection
{
    static class Program  // Program entry point
    {
        static void Main(string[] args)
        {
            using (Game game = new Game())
            {
                game.Run();
            }
        }
    }

    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        GameManager gameManager;

        public const int screenw = 1280;
        public const int screenh = 720;
        const byte shapecount = 2;   // Number of different shapes
        const byte colorcount = 3;   // Number of different shape colors

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = screenh;
            graphics.PreferredBackBufferWidth = screenw;
            graphics.PreferMultiSampling = true;
            Content.RootDirectory = "Content";
            Components.Add(new GamerServicesComponent(this));

            gameManager = new GameManager(this);
            Components.Add(gameManager);

            gameManager.AddScreen(new MenuBG());
            gameManager.AddScreen(new MainMenu());

            Guide.SimulateTrialMode = true;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }

    }
}
