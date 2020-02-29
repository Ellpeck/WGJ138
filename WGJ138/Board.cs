using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WGJ138.Entities;

namespace WGJ138 {
    public class Board {

        public const int TileSize = 16;
        private readonly Tile[,] tiles;
        public Tile this[int x, int y] {
            get {
                if (x < 0 || y < 0 || x >= this.Width || y >= this.Height)
                    return null;
                return this.tiles[x, y];
            }
            set => this.tiles[x, y] = value;
        }
        public readonly int Width;
        public readonly int Height;

        public Board(int width, int height) {
            this.Width = width;
            this.Height = height;
            this.tiles = new Tile[width, height];
            for (var x = 0; x < width; x++) {
                for (var y = 0; y < height; y++)
                    this.tiles[x, y] = new Tile(this, new Point(x, y), true);
            }
        }

        public void Update(GameTime time) {
            for (var x = 0; x < this.Width; x++) {
                for (var y = 0; y < this.Height; y++) {
                    this[x, y].Update(time);
                }
            }
        }

        public void Draw(GameTime time, SpriteBatch batch) {
            for (var x = 0; x < this.Width; x++) {
                for (var y = 0; y < this.Height; y++) {
                    this[x, y].Draw(time, batch);
                }
            }
        }

        public IEnumerable<Entity> GetEntities(Func<Entity, bool> predicate) {
            for (var x = 0; x < this.Width; x++) {
                for (var y = 0; y < this.Height; y++) {
                    var tile = this[x, y];
                    if (tile.CurrentEntity != null && predicate(tile.CurrentEntity))
                        yield return tile.CurrentEntity;
                }
            }
        }

        public Tile RandomTile() {
            return this[GameImpl.Random.Next(this.Width), GameImpl.Random.Next(this.Height)];
        }

    }
}