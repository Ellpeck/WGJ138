using System.Linq;
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
        private Entity selectedEntity;

        public Gameplay(Board board, Camera camera) {
            this.board = board;
            this.camera = camera;
        }

        public void Update(GameTime time) {
            var (hoveredX, hoveredY) = this.camera.ToWorldPos(MlemGame.Input.MousePosition.ToVector2()) / Board.TileSize;
            var hoveredTile = this.board[hoveredX.Floor(), hoveredY.Floor()];

            if (MlemGame.Input.IsMouseButtonPressed(MouseButton.Left)) {
                if (this.selectedEntity == null) {
                    this.selectedEntity = hoveredTile?.CurrentEntity;
                } else if (!this.selectedEntity.GetMovableTiles().Contains(hoveredTile)) {
                    this.selectedEntity = hoveredTile?.CurrentEntity;
                } else {
                    this.selectedEntity.Move(hoveredTile);
                    this.selectedEntity = null;
                }
            }
        }

        public void Draw(GameTime time, SpriteBatch batch) {
            if (this.selectedEntity != null) {
                this.DrawSelection(batch, this.selectedEntity.Position, Color.Red);
                foreach (var tile in this.selectedEntity.GetMovableTiles())
                    this.DrawSelection(batch, tile.Position, Color.Green);
            }
        }

        private void DrawSelection(SpriteBatch batch, Point tilePos, Color color) {
            batch.Draw(GameImpl.Textures[1, 0], tilePos.ToVector2() * Board.TileSize, color, 0, Vector2.Zero, 1, SpriteEffects.None, 0.001F);
        }

    }
}