using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Brownien.Particules {
    public class Big : Particule {
        public Texture2D texture;


        public Big(Vector3 position) : base(position) {
            var rnd = new Random();
            speed = new Vector3((float) rnd.NextDouble() * 300-150, (float) rnd.NextDouble() * 300-150,
                0);
        }

        public void update(GameTime time) {
            var tmpPos = position + speed;

            if (tmpPos.X <= 0 || tmpPos.X + 64 >= Game1.graphics.PreferredBackBufferWidth) {
                speed.X = -speed.X;
            }
            else if (tmpPos.Y <= 0 || tmpPos.Y + 64 >= Game1.graphics.PreferredBackBufferHeight) {
                speed.Y = -speed.Y;
            }

            /*
             * TODO:
             * pour check entre particule: intersection du cercle de dl
             *			-> distance A1A2 <= r1+r2
             * si commun: chance de rencontre
             *			-> fonction distance y2-y1 = 0 => rencontre
             */

            position += speed;
        }

        public override Texture2D getTexture() {
            return texture;
        }
    }
}