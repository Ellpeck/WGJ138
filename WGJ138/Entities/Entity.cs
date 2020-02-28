using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Animations;
using MLEM.Textures;

namespace WGJ138.Entities {
    public class Entity {

        protected readonly SpriteAnimation Animation;
        public Point Position;

        public Entity(Point position, SpriteAnimation animation) {
            this.Position = position;
            this.Animation = animation;
        }

        public virtual void Update(GameTime time) {
            this.Animation.Update(time);
        }

        public virtual void Draw(GameTime time, SpriteBatch batch) {
            batch.Draw(this.Animation.CurrentRegion, (this.Position.ToVector2() - new Vector2(0, 0.25F)) * Board.TileSize, Color.White);
        }

    }
}