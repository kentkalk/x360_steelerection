#region File Description
// GameManager.cs
// Allows multiple instances of class Screen to work together
// Only passes input to topmost screen
// Also controls some other minor game management
//
// Steel Erection
// Copyright (C) 2014
#endregion

#region Using
using System;
using System.Diagnostics;
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
    public class GameManager : DrawableGameComponent
    {
        #region Declarations

        public const int screenw = 1280;

        public SpriteFont deffont;
        Texture2D emptytexture;
        List<Screen> screens = new List<Screen>();  // Screen Management -- screens can be added and removed at any time
        bool isInitCompl;  // Flag if game initialization has finished yet
        SpriteBatch spriteBatch;  // All screens share this SpriteBatch
        InputControl input = new InputControl();  // Input Controller

        #endregion

        #region Properties

        public SpriteBatch SpriteBatch          // SpriteBatch Property, all screens share this
        {
            get { return spriteBatch; }
        }

        public bool IsPaused { get; set; }

        #endregion

        public GameManager(Game game) : base(game) { }

        public override void Initialize()
        {
            base.Initialize();
            isInitCompl = true;
        }

        protected override void LoadContent()
        {
            ContentManager content = Game.Content;
            spriteBatch = new SpriteBatch(GraphicsDevice);

            deffont = content.Load<SpriteFont>("fntDefFont");  // Load default font here so usuable by all screens
            emptytexture = content.Load<Texture2D>("texEmpty");

            foreach (Screen screen in screens) screen.LoadContent();
        }

        protected override void UnloadContent()
        {
            foreach (Screen screen in screens) screen.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            input.Update();

            foreach (Screen screen in screens) screen.Update(gameTime);

            screens[screens.Count - 1].ProcessInput(input);

        }

        public override void Draw(GameTime gameTime)
        {
            foreach (Screen screen in screens) screen.Draw(gameTime);
        }

        public void AddScreen(Screen screen)
        {
            screen.GameManager = this;
            if (isInitCompl) screen.LoadContent();
            screens.Add(screen);
        }

        public void RemoveScreen(Screen screen)
        {
            if (isInitCompl) screen.UnloadContent();
            screens.Remove(screen);
        }

        public void DrawFade(float alpha)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(emptytexture, new Rectangle(0, 0, 1280, 720), Color.Black * alpha);
            spriteBatch.End();
        }
    }
}
