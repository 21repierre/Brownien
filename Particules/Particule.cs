using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Brownien.Particules {
    public abstract class Particule {
        public static Particule[] particules;
        private static int particuleCounter;

        private static readonly float deltaT = 0.1f;
        public float mass = 1;

        public Vector2 position;
        public float size = 32;
        protected Vector2 speed;
        protected Vector2 acceleration;
        private static float k = 0;

        protected Particule(Vector2 position) {
            this.position = position;
            particules[particuleCounter] = this;
            particuleCounter++;
        }

        public Vector2 center => position + new Vector2(size, size);

        public abstract Texture2D getTexture();

        public void _old_calcCollision() {
            foreach (var part2 in particules) {
                if (part2 == this) continue;


                var pos1 = position;
                var pos2 = part2.position;
                var speed1 = speed;
                var speed2 = part2.speed;

                Vector2 i1, i2;
                if (findCircleIntersect(pos1.X, pos1.Y, size, pos2.X, pos2.Y, size, out i1,
                    out i2) > 0) {
                    var tangent = new Vector2(i1.X - i2.X, i1.Y - i2.Y);
                    tangent.Normalize();
                    var normal = new Vector2(-tangent.Y, tangent.X);

                    var v1T = tangent.dot(speed1);
                    var v2T = tangent.dot(speed2);
                    var v1N = normal.dot(speed1);
                    var v2N = normal.dot(speed2);

                    var speed1T = v1T * tangent;
                    var speed2T = v2T * tangent;

                    var vv1N = (v1N * (mass - part2.mass) + 2 * part2.mass * v2N) / (mass + part2.mass);
                    var vv2N = (v2N * (part2.mass - mass) + 2 * mass * v1N) / (mass + part2.mass);

                    var speed1N = vv1N * normal;
                    var speed2N = vv2N * normal;

                    speed = speed1T + speed1N;
                    part2.speed = speed2T + speed2N;
                }

                /*
                 * Calculer 2 segments de vecteur vitesse: equation, pt debut, pt arrivée
                 * parcourir à deltaT les 2 segments pour chercher intersection
                 * arret si: intersection ou 1 des 2 à la fin du segment
                 */

                var deltaV1 = speed1 * deltaT;
                var deltaV2 = speed2 * deltaT;

                var endPos1 = pos1 + speed1;
                var endPos2 = pos2 + speed2;

                var tempPos1 = new Vector2(pos1.X, pos1.Y);
                var tempPos2 = new Vector2(pos2.X, pos2.Y);

                while (tempPos1.LengthSquared() < endPos1.LengthSquared() &&
                       tempPos2.LengthSquared() < endPos2.LengthSquared()) {
                    tempPos1 += deltaV1;
                    tempPos2 += deltaV2;

                    //Intersection de cercles

                    Vector2 inter1;
                    Vector2 inter2;
                    var inters = findCircleIntersect(tempPos1.X, tempPos1.Y, size, tempPos2.X, tempPos2.Y, part2.size,
                        out inter1, out inter2);

                    if (inters > 0) {
                        Console.WriteLine("COLLISION " + inters);

                        var tangent = new Vector2(inter1.X - inter2.X, inter1.Y - inter2.Y);
                        tangent.Normalize();
                        var normal = new Vector2(-tangent.Y, tangent.X);

                        var v1T = tangent.dot(speed1);
                        var v2T = tangent.dot(speed2);
                        var v1N = normal.dot(speed1);
                        var v2N = normal.dot(speed2);

                        var speed1T = v1T * tangent;
                        var speed2T = v2T * tangent;

                        var vv1N = (v1N * (mass - part2.mass) + 2 * part2.mass * v2N) / (mass + part2.mass);
                        var vv2N = (v2N * (part2.mass - mass) + 2 * mass * v1N) / (mass + part2.mass);

                        var speed1N = vv1N * normal;
                        var speed2N = vv2N * normal;

                        speed = speed1T + speed1N;
                        part2.speed = speed2T + speed2N;

                        break;
                    }
                }
            }
        }

        public static void update(GameTime time) {
            foreach (var particule in particules) {
                //particule.position += particule.speed * (float) time.ElapsedGameTime.TotalSeconds;
                var r = new Vector2(1,0);
                particule.acceleration = (-k * particule.speed + r) / particule.mass;
                //Console.WriteLine(particule.acceleration);
                particule.speed += particule.acceleration;
                var gr = new GaussianRandom();
                r = 50 * new Vector2((float) gr.NextGaussian(), (float) gr.NextGaussian());
                particule.speed = r;
                //var position = particule.position + particule.speed * (float) time.ElapsedGameTime.TotalSeconds;
                var position = particule.position + particule.speed * (float) Math.Sqrt(time.ElapsedGameTime.TotalSeconds);
                
                if (position.X <= 0 || position.X + 2 * particule.size >= Game1.graphics.PreferredBackBufferWidth)
                    particule.speed.X = -particule.speed.X;
                else if (position.Y <= 0 || position.Y + 2 * particule.size >= Game1.graphics.PreferredBackBufferHeight) particule.speed.Y = -particule.speed.Y;

                foreach (var other in particules)
                    if (other != particule)
                        if ((position - other.position).Length() <= other.size + particule.size) {
                            var v1 = particule.speed;
                            var x1 = position;
                            var v2 = other.speed;
                            var x2 = other.position;
                            var v1p = v1 - 2 * other.mass / (particule.mass + other.mass) * (v1 - v2).dot(x1 - x2) / (x1 - x2).LengthSquared() * (x1 - x2);
                            var v2p = v2 - 2 * particule.mass / (particule.mass + other.mass) * (v2 - v1).dot(x2 - x1) / (x2 - x1).LengthSquared() * (x2 - x1);
                            particule.speed = v1p;
                            other.speed = v2p;
                        }

                particule.position += particule.speed * (float) time.ElapsedGameTime.TotalSeconds;
            }
        }

        private int findCircleIntersect(
            float cx0, float cy0, float radius0,
            float cx1, float cy1, float radius1,
            out Vector2 intersection1, out Vector2 intersection2) {
            // Find the distance between the centers.
            var dx = cx0 - cx1;
            var dy = cy0 - cy1;
            var dist = Math.Sqrt(dx * dx + dy * dy);

            // See how many solutions there are.
            if (dist > radius0 + radius1) {
                // No solutions, the circles are too far apart.
                intersection1 = new Vector2(float.NaN, float.NaN);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 0;
            }

            if (dist < Math.Abs(radius0 - radius1)) {
                // No solutions, one circle contains the other.
                intersection1 = new Vector2(float.NaN, float.NaN);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 0;
            }

            if (dist == 0 && radius0 == radius1) {
                // No solutions, the circles coincide.
                intersection1 = new Vector2(float.NaN, float.NaN);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 0;
            }

            // Find a and h.
            var a = (radius0 * radius0 -
                radius1 * radius1 + dist * dist) / (2 * dist);
            var h = Math.Sqrt(radius0 * radius0 - a * a);

            // Find P2.
            var cx2 = cx0 + a * (cx1 - cx0) / dist;
            var cy2 = cy0 + a * (cy1 - cy0) / dist;

            // Get the points P3.
            intersection1 = new Vector2(
                (float) (cx2 + h * (cy1 - cy0) / dist),
                (float) (cy2 - h * (cx1 - cx0) / dist));
            intersection2 = new Vector2(
                (float) (cx2 - h * (cy1 - cy0) / dist),
                (float) (cy2 + h * (cx1 - cx0) / dist));

            // See if we have 1 or 2 solutions.
            if (dist == radius0 + radius1) return 1;
            return 2;
        }
    }
}