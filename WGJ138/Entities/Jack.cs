using System;
using System.Collections.Generic;
using System.Linq;
using Coroutine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Animations;
using MLEM.Input;
using MLEM.Startup;
using MLEM.Textures;

namespace WGJ138.Entities {
    public class Jack : Entity {

        public Jack(Tile currentTile) : base(currentTile, new SpriteAnimation(0.4F, GameImpl.Textures[0, 1], GameImpl.Textures[0, 2])) {
            this.MoveRange = 1;
            this.Speed = 2;
        }

        public override IEnumerable<IWait> MakeTurn(Gameplay gameplay) {
            while (true) {
                yield return new WaitEvent(CoroutineEvents.Update);
                if (MlemGame.Input.IsMouseButtonPressed(MouseButton.Left)) {
                    var hoveredTile = gameplay.GetHoveredTile();
                    if (this.GetMovableTiles().Contains(hoveredTile)) {
                        foreach (var wait in this.Move(hoveredTile))
                            yield return wait;
                        yield return new WaitSeconds(0.5F);
                        break;
                    }
                }
            }
        }

    }
}