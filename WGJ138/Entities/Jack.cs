using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Animations;
using MLEM.Textures;

namespace WGJ138.Entities {
    public class Jack : Entity {

        public Jack(Tile currentTile) : base(currentTile, new SpriteAnimation(0.4F, GameImpl.Textures[0, 1], GameImpl.Textures[0, 2])) {
            this.MoveRange = 1;
        }

    }
}