#region File Description
// InputControl.cs
// Used for input processing
// This class does all the interaction with the user input
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
    public class InputControl
    {
        public readonly GamePadState[] curStates;
        public readonly GamePadState[] lastStates;
        public readonly ThumbstickState[] curStick;
        public readonly ThumbstickState[] lastStick;

        public PlayerIndex LastPlayer { get; protected set; }

        public InputControl()   // Constructor
        {
            curStates = new GamePadState[4];
            lastStates = new GamePadState[4];
            curStick = new ThumbstickState[4];
            lastStick = new ThumbstickState[4];
        }

        public void Update()   // Update input controls
        {
            for (int i = 0; i < 4; i++)
            {
                lastStates[i] = curStates[i];
                curStates[i] = GamePad.GetState((PlayerIndex)i);
                lastStick[i] = curStick[i];
                curStick[i] = new ThumbstickState(curStates[i]);
            }
        }

        public Vector2 GetRS() { return curStates[(int)LastPlayer].ThumbSticks.Right; }   // Returns right stick vector for current player only
        public Vector2 GetLS() { return curStates[(int)LastPlayer].ThumbSticks.Left; }   // Returns left stick vector for current player only

        public bool IsAPressed(bool allPlayers)
        {
            // Checks if A button is pressed
            // If allPlayers is passed as true then checks all possible controllers
            if (allPlayers)
            {

                for (int i = 0; i < 4; i++)
                {
                    if (curStates[i].Buttons.A == ButtonState.Pressed && lastStates[i].Buttons.A == ButtonState.Released)
                    {
                        LastPlayer = (PlayerIndex)i;
                        return true;
                    }
                }
            }
            else if (curStates[(int)LastPlayer].Buttons.A == ButtonState.Pressed && lastStates[(int)LastPlayer].Buttons.A == ButtonState.Released) return true;  // Current player only

            return false;
        }

        public bool IsBPressed(bool allPlayers)
        {
            // Checks if B button is pressed
            // If allPlayers is passed as true then checks all possible controllers
            if (allPlayers)  // If true check all players
            {
                for (int i = 0; i < 4; i++)
                {
                    if (curStates[i].Buttons.B == ButtonState.Pressed && lastStates[i].Buttons.B == ButtonState.Released) return true;
                }
            }
            else if (curStates[(int)LastPlayer].Buttons.B == ButtonState.Pressed && lastStates[(int)LastPlayer].Buttons.B == ButtonState.Released) return true;  // Current player only

            return false;
        }

        public bool IsPauseEvent()
        {
            // Checks if any event occurs that would pause the game
            // This event only checks on the current player's controller

            // Check Start Button
            if (curStates[(int)LastPlayer].Buttons.Start == ButtonState.Pressed && lastStates[(int)LastPlayer].Buttons.Start == ButtonState.Released) return true;

            // Check to see if guide screen is up
            if (Guide.IsVisible) return true;

            // Controller disconnect
            if (lastStates[(int)LastPlayer].IsConnected && !curStates[(int)LastPlayer].IsConnected) return true;

            return false;
        }

        public bool IsMenuUp()
        {
            // Checks both DPad and Left Stick for Menu Movement
            // All controllers since this is a menu action
            for (int i = 0; i < 4; i++)
            {
                if (curStates[i].DPad.Up == ButtonState.Pressed && lastStates[i].DPad.Up == ButtonState.Released) return true;
                if (curStick[i].LSUp == ButtonState.Pressed && lastStick[i].LSUp == ButtonState.Released) return true;
            }
            return false;
        }

        public bool IsMenuDown()
        {
            // Checks both DPad and Left Stick for Menu Movement
            // All controllers since this is a menu action
            for (int i = 0; i < 4; i++)
            {
                if (curStates[i].DPad.Down == ButtonState.Pressed && lastStates[i].DPad.Down == ButtonState.Released) return true;
                if (curStick[i].LSDown == ButtonState.Pressed && lastStick[i].LSDown == ButtonState.Released) return true;
            }
            return false;
        }
    }

    public class ThumbstickState
    {
        // Converts Thumbstick control to a "button-like" state
        public ButtonState LSUp { get; protected set; }
        public ButtonState LSDown { get; protected set; }
        public ButtonState LSLeft { get; protected set; }
        public ButtonState LSRight { get; protected set; }
        public ButtonState RSUp { get; protected set; }
        public ButtonState RSDown { get; protected set; }
        public ButtonState RSLeft { get; protected set; }
        public ButtonState RSRight { get; protected set; }

        public ThumbstickState() { }

        public ThumbstickState(GamePadState state)
        {
            // Sets states
            if (state.ThumbSticks.Left.Y > 0.7f) LSUp = ButtonState.Pressed; else LSUp = ButtonState.Released;
            if (state.ThumbSticks.Left.Y < -0.7f) LSDown = ButtonState.Pressed; else LSDown = ButtonState.Released;
            if (state.ThumbSticks.Left.X < -0.7f) LSLeft = ButtonState.Pressed; else LSLeft = ButtonState.Released;
            if (state.ThumbSticks.Left.X > 0.7f) LSRight = ButtonState.Pressed; else LSRight = ButtonState.Released;
            if (state.ThumbSticks.Right.Y > 0.7f) RSUp = ButtonState.Pressed; else RSUp = ButtonState.Released;
            if (state.ThumbSticks.Right.Y < -0.7f) RSDown = ButtonState.Pressed; else RSDown = ButtonState.Released;
            if (state.ThumbSticks.Right.X < -0.7f) RSLeft = ButtonState.Pressed; else RSLeft = ButtonState.Released;
            if (state.ThumbSticks.Right.X > 0.7f) RSRight = ButtonState.Pressed; else RSRight = ButtonState.Released;
        }
    }
}
