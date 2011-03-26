#region

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace GameStateManagement {
    /// <summary>
    ///   Helper class represents a single entry in a MenuScreen. By default this
    ///   just draws the entry text string, but it can be customized to display menu
    ///   entries in different ways. This also provides an event that will be raised
    ///   when the menu entry is selected.
    /// </summary>
    internal class MenuEntry {
        #region Fields

        /// <summary>
        ///   The position at which the entry is drawn. This is set by the MenuScreen
        ///   each frame in Update.
        /// </summary>
        private Vector2 _position;

        /// <summary>
        ///   Tracks a fading selection effect on the entry.
        /// </summary>
        /// <remarks>
        ///   The entries transition out of the selection effect when they are deselected.
        /// </remarks>
        private float _selectionFade;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the text of this menu entry.
        /// </summary>
        public string Text { get; set; }


        /// <summary>
        ///   Gets or sets the position at which to draw this menu entry.
        /// </summary>
        public Vector2 Position {
            get { return _position; }
            set { _position = value; }
        }

        #endregion

        #region Events

        /// <summary>
        ///   Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> Selected;


        /// <summary>
        ///   Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex) {
            if (Selected != null)
                Selected(this, new PlayerIndexEventArgs(playerIndex));
        }

        #endregion

        #region Initialization

        /// <summary>
        ///   Constructs a new menu entry with the specified text.
        /// </summary>
        public MenuEntry(string text) {
            Text = text;
        }

        #endregion

        #region Update and Draw

        /// <summary>
        ///   Updates the menu entry.
        /// </summary>
        public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime) {
            // there is no such thing as a selected item on Windows Phone, so we always
            // force isSelected to be false
#if WINDOWS_PHONE
            isSelected = false;
#endif

            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            var fadeSpeed = (float) gameTime.ElapsedGameTime.TotalSeconds*4;

            if (isSelected)
                _selectionFade = Math.Min(_selectionFade + fadeSpeed, 1);
            else
                _selectionFade = Math.Max(_selectionFade - fadeSpeed, 0);
        }


        /// <summary>
        ///   Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime) {
            // there is no such thing as a selected item on Windows Phone, so we always
            // force isSelected to be false
#if WINDOWS_PHONE
            isSelected = false;
#endif

            // Draw the selected entry in yellow, otherwise white.
            var color = isSelected ? Color.DarkRed : Color.DarkGray;

            // Pulsate the size of the selected menu entry.
            var time = gameTime.TotalGameTime.TotalSeconds;

            var pulsate = (float) Math.Sin(time*6) + 1;

            var scale = 1 + pulsate*0.05f*_selectionFade;

            // Modify the alpha to fade text out during transitions.
            color *= screen.TransitionAlpha;

            // Draw text, centered on the middle of each line.
            var screenManager = screen.ScreenManager;
            var spriteBatch = screenManager.SpriteBatch;
            var font = screenManager.Font;

            var origin = new Vector2(0, font.LineSpacing/2);

            // Draw text shadow
            if (isSelected) {
                var shadowColor = Color.LightGray;
                var shadowPosition = new Vector2(_position.X + 2, _position.Y + 2);
                spriteBatch.DrawString(font, Text, shadowPosition, shadowColor, 0,
                                       origin, scale, SpriteEffects.None, 1);
            } // Draw realtext
            spriteBatch.DrawString(font, Text, _position, color, 0,
                                   origin, scale, SpriteEffects.None, 0);
        }


        /// <summary>
        ///   Queries how much space this menu entry requires.
        /// </summary>
        public virtual int GetHeight(MenuScreen screen) {
            return screen.ScreenManager.Font.LineSpacing;
        }


        /// <summary>
        ///   Queries how wide the entry is, used for centering on the screen.
        /// </summary>
        public virtual int GetWidth(MenuScreen screen) {
            return (int) screen.ScreenManager.Font.MeasureString(Text).X;
        }

        #endregion

        internal MenuScreen MenuScreen {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}