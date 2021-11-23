using System;
using System.IO;
using System.Threading;
using Brownien.Particules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Brownien {
    public class Game1 : Game {
        public static GraphicsDeviceManager graphics;

        public static Game1 instance;
        public static bool end;
        private readonly float minAbsRate = 0.1f;
        private readonly int numOfParticules = 1024;
        private SpriteBatch drawer;
        public float endTime;
        private Matrix globalTransformation;

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            instance = this;
        }

        protected override void Initialize() {
            //Particule.particules = new Particule[bigParticules + smallParticule];
            Particule.particules = new Particule[numOfParticules + 1];

            graphics.PreferredBackBufferWidth = 1920; // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = 1000; // set this value to the desired height of your window
            graphics.ApplyChanges();


            var screenScalingFactor = new Vector3(1, 1, 1);
            globalTransformation = Matrix.CreateScale(screenScalingFactor);

            base.Initialize();
        }

        protected override void LoadContent() {
            Target.texture = Content.Load<Texture2D>("pictures/target");
            ColorTexture.ARIAL_50 = Content.Load<SpriteFont>("arial_50");
            ColorTexture.ARIAL_20 = Content.Load<SpriteFont>("arial_20");
            ColorTexture.load(GraphicsDevice);

            var t1 = Content.Load<Texture2D>("pictures/big");
            var t2 = Content.Load<Texture2D>("pictures/big_blue");

            /*
             * Génération des particules
             */

            const float size = 8f;
            var res = (int) Math.Floor(graphics.PreferredBackBufferWidth / (2 * size));
            var resh = (int) Math.Floor(graphics.PreferredBackBufferHeight / (2 * size));
            var middle = (numOfParticules - 1) * 4 * size * size / graphics.PreferredBackBufferHeight;
            Console.WriteLine(resh);
            for (var i = 0; i < numOfParticules; i++) {
                // var x = i % res * 2 * size;
                // var y = (float) (Math.Floor(i * 2 * size / graphics.PreferredBackBufferWidth) * 2 * size);
                var x = (graphics.PreferredBackBufferWidth / 2 - middle / 2) + (float) (Math.Floor(i * 2 * size / graphics.PreferredBackBufferHeight) * 2 * size);
                var y = i % resh * 2 * size;
                //Console.WriteLine(x + " - " + y);
                var b = new Big(new Vector2(x, y)) {
                    texture = new Random().Next(0, 2) == 0 ? t1 : t2,
                    mass = .5f + (float) (new Random().NextDouble() * 1f),
                    size = size / 2
                };
                // * b.mass;
            }

            /*
             * Génération des cilbes
             */
            var target = new Target(new Vector2(1500, 200)) {
                size = 80f,
                mass = 1e2f
            };

            drawer = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Window.Title = "Brownien - " + Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds) + " FPS";
            if (Particule.absorbedCount < numOfParticules * minAbsRate) {
                for (var i = 0; i < 15; i++)
                    new Thread(() => { Particule.update((float) gameTime.ElapsedGameTime.TotalSeconds, i); }).Start();
            } else if (!end) {
                endTime = (float) Math.Round(gameTime.TotalGameTime.TotalSeconds, 1);
                end = true;

                var origins = "";
                var targets = "";
                var totSpeeds = Vector2.Zero;

                foreach (var particule in Particule.particules) {
                    if (particule.GetType() == typeof(Big)) {
                        if (!particule.absorbed) {
                            var distFromOrigin = Math.Round((particule.center - particule.origin).Length(), 0).ToString();
                            origins += distFromOrigin + "\n";
                            totSpeeds += particule.speed;
                        }

                        var distFromTarg = particule.absorbed ? "0" : Math.Round((Particule.particules[numOfParticules].center - particule.center).Length(), 0).ToString();
                        targets += distFromTarg + "\n";
                    }
                }

                Console.WriteLine(totSpeeds);
                File.AppendAllText("../../../datas/origins.txt", origins);
                File.AppendAllText("../../../datas/targets.txt", targets);
                File.AppendAllText("../../../datas/speeds.txt", totSpeeds.Length().ToString() + "\n");
            }

            base.Update(gameTime);
        }

        public void forceDraw() {
            Draw(new GameTime(new TimeSpan(0), TimeSpan.Zero));
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            drawer.Begin(SpriteSortMode.Immediate, null, null, null, null, null, globalTransformation);

            // Affichage de la simulation
            foreach (var particule in Particule.particules) {
                if (particule.absorbed) continue;

                var texture = particule.getTexture();
                if (texture != null) {
                    var dsize = particule.size / 2;
                    drawer.Draw(texture, new Vector2(particule.position.X - dsize, particule.position.Y - dsize),
                        null, Color.White, 0, new Vector2(0, 0), particule.size / 64, 0, 0);
                }
            }

            // Affichage du temps
            drawer.DrawString(ColorTexture.ARIAL_20, "Temps: " + (end ? endTime : (float) Math.Round(gameTime.TotalGameTime.TotalSeconds, 1)) + "s", new Vector2(10, 10), Color.White);
            drawer.DrawString(ColorTexture.ARIAL_20, "Avancement: " + Math.Min(Math.Round(Particule.absorbedCount / (numOfParticules * minAbsRate) * 100, 1), 100) + "%", new Vector2(10, 30), Color.White);

            // Simulation finie
            if (end) {
                var str = "Le patient est guéri";
                var size = ColorTexture.ARIAL_50.MeasureString(str);
                var bounds = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight) - size;
                drawer.DrawString(ColorTexture.ARIAL_50, str, bounds / 2, Color.White);
            }

            drawer.End();
            base.Draw(gameTime);
        }
    }
}