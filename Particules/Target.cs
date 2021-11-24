using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Brownien.Particules {
    public class Target : Particule {
        public static Texture2D texture;

        public Target(Vector2 position) : base(position) {
        }

        public override Texture2D getTexture() {
            return texture;
        }
    }
}