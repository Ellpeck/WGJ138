using System;
using System.Collections.Generic;
using System.Linq;
using Coroutine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Cameras;
using MLEM.Extensions;
using MLEM.Input;
using MLEM.Startup;
using MLEM.Textures;
using WGJ138.Entities;

namespace WGJ138 {
    public class Gameplay {

        private readonly Board board;
        private readonly Camera camera;

        private readonly Queue<Entity> remainingRound = new Queue<Entity>();
        private Entity currentEntity;
        private ActiveCoroutine currentMove;

        public Gameplay(Board board, Camera camera) {
            this.board = board;
            this.camera = camera;
            this.PopulateRound();
        }

        public void Update(GameTime time) {
            if (this.currentMove != null && !this.currentMove.IsFinished)
                return;
            do {
                if (this.remainingRound.Count <= 0)
                    this.PopulateRound();

                this.currentEntity = this.remainingRound.Dequeue();
                if (this.currentEntity.IsDead)
                    continue;
                var turn = this.currentEntity.MakeTurn(this);
                if (turn == null)
                    continue;
                this.currentMove = CoroutineHandler.Start(turn.GetEnumerator());
                break;
            } while (true);
        }

        public Tile GetHoveredTile() {
            var (hoveredX, hoveredY) = this.camera.ToWorldPos(MlemGame.Input.MousePosition.ToVector2()) / Board.TileSize;
            return this.board[hoveredX.Floor(), hoveredY.Floor()];
        }

        private void PopulateRound() {
            var entities = new List<Entity>();
            for (var x = 0; x < this.board.Width; x++) {
                for (var y = 0; y < this.board.Height; y++) {
                    var tile = this.board[x, y];
                    if (tile.CurrentEntity != null && tile.CurrentEntity.Speed > 0)
                        entities.Add(tile.CurrentEntity);
                }
            }
            entities.Sort((e1, e2) => Comparer<float>.Default.Compare(e2.Speed, e1.Speed));
            foreach (var entity in entities)
                this.remainingRound.Enqueue(entity);
        }

        public void Draw(GameTime time, SpriteBatch batch) {
            if (this.currentEntity != null) {
                this.DrawSelection(batch, this.currentEntity.Position, Color.Red);
                if (this.currentEntity is Jack) {
                    foreach (var tile in this.currentEntity.GetTilesInRange()) {
                        var color = tile.CanOccupy() ? Color.Green : Color.Orange;
                        this.DrawSelection(batch, tile.Position, color);
                    }
                }
            }
        }

        private void DrawSelection(SpriteBatch batch, Point tilePos, Color color) {
            batch.Draw(GameImpl.Textures[1, 0], tilePos.ToVector2() * Board.TileSize, color, 0, Vector2.Zero, 1, SpriteEffects.None, 0.001F);
        }

    }
}