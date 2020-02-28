using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Cameras;
using MLEM.Extensions;
using MLEM.Startup;
using MLEM.Textures;
using WGJ138.Entities;

namespace WGJ138 {
    public class GameImpl : MlemGame {

        public static UniformTextureAtlas Textures { get; private set; }
        public Board Board { get; private set; }
        public Camera Camera { get; private set; }
        public Gameplay Gameplay { get; private set; }

        public GameImpl() {
            this.IsMouseVisible = true;
        }

        protected override void LoadContent() {
            base.LoadContent();

            Textures = new UniformTextureAtlas(LoadContent<Texture2D>("Textures"), 8, 8);
            this.Camera = new Camera(this.GraphicsDevice) {
                AutoScaleWithScreen = true,
                Scale = 4
            };

            this.Board = new Board(20, 10);
            new Jack(this.Board[5, 5]);
            this.Gameplay = new Gameplay(this.Board, this.Camera);
        }

        protected override void DoUpdate(GameTime gameTime) {
            base.DoUpdate(gameTime);
            this.Camera.ConstrainWorldBounds(new Vector2(0, 0), new Vector2(this.Board.Width, this.Board.Height) * Board.TileSize);
            this.Board.Update(gameTime);
            this.Gameplay.Update(gameTime);
        }

        protected override void DoDraw(GameTime gameTime) {
            base.DoDraw(gameTime);
            this.SpriteBatch.Begin(SpriteSortMode.FrontToBack, null, SamplerState.PointClamp, transformMatrix: this.Camera.ViewMatrix);
            this.Board.Draw(gameTime, this.SpriteBatch);
            this.Gameplay.Draw(gameTime, this.SpriteBatch);
            this.SpriteBatch.End();
        }

    }
}