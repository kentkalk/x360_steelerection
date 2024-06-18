#region File Decription
// MenuBG.cs
// Menu Backgrounds
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
    class MenuBG : Screen
    {
        // Constructor
        public MenuBG() { }

        ContentManager content;
        RectSprite menubg = new RectSprite();

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(GameManager.Game.Services, "Content");

            menubg.Texture = content.Load<Texture2D>("texMenusBG");
            menubg.Position = new Vector2(0, 0);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = GameManager.SpriteBatch;

            spriteBatch.Begin();

            menubg.Draw(spriteBatch);

            spriteBatch.End();
        }

        public override void UnloadContent()
        {
            // TODO the menu background never gets unloaded even when it closes..fix that
        }
    }
}
