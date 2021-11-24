using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Brownien {
    public class ColorTexture {
        public static Texture2D WHITE;
        public static SpriteFont ARIAL_50;
        public static SpriteFont ARIAL_20;

        public static void load(GraphicsDevice device) {
            WHITE = new Texture2D(device, 1, 1);
            WHITE.SetData(new[] {Color.White});
        }
    }
}