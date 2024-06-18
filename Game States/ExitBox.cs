#region File Decription
// ExitBox.cs
// Pause Screen
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
    class ExitBox : Screen
    {
        // Constructor
        public ExitBox() { }

        ContentManager content;
        SpriteFont menufont;
        RectSprite popup = new RectSprite();
        List<Menu> pausemenu = new List<Menu>();
        MenuChoice menuChoice;
        const int MenuXShift = 60;

        enum MenuChoice
        {
            Continue,
            Exit
        }

        public override void LoadContent()
        {
            pausemenu.Add(new Menu("Continue", true));
            pausemenu.Add(new Menu("Exit", false));
            
            if (content == null)
                content = new ContentManager(GameManager.Game.Services, "Content");

            popup.Texture = content.Load<Texture2D>("texExitBox");
            popup.Position = new Vector2(448, 256);

            menufont = content.Load<SpriteFont>("fntMenuFont");
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void ProcessInput(InputControl input)
        {
            if (input.IsMenuDown())
            {
                bool SelectNext = false;
                for (int i = 0; i < pausemenu.Count; i++)
                {
                    if (SelectNext)
                    {
                        pausemenu[i].Selected = true;
                        break;
                    }
                    if (pausemenu[i].Selected && i < (pausemenu.Count - 1))
                    {
                        SelectNext = true;
                        pausemenu[i].Selected = false;
                    }
                }

            }

            if (input.IsMenuUp())
            {
                bool SelectPrev = false;
                for (int i = (pausemenu.Count - 1); i >= 0; i--)
                {
                    if (SelectPrev)
                    {
                        pausemenu[i].Selected = true;
                        break;
                    }
                    if (pausemenu[i].Selected && i != 0)
                    {
                        SelectPrev = true;
                        pausemenu[i].Selected = false;
                    }
                }
            }

            if (input.IsAPressed(true))
            {
                for (int i = 0; i < pausemenu.Count; i++) if (pausemenu[i].Selected) menuChoice = (MenuChoice)i;
                switch (menuChoice)
                {
                    case MenuChoice.Continue:
                        GameManager.RemoveScreen(this);
                        GameManager.IsPaused = false;
                        break;
                    case MenuChoice.Exit:
                        GameManager.Game.Exit();
                        break;
                    default:
                        break;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = GameManager.SpriteBatch;

            spriteBatch.Begin();

            popup.Draw(spriteBatch);

            for (int i = 0; i < pausemenu.Count; i++)
            {
                Vector2 posItem = new Vector2(0, 335);
                Vector2 textsize = menufont.MeasureString(pausemenu[i].Text);
                posItem.X = (((GameManager.screenw - textsize.X) / 2) + MenuXShift);
                posItem.Y += (i * 50);
                Color itemcolor = Color.Black;
                if (pausemenu[i].Selected == true)
                {
                    itemcolor = Color.DarkBlue;
                    // TODO improve this so menu will be more animated
                }
                spriteBatch.DrawString(menufont, pausemenu[i].Text, new Vector2(posItem.X + 3, posItem.Y + 3), Color.LightGray);
                spriteBatch.DrawString(menufont, pausemenu[i].Text, posItem, itemcolor);
            }

            spriteBatch.End();
        }

        public override void UnloadContent()
        {
        }
    }
}
