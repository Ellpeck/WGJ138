using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Textures;
using WGJ138.Entities;

namespace WGJ138 {
    public class Tile {

        public Entity CurrentEntity;
        public readonly bool CanOccupy;

        public readonly Point Position;

        public Tile(Point position, bool canOccupy) {
            this.CanOccupy = canOccupy;
            this.Position = position;
        }

        public void Update(GameTime time) {
            if (this.CurrentEntity != null)
                this.CurrentEntity.Update(time);
        }

        public void Draw(GameTime time, SpriteBatch batch) {
            batch.Draw(GameImpl.Textures[0, 0], this.Position.ToVector2() * Board.TileSize, Color.White);
            if (this.CurrentEntity != null)
                this.CurrentEntity.Draw(time, batch);
        }

    }
}