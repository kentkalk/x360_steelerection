using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SteelErection
{
    // Block Data -- holds precalculated data specific to shapes
    public class BlockData
    {
        public int Radius { get; set; }
        public Color[,] ColorArray { get; set; }
        public Matrix CollisionMat { get; set; }
        public Vector2 Origin { get; set; }

        public BlockData() { }
    }


    // Block Sprite -- sprite class used for blocks
    public class BlockSprite
    {
        public byte ShapeIndex { get; set; }
        public byte ColorIndex { get; set; }
        public Vector2 Position { get; set; }
        public float Angle { get; set; }

        public BlockSprite() { }

        public BlockSprite(byte shapeindex, byte colorindex, Vector2 position, float angle)
        {
            ShapeIndex = shapeindex;
            ColorIndex = colorindex;
            Position = position;
            Angle = angle;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Texture2D Texture = Gameplay.blkTextures[ShapeIndex, ColorIndex];
            Vector2 Origin = Gameplay.blkData[ShapeIndex].Origin;
            spriteBatch.Draw(Texture, Position, null, Color.White, Angle, Origin, 1f, SpriteEffects.None, 0f);
        }
    }


    // Rectangle Sprite -- sprite class used for sprites that do not require rotation
    public class RectSprite
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Color[,] ColorArray { get; set; }

        public RectSprite() { }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }

    // Menu -- used for menu screens
    public class Menu
    {
        public string Text { get; set; }
        public bool Selected { get; set; }

        public Menu() { }

        public Menu(string text, bool selected)
        {
            Text = text;
            Selected = selected;
        }
    }

    public abstract class Screen
    {
        public GameManager GameManager
        {
            get { return gameManager; }
            internal set { gameManager = value; }
        }
        GameManager gameManager;

        public virtual void LoadContent() { }
        public virtual void UnloadContent() { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(GameTime gameTime) { }
        public virtual void ProcessInput(InputControl input) { }
    }
}