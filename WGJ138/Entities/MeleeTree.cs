using System;
using System.Collections.Generic;
using System.Linq;
using Coroutine;
using Microsoft.Xna.Framework;
using MLEM.Animations;
using MLEM.Extensions;

namespace WGJ138.Entities {
    public class MeleeTree : Entity {

        public MeleeTree(Tile currentTile) : base(currentTile, new SpriteAnimation(0.45F, GameImpl.Textures[2, 1], GameImpl.Textures[2, 2]), 5) {
            this.MoveRange = 1;
        }

        public override IEnumerable<IWait> MakeTurn(Gameplay gameplay) {
            yield return new WaitSeconds(0.5F);
            var attack = this.GetTilesInRange().Where(t => t.CurrentEntity is Jack).ToArray();
            if (attack.Length > 0) {
                var tile = GameImpl.Random.GetRandomWeightedEntry(attack, e => e.CurrentEntity.Health);
                foreach (var wait in this.Attack(tile.CurrentEntity))
                    yield return wait;
            } else {
                var tiles = this.GetTilesInRange().Where(t => t.CanOccupy()).ToArray();
                var closestGoal = this.CurrentTile.Board.GetEntities(e => e is Jack)
                    .OrderBy(e => Vector2.DistanceSquared(e.Position.ToVector2(), this.VisualPosition))
                    .First();
                var choice = tiles.OrderBy(t => (int) Vector2.DistanceSquared(closestGoal.Position.ToVector2(), t.Position.ToVector2())).First();
                foreach (var wait in this.Move(choice))
                    yield return wait;
            }
            yield return new WaitSeconds(0.5F);
        }

    }
}