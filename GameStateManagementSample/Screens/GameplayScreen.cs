#region

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace GameStateManagement {
    /// <summary>
    ///   This screen implements the actual game logic. It is just a
    ///   placeholder to get the idea across: you'll probably want to
    ///   put some more interesting gameplay in here!
    /// </summary>
    internal class GameplayScreen : GameScreen {
        #region Fields

        private Map _chessBoard;
        private ContentManager _content;

        private Vector2 _enemyPosition = new Vector2(100, 100);
        private SpriteFont _gameFont;

        private List<Texture2D> _mapTiles;
        private Point _mousePoint;

        private float _pauseAlpha;
        private Vector2 _playerPosition = new Vector2(100, 100);
        private Random random = new Random();

        private enum MapTileEnum {
            Black,
            White
        }

        #endregion

        #region Initialization

        /// <summary>
        ///   Constructor.
        /// </summary>
        public GameplayScreen() {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _mapTiles = new List<Texture2D>();
        }


        /// <summary>
        ///   Load graphics content for the game.
        /// </summary>
        public override void LoadContent() {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _gameFont = _content.Load<SpriteFont>("gamefont");
            _mapTiles.Add(_content.Load<Texture2D>("MapTiles\\black"));
            _mapTiles.Add(_content.Load<Texture2D>("MapTiles\\white"));

            _chessBoard = new Map("chessBoard.txt");

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }


        // Unload graphics content used by the game.
        /// <summary>
        /// </summary>
        public override void UnloadContent() {
            _content.Unload();
        }

        #endregion

        #region Update and Draw

        /// <summary>
        ///   Updates the state of the game. This method checks the GameScreen.IsActive
        ///   property, so the game will stop updating when the pause menu is active,
        ///   or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                    bool coveredByOtherScreen) {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                _pauseAlpha = Math.Min(_pauseAlpha + 1f/32, 1);
            else
                _pauseAlpha = Math.Max(_pauseAlpha - 1f/32, 0);

            if (!IsActive) return;
            // Apply some random jitter to make the enemy move around.
            const float randomization = 10;

            _enemyPosition.X += (float) (random.NextDouble() - 0.5)*randomization;
            _enemyPosition.Y += (float) (random.NextDouble() - 0.5)*randomization;

            // Apply a stabilizing force to stop the enemy moving off the screen.
            var targetPosition = new Vector2(
                ScreenManager.GraphicsDevice.Viewport.Width/2 - _gameFont.MeasureString("Insert Gameplay Here").X/2,
                200);

            _enemyPosition = Vector2.Lerp(_enemyPosition, targetPosition, 0.05f);

            // TODO: this game isn't very fun! You could probably improve
            // it by inserting something more interesting in this space :-)
        }


        /// <summary>
        ///   Lets the game respond to player input. Unlike the Update method,
        ///   this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input) {
            if (input == null)
                throw new ArgumentNullException("input");

            // Mouse handle
            var curMouseState = Mouse.GetState();
            if (curMouseState.LeftButton == ButtonState.Pressed) {
                _mousePoint = _chessBoard.GetMatrixPosition(new Point(curMouseState.X, curMouseState.Y), 30);
            }


            // Look up inputs for the active player profile.
            if (ControllingPlayer != null) {
                var playerIndex = (int) ControllingPlayer.Value;

                var keyboardState = input.CurrentKeyboardStates[playerIndex];

                // The game pauses either if the user presses the pause button, or if
                // they unplug the active gamepad. This requires us to keep track of
                // whether a gamepad was ever plugged in, because we don't want to pause
                // on PC if they are playing with a keyboard and have no gamepad at all!

                if (input.IsPauseGame(ControllingPlayer)) {
                    ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
                }
                else {
                    // Otherwise move the player position.
                    var movement = Vector2.Zero;

                    if (keyboardState.IsKeyDown(Keys.Left))
                        movement.X--;

                    if (keyboardState.IsKeyDown(Keys.Right))
                        movement.X++;

                    if (keyboardState.IsKeyDown(Keys.Up))
                        movement.Y--;

                    if (keyboardState.IsKeyDown(Keys.Down))
                        movement.Y++;

                    if (movement.Length() > 1)
                        movement.Normalize();

                    _playerPosition += movement*2;
                }
                // Mouse
            }
        }


        /// <summary>
        ///   Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime) {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);

            // Our player and enemy are both actually just text strings.
            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            // Draw chess map
            DrawChessBoard(ref spriteBatch);
            // Draw mouse position
            spriteBatch.DrawString(_gameFont,
                                   new StringBuilder(string.Format("Mouse position: {0}:{1}", _mousePoint.X,
                                                                   _mousePoint.Y)), new Vector2(300), Color.White);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || _pauseAlpha > 0) {
                var alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, _pauseAlpha/2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

        private void DrawChessBoard(ref SpriteBatch spriteBatch) {
            var x = _chessBoard.MatrixSize.X;
            var y = _chessBoard.MatrixSize.Y;
            const int black = (int) MapTileEnum.Black;
            const int white = (int) MapTileEnum.White;

            for (var i = 0; i < y; i++) {
                for (var j = 0; j < x; j++) {
                    if (_mapTiles == null) continue;

                    var mapTile = _chessBoard.MapMatrix[i][j] == black ? _mapTiles[black] : _mapTiles[white];
                    var square = mapTile.Width;
                    spriteBatch.Draw(mapTile, new Vector2(i*square, j*square), Color.White);
                }
            }
        }

        #endregion
    }
}