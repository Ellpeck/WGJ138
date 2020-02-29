using System;
using System.Collections.Generic;
using System.Linq;
using Coroutine;
using MLEM.Animations;
using MLEM.Extensions;

namespace WGJ138.Entities {
    public class MeleeTree : Entity {

        public MeleeTree(Tile currentTile) : base(currentTile, new SpriteAnimation(0.45F, GameImpl.Textures[1, 1], GameImpl.Textures[1, 2]), 2) {
            this.MoveRange = 1;
        }

        public override IEnumerable<IWait> MakeTurn(Gameplay gameplay) {
            yield return new WaitSeconds(0.5F);
            var tiles = this.GetTilesInRange().Where(t => t.CanOccupy()).ToArray();
            foreach (var wait in this.Move(GameImpl.Random.GetRandomEntry(tiles)))
                yield return wait;
            yield return new WaitSeconds(0.5F);
        }

    }
}