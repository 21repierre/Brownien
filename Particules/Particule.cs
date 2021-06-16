﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Brownien.Particules {
    public abstract class Particule {
        public static Particule[] particules;
        private static int particuleCounter;

        private static readonly float deltaT = 0.1f;

        public Vector3 position;
        public float size = 32;
        protected Vector3 speed;

        protected Particule(Vector3 position) {
            this.position = position;
            particules[particuleCounter] = this;
            particuleCounter++;
        }

        public abstract void update(GameTime time);

        public abstract Texture2D getTexture();

        public void __old_calcCollision() {
            var parts = new Particule[particules.Length - 1];
            var x1 = position.X;
            var y1 = position.Y;
            var r1 = Math.Pow(y1 + speed.Y, 2) + Math.Pow(x1 + speed.X, 2);
            foreach (var part2 in particules)
                if (this != part2) {
                    var x2 = part2.position.X;
                    var y2 = part2.position.Y;
                    var r2 = Math.Pow(y2 + part2.speed.Y, 2) + Math.Pow(x2 + part2.speed.X, 2);

                    var a = (r1 - r2 - x1 * x1 - y1 * y1 + x2 * x2 - y2 * y2) / (2 * y2 - 2 * y2);
                    var d = (x2 - x1) / (y2 - y1);

                    var A = d * d + 1;
                    var B = -2 * x1 + 2 * y1 * d - 2 * a * d;
                    var C = x1 * x1 + y1 * y1 + -2 * y1 * a + a * a - r1;

                    var DELTA = B * B - 4 * A * C;

                    if (DELTA >= 0 || true) {
                        //Console.WriteLine("delta ok");
                        //POTENTIELLE COLLISION
                        // TODO: attention au divide par 0 donc a refaire si v selon on axe = 0
                        var xfinal = position.X + speed.X;
                        var xfinal2 = part2.position.X + part2.speed.X;
                        var yfinal = position.Y + speed.Y;
                        var yfinal2 = part2.position.Y + part2.speed.Y;

                        var xcollision =
                            ((position.Y - part2.position.Y) * speed.X * part2.speed.X +
                                speed.Y * part2.speed.X * position.X - part2.speed.Y * speed.X * part2.position.X) /
                            (speed.Y * part2.speed.X - part2.speed.Y * speed.X);

                        //Si xcollision € droite_speed_1 et droite_speed_2 alors y'a collision

                        var firstCheck = false;
                        var secondCheck = false;

                        //TODO: ici aussi /!\ speedx=0
                        if (speed.X > 0) {
                            if (position.X <= xcollision && xfinal >= xcollision) firstCheck = true;
                        }
                        else {
                            if (position.X >= xcollision && xfinal <= xcollision) firstCheck = true;
                        }

                        if (firstCheck) {
                            Console.WriteLine("first check");
                            if (part2.speed.X > 0) {
                                if (part2.position.X <= xcollision && xfinal2 >= xcollision) secondCheck = true;
                            }
                            else {
                                if (part2.position.X <= xcollision && xfinal2 <= xcollision) secondCheck = true;
                            }

                            if (secondCheck) {
                                //Y a collision
                                Console.WriteLine("collide");

                                var ycollision = speed.Y * (xcollision - position.X) / speed.X - position.Y;

                                //Bounce part 1
                                var xbounce = xcollision - xfinal;
                                var ybounce = ycollision - yfinal;
                                Console.WriteLine(xcollision + " - " + ycollision);
                                Console.WriteLine(position + " - " + speed);
                                position = new Vector3(xbounce, ybounce, 0);
                                Console.WriteLine(position + " - " + speed);
                                speed = -speed;

                                //Bounce part 2
                                var xbounce2 = xcollision - xfinal2;
                                var ybounce2 = ycollision - yfinal2;
                                Console.WriteLine(part2.position + " - " + part2.speed);
                                part2.position = new Vector3(xbounce2, ybounce2, 0);
                                part2.speed = -part2.speed;
                                Console.WriteLine(part2.position + " - " + part2.speed);
                                //Game1.stopDrawing = true;
                            }
                        }
                    }
                }
        }

        public void calcCollision() {
            foreach (var part2 in particules) {
                if (part2 == this) continue;


                var pos1 = position;
                var pos2 = part2.position;
                var speed1 = speed;
                var speed2 = part2.speed;

                if (findCircleIntersect(pos1.X, pos1.Y, size, pos2.X, pos2.Y, size, out _,
                    out _) > 0) {
                    speed = -speed;
                    part2.speed = -part2.speed;
                    position += speed;
                    part2.position += part2.speed;
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

                var tempPos1 = new Vector3(pos1.X, pos1.Y, pos1.Z);
                var tempPos2 = new Vector3(pos2.X, pos2.Y, pos2.Z);

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

                        var tangent = new Vector3(inter1.X - inter2.X, inter1.Y - inter2.Y, 0);

                        var between1 = tangent - tempPos1;
                        var crossprod1 = 0.5 * (tempPos1.LengthSquared() + tangent.LengthSquared() - between1.LengthSquared());
                        var cosa1 = crossprod1 / (tempPos1.Length() * tangent.Length());
                        var a1 = Math.Acos(cosa1);
                        var newSpeed1 = new Vector3((float) (cosa1 * speed.X - Math.Sin(a1) * speed.Y),
                            (float) (Math.Sin(a1) * speed.X + cosa1 * speed.Y), 0);
                        Console.WriteLine(speed + "  " + newSpeed1);
                        
                        //speed = -speed;
                        //position -= endPos1 - tempPos1;
                        speed = newSpeed1;
                        position += newSpeed1 * deltaT;

                        
                        var between2 = tangent - tempPos2;
                        var crossprod2 = 0.5 * (tempPos2.LengthSquared() + tangent.LengthSquared() - between2.LengthSquared());
                        var cosa2 = crossprod2 / (tempPos2.Length() * tangent.Length());
                        var a2 = Math.Acos(cosa2);
                        var newSpeed2 = new Vector3((float) (cosa2 * speed2.X - Math.Sin(a2) * speed2.Y),
                            (float) (Math.Sin(a2) * speed2.X + cosa2 * speed2.Y), 0);
                        part2.speed = newSpeed2;
                        part2.position += newSpeed2 * deltaT;

                        break;
                    }
                }
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