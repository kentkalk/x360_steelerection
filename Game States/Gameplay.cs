#region File Description
// Gameplay.cs
// This is where the game itself actually runs
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
    class Gameplay : Screen
    {
        // Constructor
        public Gameplay() { }

        #region Declarations

        ContentManager content;

        // Constant Settings
        const int shapecount = 5;
        const int colorcount = 3;

        // Textures
        public static Texture2D[,] blkTextures = new Texture2D[shapecount, colorcount];
        public static BlockData[] blkData = new BlockData[shapecount];
        Matrix groundMat;

        // Sprites
        BlockSprite player = new BlockSprite();  // Player
        List<BlockSprite> blocks = new List<BlockSprite>();  // Placed Blocks
        RectSprite ground = new RectSprite();
        RectSprite topbar = new RectSprite();
        RectSprite sky = new RectSprite();

        // Game Variables
        int desty;  // used for vertical screen scroll
        float modcur;  // used for vertical screen scroll
        float modtot = 0f;  // used for verical screen scroll
        float FallModifier = 1f;
        int score = 0;  // Scorekeeper
        int screenscore = 0;  // Screen Scroll Trigger
        byte blkHitColor;  // Color of block that collision occured with
        float scrnAlpha = 1;  // Alpha of current fade

        // Event Flags
        bool eventCollision;  //  Collision Occured
        bool eventScrollScreen;  // Screen Scroll in process
        bool eventFirstScroll = true;  // Is this first time screen will scroll?
        bool eventGameOver;  // Game Over
        bool eventTransitionIn = true;  // Screen Fade In from Black

        // Text
        SpriteFont deffont;  // Font for on screen text
        String debugout1 = "", debugout2 = "", debugout3 = "", debugout4 = "";  // Strings for debugging purposes

        // Timing
        TimeSpan playtime = TimeSpan.Zero;

        #endregion

        #region TODO CLEAN THIS STUFF UP
        // From earlier versions, most of this should relocate to another class

        // Constants
        const int screenw = 1280;
        const int screenh = 720;

        // Setting Variables
        int movesens = 15;  // Left Stick Sensitivity
        float rotsens = 0.25f;  //  Right Stick Sensitivity


        const int lbound = 128;
        const int rbound = 1152;

        #endregion

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(GameManager.Game.Services, "Content");

            // Load font
            deffont = content.Load<SpriteFont>("fntDefFont");

            // Load Blocks
            //   Shape 0 = long
            //   Shape 1 = bent
            //   Shape 2 = square
            //   Shape 3 = pipe
            //   Shape 4 = T
            //   Color 0 = red
            //   Color 1 = blue
            //   Color 2 = green
            //   blkTextures array [x,y] where x is shape and y is color
            blkTextures[0, 0] = content.Load<Texture2D>(@".\Blocks\texRedLong");
            blkTextures[1, 0] = content.Load<Texture2D>(@".\Blocks\texRedBent");
            blkTextures[2, 0] = content.Load<Texture2D>(@".\Blocks\texRedSquare");
            blkTextures[3, 0] = content.Load<Texture2D>(@".\Blocks\texRedPipe");
            blkTextures[4, 0] = content.Load<Texture2D>(@".\Blocks\texRedT");
            blkTextures[0, 1] = content.Load<Texture2D>(@".\Blocks\texBlueLong");
            blkTextures[1, 1] = content.Load<Texture2D>(@".\Blocks\texBlueBent");
            blkTextures[2, 1] = content.Load<Texture2D>(@".\Blocks\texBlueSquare");
            blkTextures[3, 1] = content.Load<Texture2D>(@".\Blocks\texBluePipe");
            blkTextures[4, 1] = content.Load<Texture2D>(@".\Blocks\texBlueT");
            blkTextures[0, 2] = content.Load<Texture2D>(@".\Blocks\texGreenLong");
            blkTextures[1, 2] = content.Load<Texture2D>(@".\Blocks\texGreenBent");
            blkTextures[2, 2] = content.Load<Texture2D>(@".\Blocks\texGreenSquare");
            blkTextures[3, 2] = content.Load<Texture2D>(@".\Blocks\texGreenPipe");
            blkTextures[4, 2] = content.Load<Texture2D>(@".\Blocks\texGreenT");

            // Calculate Block Data
            for (int i = 0; i < shapecount; i++)
            {
                blkData[i] = new BlockData();
                blkData[i].Radius = (int)Math.Sqrt((blkTextures[i, 0].Width * blkTextures[i, 0].Width) + (blkTextures[i, 0].Height * blkTextures[i, 0].Height)) / 2;
                blkData[i].ColorArray = TexturetoArray(blkTextures[i, 0]);
                blkData[i].CollisionMat = Matrix.CreateTranslation(-(blkTextures[i, 0].Width / 2f), -(blkTextures[i, 0].Height / 2f), 0);
                blkData[i].Origin = new Vector2(blkTextures[i, 0].Width / 2, blkTextures[i, 0].Height / 2);
            }

            // Reset Player
            ResetPlayerBlock(player);

            // Load Other Textures
            ground.Texture = content.Load<Texture2D>("texGround");
            ground.ColorArray = TexturetoArray(ground.Texture);
            ground.Position = new Vector2(0, screenh - 122);
            topbar.Texture = content.Load<Texture2D>("texTopBar");
            topbar.Position = new Vector2(0, 0);

            sky.Texture = content.Load<Texture2D>("texSky");
            sky.Position = new Vector2(0, -500);

            groundMat = Matrix.CreateTranslation(0, screenh - 122, 0);  // Calculate ground matrix for collision detection

            GameManager.IsPaused = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (eventTransitionIn)
            {
                if (scrnAlpha > 0) scrnAlpha -= 0.03f;
                else
                {
                    scrnAlpha = 0;
                    eventTransitionIn = false;
                    GameManager.IsPaused = false;
                }
            }

            if (!GameManager.IsPaused)
            {
                // Smooth Scroll
                if (eventScrollScreen && desty > 0)
                {
                    modcur = 400 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    //modcur = -(modcur * modcur) + (desty * modcur);
                    modtot += modcur;
                    if (modtot > desty)
                    {
                        modcur = modtot - desty;
                        modtot = 0f;
                        eventScrollScreen = false;
                    }
                    foreach (BlockSprite block in blocks)
                    {
                        block.Position = new Vector2(block.Position.X, block.Position.Y + modcur);
                    }
                    ground.Position = new Vector2(ground.Position.X, ground.Position.Y + modcur);
                }

                // Crane Fall Control
                // Drops crane based on fall modifier
                player.Position = new Vector2(player.Position.X, player.Position.Y + FallModifier);

                // Checks if crane falls too far
                if (player.Position.Y > screenh) eventGameOver = true;

                // Collision Detection
                // Checks if player collides with any blocks or the ground
                if (!eventGameOver)
                {
                    Matrix playerMat = blkData[player.ShapeIndex].CollisionMat * Matrix.CreateRotationZ(player.Angle) * Matrix.CreateTranslation(player.Position.X, player.Position.Y, 0);

                    // Collision with ground
                    if ((ground.Position.Y - blkData[player.ShapeIndex].Radius) < player.Position.Y)
                    {
                        if (PixelCollisionCheck(blkData[player.ShapeIndex].ColorArray, playerMat, ground.ColorArray, groundMat).X != -1)
                        {
                            eventCollision = true;
                            blkHitColor = player.ColorIndex;
                        }
                    }

                    // Collision with other blocks
                    if (!eventCollision)
                    {
                        foreach (BlockSprite block in blocks)
                        {
                            int r1 = blkData[player.ShapeIndex].Radius;
                            int r2 = blkData[block.ShapeIndex].Radius;

                            if (block.Position.Y < screenh + r2)
                            {
                                if (RadialCollisionCheck(blkData[player.ShapeIndex].Radius, blkData[block.ShapeIndex].Radius, player.Position, block.Position))
                                {
                                    Matrix blockMat = blkData[block.ShapeIndex].CollisionMat * Matrix.CreateRotationZ(block.Angle) * Matrix.CreateTranslation(block.Position.X, block.Position.Y, 0);
                                    if (PixelCollisionCheck(blkData[player.ShapeIndex].ColorArray, playerMat, blkData[block.ShapeIndex].ColorArray, blockMat).X != -1)
                                    {
                                        eventCollision = true;
                                        blkHitColor = block.ColorIndex;
                                        break;
                                    }
                                }
                            }
                        }
                    }


                    // Process if Collision Exists
                    if (eventCollision)
                    {
                        if (blkHitColor != player.ColorIndex) eventGameOver = true;
                        else
                        {
                            FallModifier = 7 / (1 + (float)Math.Pow(Math.E, (-0.05 * (blocks.Count - 36))));
                            blocks.Add(new BlockSprite(player.ShapeIndex, player.ColorIndex, player.Position, player.Angle));

                            // Update Score
                            float curscore = ground.Position.Y - player.Position.Y + (blkData[player.ShapeIndex].Radius * (float)Math.Abs(Math.Cos(player.Angle)));
                            if (curscore > score) score = (int)curscore;

                            // Scroll Screen
                            if (eventFirstScroll)
                            {
                                if ((score - screenscore) > 250)
                                {
                                    eventScrollScreen = true;
                                    desty = score - screenscore - 50;
                                    screenscore = score;
                                    eventFirstScroll = false;
                                }
                            }
                            else
                            {
                                eventScrollScreen = true;
                                desty = score - screenscore;
                                screenscore = score;
                            }

                            ResetPlayerBlock(player);
                            eventCollision = false;
                        }
                    }
                }
                if (eventGameOver) GameManager.IsPaused = true;
            }

        }

        public override void ProcessInput(InputControl input)
        {
            if (input.IsPauseEvent())
            {
                GameManager.IsPaused = true;
                GameManager.AddScreen(new PauseBox());
            }

            if (!GameManager.IsPaused)
            {
                Vector2 posPlayer1 = player.Position;

                float LSx = input.GetLS().X;  // Left Stick
                if (LSx > 0 || LSx < 0) LSx *= movesens;

                float RSx = input.GetRS().X;  // Right Stick
                RSx *= rotsens;

                // Player Movement
                posPlayer1.X += LSx;
                if (posPlayer1.X < lbound) posPlayer1.X = lbound;
                if (posPlayer1.X > rbound) posPlayer1.X = rbound;
                player.Position = posPlayer1;
                player.Angle += RSx;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = GameManager.SpriteBatch;

            spriteBatch.Begin();

            // Draw Background
            sky.Draw(spriteBatch);
            if (ground.Position.Y < screenh) ground.Draw(spriteBatch);

            // Draw Crane and Current Block
            if (!eventGameOver) player.Draw(spriteBatch);

            // Draw Existing Blocks
            foreach (BlockSprite block in blocks)
            {
                if (block.Position.Y - blkData[block.ShapeIndex].Radius < screenh) block.Draw(spriteBatch);
            }

            // Draw Topbar
            topbar.Draw(spriteBatch);

            // Draw Score
            spriteBatch.DrawString(deffont, Convert.ToString(score) + " feet", new Vector2(196, 70), Color.Black);

            // Screen Text for Debug Purposes
            spriteBatch.DrawString(deffont, debugout1, new Vector2(0, 220), Color.Red);
            spriteBatch.DrawString(deffont, debugout2, new Vector2(0, 260), Color.Red);
            spriteBatch.DrawString(deffont, debugout3, new Vector2(0, 300), Color.Red);
            spriteBatch.DrawString(deffont, debugout4, new Vector2(0, 340), Color.Red);

            spriteBatch.End();

            GameManager.DrawFade(scrnAlpha);
        }

        public override void UnloadContent() { }

        #region Functions
        // Functions

        // Checks for pixel perfect collisions
        private Vector2 PixelCollisionCheck(Color[,] tex1, Matrix mat1, Color[,] tex2, Matrix mat2)
        {
            Matrix mat1to2 = mat1 * Matrix.Invert(mat2);
            int width1 = tex1.GetLength(0);
            int height1 = tex1.GetLength(1);
            int width2 = tex2.GetLength(0);
            int height2 = tex2.GetLength(1);

            for (int x1 = 0; x1 < width1; x1++)
            {
                for (int y1 = 0; y1 < height1; y1++)
                {
                    Vector2 pos1 = new Vector2(x1, y1);
                    Vector2 pos2 = Vector2.Transform(pos1, mat1to2);

                    int x2 = (int)pos2.X;
                    int y2 = (int)pos2.Y;
                    if ((x2 >= 0) && (x2 < width2))
                    {
                        if ((y2 >= 0) && (y2 < height2))
                        {
                            if (tex1[x1, y1].A > 0)
                            {
                                if (tex2[x2, y2].A > 0)
                                {
                                    Vector2 screenPos = Vector2.Transform(pos1, mat1);
                                    return screenPos;
                                }
                            }
                        }
                    }
                }
            }
            return new Vector2(-1, -1);
        }

        // Builds array from a texture
        private Color[,] TexturetoArray(Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
                for (int y = 0; y < texture.Height; y++)
                    colors2D[x, y] = colors1D[x + y * texture.Width];

            return colors2D;
        }

        // Checks if two circles have collided
        private bool RadialCollisionCheck(int r1, int r2, Vector2 pos1, Vector2 pos2)
        {
            int r = r1 + r2;
            int dx = (int)pos1.X - (int)pos2.X;
            int dy = (int)pos1.Y - (int)pos2.Y;
            if ((dx * dx) + (dy * dy) < (r * r)) return true;
            else return false;
        }

        // Resets Player Block to Starting Point
        private void ResetPlayerBlock(BlockSprite player)
        {
            Random random = new Random();
            player.ShapeIndex = (byte)random.Next(0, shapecount);
            player.ColorIndex = (byte)random.Next(0, colorcount);
            player.Position = new Vector2(screenw / 2, 172);
            player.Angle = 0f;
        }
        #endregion
    }
}
