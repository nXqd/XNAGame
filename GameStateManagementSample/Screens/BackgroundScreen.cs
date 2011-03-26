#region

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

#endregion

namespace GameStateManagement {
    /// <summary>
    ///   The background screen sits behind all the other menu screens.
    ///   It draws a background image that remains fixed in place regardless
    ///   of whatever transitions the screens on top of it may be doing.
    /// </summary>
    internal class BackgroundScreen : GameScreen {
        #region Fields

        private Texture2D _backgroundFadeTexture;
        private Song _backgroundSong;
        private Texture2D _backgroundTexture;
        private ContentManager _content;
        private int _mAlphaValue = 1;
        private double _mFadeDelay = .035;
        private int _mFadeIncrement = 3;

        #endregion

        #region Initialization

        /// <summary>
        ///   Constructor.
        /// </summary>
        public BackgroundScreen() {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        ///   Loads graphics content for this screen. The background texture is quite
        ///   big, so we use our own local ContentManager to load it. This allows us
        ///   to unload before going from the menus into the game itself, wheras if we
        ///   used the shared ContentManager provided by the Game class, the content
        ///   would remain loaded forever.
        /// </summary>
        public override void LoadContent() {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _backgroundTexture = _content.Load<Texture2D>("background");
            _backgroundFadeTexture = _content.Load<Texture2D>("background-fade");
            _backgroundSong = _content.Load<Song>("backgroundMusic");

            MediaPlayer.Play(_backgroundSong);
            MediaPlayer.IsRepeating = true;
        }


        /// <summary>
        ///   Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent() {
            _content.Unload();
        }

        #endregion

        #region Update and Draw

        /// <summary>
        ///   Updates the background screen. Unlike most screens, this should not
        ///   transition off even if it has been covered by another screen: it is
        ///   supposed to be covered, after all! This overload forces the
        ///   coveredByOtherScreen parameter to false in order to stop the base
        ///   Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                    bool coveredByOtherScreen) {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Fade effect
            _mFadeDelay -= gameTime.ElapsedGameTime.TotalSeconds;

            //If the Fade delays has dropped below zero, then it is time to 
            //fade in/fade out the image a little bit more.
            if (_mFadeDelay <= 0) {
                //Reset the Fade delay
                _mFadeDelay = .035;

                //Increment/Decrement the fade value for the image
                _mAlphaValue += _mFadeIncrement;

                //If the AlphaValue is equal or above the max Alpha value or
                //has dropped below or equal to the min Alpha value, then 
                //reverse the fade
                if (_mAlphaValue >= 255 || _mAlphaValue <= 0) {
                    _mFadeIncrement *= -1;
                }
            }
        }


        /// <summary>
        ///   Draws the background screen.
        /// </summary>
        public override void Draw(GameTime gameTime) {
            var spriteBatch = ScreenManager.SpriteBatch;
            var viewport = ScreenManager.GraphicsDevice.Viewport;
            var fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            spriteBatch.Begin();

            spriteBatch.Draw(_backgroundFadeTexture, fullscreen,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            spriteBatch.Draw(_backgroundTexture, fullscreen,
                             new Color(255, 255, 255, (byte) MathHelper.Clamp(_mAlphaValue, 190, 255)));

            spriteBatch.End();
        }

        #endregion
    }
}