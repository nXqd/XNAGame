#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        #region Fields

        private MenuEntry SoundMenuEntry;
        private int _soundVolme = 10;


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Options")
        {
            // Create our menu entries.
            SoundMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            var back = new MenuEntry("Back");

            // Hook up menu event handlers.
            SoundMenuEntry.Selected += SoundMenuEntrySelected;

            
            // Add entries to the menu.
            MenuEntries.Add(SoundMenuEntry);
            MenuEntries.Add(back);
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            SoundMenuEntry.Text = "Music: " + _soundVolme;
        }


        #endregion

        #region Handle Input

        private void SoundMenuEntrySelected(object sender, PlayerIndexEventArgs e) {
            _soundVolme++;
            if (_soundVolme > 10)
                _soundVolme = 0;

            MediaPlayer.Volume = (float) _soundVolme/10;
            
            SetMenuEntryText();
        }
        #endregion
    }
}
