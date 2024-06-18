#region File Decription
// PauseBox.cs
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
    class PauseBox : Screen
    {
        // Constructor
        public PauseBox() { }

        ContentManager content;
        SpriteFont menufont;
        RectSprite popup = new RectSprite();
        List<Menu> pausemenu = new List<Menu>();
        MenuChoice menuChoice;
        const int MenuXShift = 60;
        float scrnAlpha;
        bool eventTransitionIn = true;
        bool eventTransitionOut;

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

            popup.Texture = content.Load<Texture2D>("texPauseBox");
            popup.Position = new Vector2(448, 256);

            menufont = content.Load<SpriteFont>("fntMenuFont");
        }

        public override void Update(GameTime gameTime)
        {
            if (eventTransitionIn) if (scrnAlpha < 0.75f) scrnAlpha += 0.075f;
                else
                {
                    scrnAlpha = 0.75f;
                    eventTransitionIn = false;
                }

            if (eventTransitionOut) if (scrnAlpha > 0) scrnAlpha -= 0.075f;
                else scrnAlpha = 0;
        }

        public override void ProcessInput(InputControl input)
        {
            if (!eventTransitionIn && !eventTransitionOut)
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
                            eventTransitionOut = true;
                            break;
                        case MenuChoice.Exit:
                            GameManager.Game.Exit();
                            break;
                        default:
                            break;
                    }
                }

            }

            if (scrnAlpha == 0)
            {
                GameManager.RemoveScreen(this);
                GameManager.IsPaused = false;
                eventTransitionOut = false;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = GameManager.SpriteBatch;

            GameManager.DrawFade(scrnAlpha);

            if (!eventTransitionIn)
            {
                spriteBatch.Begin();

                popup.Draw(spriteBatch);

                for (int i = 0; i < pausemenu.Count; i++)
                {
                    Vector2 posItem = new Vector2(0, 355);
                    Vector2 textsize = menufont.MeasureString(pausemenu[i].Text);
                    posItem.X = (GameManager.screenw / 2) + MenuXShift;
                    posItem.Y += (i * 50);
                    Color itemcolor = Color.Black;
                    if (pausemenu[i].Selected == true)
                    {
                        float scale = 0.0625f * (float)Math.Sin(6 * gameTime.TotalGameTime.TotalSeconds) + 1;
                        spriteBatch.DrawString(menufont, pausemenu[i].Text, new Vector2(posItem.X + 3, posItem.Y + 3), Color.LightGray, 0, new Vector2((textsize.X / 2), (textsize.Y / 2)), scale, SpriteEffects.None, 0);
                        spriteBatch.DrawString(menufont, pausemenu[i].Text, posItem, Color.Red, 0, new Vector2((textsize.X / 2), (textsize.Y / 2)), scale, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.DrawString(menufont, pausemenu[i].Text, posItem, Color.Black, 0, new Vector2((textsize.X / 2), (textsize.Y / 2)), 1f, SpriteEffects.None, 0);
                    }

                }

                spriteBatch.End();
            }
        }

        public override void UnloadContent()
        {
        }
    }
}
