#region File Decription
// MainMenu.cs
// Main Menu Screen
//
// Steel Erection
// Copyright (C) 2014
#endregion

#region Using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
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
    class MainMenu : Screen
    {
        // Constructor
        public MainMenu() { }

        #region Declarations

        ContentManager content;
        RectSprite mainmenu = new RectSprite();
        RectSprite kdklogo = new RectSprite();
        RectSprite buttona = new RectSprite();
        List<Menu> main = new List<Menu>();
        SpriteFont menufont;
        MenuChoice menuChoice;
        int MenuYPos = 220;
        bool TrialUpdated;

        enum MenuChoice
        {
            PlayGame,
            HowToPlay,
            FreeBuild,
            HighScores,
            Settings,
            Exit,
            BuyFull
        }

        #endregion

        public override void LoadContent()
        {
            main.Add(new Menu("Speed Mode", true));
            main.Add(new Menu("Free Build Mode", false));
            main.Add(new Menu("How To Play", false));
            main.Add(new Menu("High Scores", false));
            main.Add(new Menu("Settings", false));
            main.Add(new Menu("Exit", false));

            if (content == null)
                content = new ContentManager(GameManager.Game.Services, "Content");

            mainmenu.Texture = content.Load<Texture2D>("texMainMenu");
            mainmenu.Position = new Vector2(342, 80);
            kdklogo.Texture = content.Load<Texture2D>("texKDKLogo");
            kdklogo.Position = new Vector2(952, 609);
            buttona.Texture = content.Load<Texture2D>("texButtonA");
            buttona.Position = new Vector2(148, 609);

            menufont = content.Load<SpriteFont>("fntMenuFont");
        }

        public override void Update(GameTime gameTime)
        {
            // Check Trial Mode
            // This was moved to Update() as it won't get the correct status until then
            if (Guide.IsTrialMode && !TrialUpdated)
            {
                main.Add(new Menu("Buy Full Game", false));
                MenuYPos = 200;
                TrialUpdated = true;
            }
        }

        public override void ProcessInput(InputControl input)
        {
            if (input.IsMenuDown())
            {
                bool SelectNext = false;
                for (int i = 0; i < main.Count; i++)
                {
                    if (SelectNext)
                    {
                        main[i].Selected = true;
                        break;
                    }
                    if (main[i].Selected && i < (main.Count - 1))
                    {
                        SelectNext = true;
                        main[i].Selected = false;
                    }
                }

            }

            if (input.IsMenuUp())
            {
                bool SelectPrev = false;
                for (int i = (main.Count - 1); i >= 0; i--)
                {
                    if (SelectPrev)
                    {
                        main[i].Selected = true;
                        break;
                    }
                    if (main[i].Selected && i != 0)
                    {
                        SelectPrev = true;
                        main[i].Selected = false;
                    }
                }
            }

            if (input.IsAPressed(true))
            {
                for (int i = 0; i < main.Count; i++) if (main[i].Selected) menuChoice = (MenuChoice)i;
                switch (menuChoice)
                {
                    case MenuChoice.PlayGame:
                        GameManager.RemoveScreen(this);
                        GameManager.AddScreen(new Gameplay());
                        break;
                    case MenuChoice.Exit:
                        GameManager.Game.Exit();
                        break;
                    case MenuChoice.BuyFull:
                        Guide.ShowMarketplace(input.LastPlayer);
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

            kdklogo.Draw(spriteBatch);
            mainmenu.Draw(spriteBatch);
            buttona.Draw(spriteBatch);

            spriteBatch.DrawString(GameManager.deffont, "Select", new Vector2(183, 613), Color.Black, 0, new Vector2(0, 0), 0.7f, SpriteEffects.None, 0);

            for (int i = 0; i < main.Count; i++)
            {
                Vector2 posItem = new Vector2(0, MenuYPos);
                Vector2 textsize = menufont.MeasureString(main[i].Text);
                posItem.X = ((GameManager.screenw /2));
                posItem.Y += (i * 50);
                if (main[i].Selected == true)
                {
                    float scale = 0.0625f * (float)Math.Sin(6*gameTime.TotalGameTime.TotalSeconds) + 1;
                    spriteBatch.DrawString(menufont, main[i].Text, new Vector2(posItem.X + 3, posItem.Y + 3), Color.LightGray, 0, new Vector2((textsize.X / 2),(textsize.Y / 2)), scale, SpriteEffects.None, 0);
                    spriteBatch.DrawString(menufont, main[i].Text, posItem, Color.Red, 0, new Vector2((textsize.X / 2), (textsize.Y / 2)), scale, SpriteEffects.None, 0);
                } 
                else
                {
                    spriteBatch.DrawString(menufont, main[i].Text, posItem, Color.Black, 0, new Vector2((textsize.X / 2), (textsize.Y /2)), 1f, SpriteEffects.None, 0);  
                }
            }

            spriteBatch.End();
        }

        public override void UnloadContent()
        {
            content.Unload();
        }
    }
}
