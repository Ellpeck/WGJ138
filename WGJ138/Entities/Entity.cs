using System.Collections.Generic;
using Coroutine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Animations;
using MLEM.Startup;
using MLEM.Textures;

namespace WGJ138.Entities {
    public class Entity {

        protected readonly SpriteAnimation Animation;
        protected Tile CurrentTile { get; private set; }
        public Point Position => this.CurrentTile.Position;
        protected Vector2 VisualPosition;
        protected int MoveRange;

        public Entity(Tile currentTile, SpriteAnimation animation) {
            this.Move(currentTile);
            this.Animation = animation;
        }

        public void Move(Tile newTile) {
            if (this.CurrentTile != null) {
                this.CurrentTile.CurrentEntity = null;
                CoroutineHandler.Start(this.MoveToTile(newTile.Position.ToVector2()));
            } else {
                this.VisualPosition = newTile.Position.ToVector2();
            }
            this.CurrentTile = newTile;
            newTile.CurrentEntity = this;
        }

        private IEnumerator<IWait> MoveToTile(Vector2 newTilePos) {
            while (true) {
                var diff = newTilePos - this.VisualPosition;
                if (diff.Length() < 0.1F) {
                    this.VisualPosition = newTilePos;
                    yield break;
                }
                diff.Normalize();
                this.VisualPosition += diff * 0.1F;
                yield return new WaitEvent(CoroutineEvents.Update);
            }
        }

        public virtual void Update(GameTime time) {
            this.Animation.Update(time);
        }

        public virtual void Draw(GameTime time, SpriteBatch batch) {
            batch.Draw(this.Animation.CurrentRegion, (this.VisualPosition - new Vector2(0, 0.25F)) * Board.TileSize, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, this.GetRenderDepth());
        }

        protected virtual float GetRenderDepth(float offset = 0) {
            return (this.Position.Y + offset) / this.CurrentTile.Board.Height;
        }

        public IEnumerable<Tile> GetMovableTiles() {
            for (var x = -this.MoveRange; x <= this.MoveRange; x++) {
                for (var y = -this.MoveRange; y <= this.MoveRange; y++) {
                    if (x == 0 && y == 0)
                        continue;
                    var tile = this.CurrentTile.Board[this.Position.X + x, this.Position.Y + y];
                    if (tile != null && tile.CanOccupy)
                        yield return tile;
                }
            }
        }

    }
}