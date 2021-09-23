using Microsoft.Xna.Framework;

namespace Brownien {
    public static class Utils {
        public static float dot(this Vector3 one, Vector3 two) {
            return one.X * two.X + one.Y * two.Y + one.Z * two.Z;
        }
    }
}