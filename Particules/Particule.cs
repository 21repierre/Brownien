using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Brownien.Particules {
    public abstract class Particule {
        private const float deltaT = 0.1f;
        public static Particule[] particules;
        private static int particuleCounter;
        public static int absorbedCount;
        private static readonly GaussianRandom gaussian = new GaussianRandom();

        public bool absorbed;
        protected Vector2 acceleration;
        private Particule attachedTo;
        private bool frozen;
        public float mass = 1;

        public Vector2 position;
        public float size = 32;
        public Vector2 speed;
        public Vector2 origin;

        protected Particule(Vector2 position) {
            this.position = position;
            origin = position;
            particules[particuleCounter] = this;
            particuleCounter++;
        }

        public Vector2 center => position + new Vector2(size, size);

        public abstract Texture2D getTexture();

        public static void update(float time, int index) {
            var start = index * particules.Length / 16;
            var end = (index + 1) * particules.Length / 16;

            for (var i = start; i < end; i++) {
                var particule = particules[i];
                if (particule.absorbed) continue;

                if (particule.frozen) {
                    if ((particule.attachedTo.position - particule.position).Length() <= particule.attachedTo.size / 2) {
                        particule.absorbed = true;
                        absorbedCount++;
                    } else {
                        particule.position += (particule.attachedTo.position - particule.position) / 100;
                    }

                    continue;
                }

                // Mise à jour de la vitesse avec une vitesse aléatoire
                var r = 0.1f * new Vector2((float) gaussian.NextGaussian(), (float) gaussian.NextGaussian());
                particule.speed += r;
                var position = particule.position + particule.speed * (float) Math.Sqrt(time);

                // Vérifier qu'on reste dans le cadre de la fenetre
                if (position.X <= 0 || position.X + 2 * particule.size >= Game1.graphics.PreferredBackBufferWidth)
                    particule.speed.X = -particule.speed.X;
                else if (position.Y <= 0 || position.Y + 2 * particule.size >= Game1.graphics.PreferredBackBufferHeight) particule.speed.Y = -particule.speed.Y;


                foreach (var other in particules)
                    if (other != particule) // Détection de particule
                        if ((position - other.position).Length() <= (other.size + particule.size) / 2) {
                            /*
                             * Collision détecté, on traite comme une collision élastique
                             */
                            var v1 = particule.speed;
                            var x1 = position;
                            var v2 = other.speed;
                            var x2 = other.position;
                            var v1p = v1 - 2 * other.mass / (particule.mass + other.mass) * (v1 - v2).dot(x1 - x2) / (x1 - x2).LengthSquared() * (x1 - x2);
                            var v2p = v2 - 2 * particule.mass / (particule.mass + other.mass) * (v2 - v1).dot(x2 - x1) / (x2 - x1).LengthSquared() * (x2 - x1);
                            //particule.speed = v1p;
                            if (other.GetType() != typeof(Target)) {
                                //other.speed = v2p;
                            } else {
                                //La particule rencontre une cible
                                particule.frozen = true;
                                particule.attachedTo = other;
                                Vector2 outPos;
                                findCircleIntersect(position, other.position, particule.size / 2, other.size / 2, out outPos, out _);
                                particule.position = outPos;
                                particule.speed = Vector2.Zero;
                            }
                        }


                particule.position += particule.speed * time;
            }
        }

        private static int findCircleIntersect(
            Vector2 pos1, Vector2 pos2, float radius1, float radius2,
            out Vector2 intersection1, out Vector2 intersection2) {
            // Find the distance between the centers.
            var dist = (pos1 - pos2).Length();

            // See how many solutions there are.
            if (dist > radius1 + radius2) {
                // No solutions, the circles are too far apart.
                intersection1 = new Vector2(float.NaN, float.NaN);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 0;
            }

            if (dist < Math.Abs(radius2 - radius1)) {
                // No solutions, one circle contains the other.
                intersection1 = new Vector2(float.NaN, float.NaN);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 0;
            }

            if (dist == 0 && radius2 == radius1) {
                // No solutions, the circles coincide.
                intersection1 = new Vector2(float.NaN, float.NaN);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 0;
            }

            // Find a and h.
            var a = (radius1 * radius1 -
                radius2 * radius2 + dist * dist) / (2 * dist);
            var h = Math.Sqrt(radius1 * radius1 - a * a);

            // Find P2.
            var cx2 = pos1.X + a * (pos2.X - pos1.X) / dist;
            var cy2 = pos1.Y + a * (pos2.Y - pos1.Y) / dist;

            // Get the points P3.
            intersection1 = new Vector2(
                (float) (cx2 + h * (pos2.Y - pos1.Y) / dist),
                (float) (cy2 - h * (pos2.X - pos1.X) / dist));
            intersection2 = new Vector2(
                (float) (cx2 - h * (pos2.Y - pos1.Y) / dist),
                (float) (cy2 + h * (pos2.X - pos1.X) / dist));

            // See if we have 1 or 2 solutions.
            if (dist == radius2 + radius1) return 1;
            return 2;
        }
    }
}
//Coucou Pierre, ça va ? Moi oui. Passe le bonjour à Hugo ! (message du 19 octobre 2021)