using System;
using System.Collections.Generic;
using System.Linq;
using Coroutine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Animations;
using MLEM.Extensions;
using MLEM.Misc;
using MLEM.Startup;
using MLEM.Textures;
using MLEM.Ui;
using MLEM.Ui.Elements;

namespace WGJ138.Entities {
    public class Entity {

        protected readonly SpriteAnimation Animation;
        protected Tile CurrentTile { get; private set; }
        public Point Position => this.CurrentTile.Position;
        public bool IsDead => this.Health <= 0;
        protected Vector2 VisualPosition;
        protected int MoveRange;
        private readonly ProgressBar healthBar;

        public int MaxHealth { get; protected set; }
        public int Health { get; private set; }
        public float Speed { get; protected set; } = 1;
        public int Strength { get; protected set; } = 1;

        private float damageTimer;

        public Entity(Tile currentTile, SpriteAnimation animation, int maxHealth) {
            this.Move(currentTile);
            this.Animation = animation;
            this.MaxHealth = maxHealth;
            this.Health = maxHealth;

            if (this.MaxHealth > 0) {
                var ui = GameImpl.Instance.UiSystem;
                var root = ui.Get("Health").Element;

                this.healthBar = root.AddChild(new ProgressBar(Anchor.TopLeft, new Vector2(20, 5), Direction2.Right, 1) {
                    IsHidden = true,
                    DrawAlpha = 0,
                    Texture = GameImpl.Instance.SpriteBatch.GenerateTexture(new Color(100, 50, 50))
                });
                this.healthBar.OnUpdated = (e, time) => {
                    var pos = this.VisualPosition + new Vector2(0.5F, -0.65F);
                    var offset = GameImpl.Instance.Camera.ToCameraPos(pos * Board.TileSize) / e.Scale;
                    offset.X -= e.Size.X / 2;
                    e.PositionOffset = offset;
                    this.healthBar.CurrentValue = this.Health / (float) this.MaxHealth;
                };
            }
        }

        private IEnumerator<IWait> ShowHealthBar(float seconds) {
            this.healthBar.IsHidden = false;
            while (this.healthBar.DrawAlpha < 1) {
                this.healthBar.DrawAlpha += 0.05F;
                yield return new WaitEvent(CoroutineEvents.Update);
            }
            yield return new WaitSeconds(seconds);
            while (this.healthBar.DrawAlpha > 0) {
                this.healthBar.DrawAlpha -= 0.05F;
                yield return new WaitEvent(CoroutineEvents.Update);
            }
            this.healthBar.IsHidden = true;
        }

        public IEnumerable<IWait> Move(Tile newTile) {
            if (this.CurrentTile != null) {
                this.CurrentTile.CurrentEntity = null;
            } else {
                this.VisualPosition = newTile.Position.ToVector2();
            }
            this.CurrentTile = newTile;
            if (newTile != null) {
                newTile.CurrentEntity = this;
                return this.MoveTo(newTile.Position.ToVector2());
            } else {
                this.healthBar.Parent.RemoveChild(this.healthBar);
            }
            return null;
        }

        private IEnumerable<IWait> MoveTo(Vector2 newTilePos) {
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

        protected IEnumerable<IWait> Attack(Entity other) {
            CoroutineHandler.Start(other.ShowHealthBar(1));
            foreach (var wait in this.MoveTo(other.VisualPosition))
                yield return wait;
            other.OnAttacked(this);
            yield return new WaitSeconds(0.1F);
            foreach (var wait in this.MoveTo(this.Position.ToVector2()))
                yield return wait;
        }

        public virtual void Update(GameTime time) {
            this.Animation.Update(time);
            if (this.damageTimer > 0) {
                this.damageTimer -= (float) time.ElapsedGameTime.TotalSeconds;
                if (this.damageTimer <= 0 && this.IsDead)
                    this.Move(null);
            }
        }

        public virtual void Draw(GameTime time, SpriteBatch batch) {
            var color = Color.Lerp(this.IsDead ? Color.Transparent : Color.White, Color.Red, this.damageTimer / 0.5F);
            batch.Draw(this.Animation.CurrentRegion, (this.VisualPosition - new Vector2(0, 0.25F)) * Board.TileSize, color, 0, Vector2.Zero, 1, SpriteEffects.None, this.GetRenderDepth());
        }

        protected virtual float GetRenderDepth(float offset = 0) {
            return (this.Position.Y + 1 + offset) / this.CurrentTile.Board.Height;
        }

        public virtual IEnumerable<IWait> MakeTurn(Gameplay gameplay) {
            return null;
        }

        public void OnAttacked(Entity attacker) {
            this.Health -= attacker.Strength;
            this.damageTimer = 0.5F;
        }

        public IEnumerable<Tile> GetTilesInRange() {
            for (var x = -this.MoveRange; x <= this.MoveRange; x++) {
                for (var y = -this.MoveRange; y <= this.MoveRange; y++) {
                    if (x == 0 && y == 0)
                        continue;
                    var tile = this.CurrentTile.Board[this.Position.X + x, this.Position.Y + y];
                    if (tile != null)
                        yield return tile;
                }
            }
        }

    }
}