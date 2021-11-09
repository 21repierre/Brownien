using System;
using System.Threading;
using Brownien.Particules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Brownien {
    public class Game1 : Game {
        public static GraphicsDeviceManager graphics;

        public static Game1 instance;
        private readonly int numOfParticules = 1024;
        private SpriteBatch drawer;
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
            graphics.PreferredBackBufferHeight = 300; // set this value to the desired height of your window
            graphics.ApplyChanges();


            var screenScalingFactor = new Vector3(1, 1, 1);
            globalTransformation = Matrix.CreateScale(screenScalingFactor);

            base.Initialize();
        }

        protected override void LoadContent() {
            Target.texture = Content.Load<Texture2D>("pictures/target");

            var t1 = Content.Load<Texture2D>("pictures/big");
            var t2 = Content.Load<Texture2D>("pictures/big_blue");
            const float size = 8f;
            var res = (int) Math.Floor(graphics.PreferredBackBufferWidth / (2 * size));
            var resh = (int) Math.Floor(graphics.PreferredBackBufferHeight / (2 * size));
            for (var i = 0; i < numOfParticules; i++) {
                // var x = i % res * 2 * size;
                // var y = (float) (Math.Floor(i * 2 * size / graphics.PreferredBackBufferWidth) * 2 * size);
                var x = (float) (Math.Floor(i * 2 * size / graphics.PreferredBackBufferHeight) * 2 * size);
                var y = i % resh * 2 * size;
                //Console.WriteLine(x + " - " + y);
                var b = new Big(new Vector2(x, y)) {
                    texture = new Random().Next(0, 2) == 0 ? t1 : t2, 
                    mass = .5f + (float) (new Random().NextDouble() * 1f), 
                    size = size / 2
                };
                // * b.mass;
            }

            var target = new Target(new Vector2(1500, 200)) {
                size = 80f,
                mass = 1e2f,
            };

            drawer = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Window.Title = "Brownien - " + Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds) + " FPS";

            for (var i = 0; i < 15; i++)
                new Thread(() => { Particule.update((float) gameTime.ElapsedGameTime.TotalSeconds, i); }).Start();

            base.Update(gameTime);
        }

        public void forceDraw() {
            Draw(new GameTime(new TimeSpan(0), TimeSpan.Zero));
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            drawer.Begin(SpriteSortMode.Immediate, null, null, null, null, null, globalTransformation);

            foreach (var particule in Particule.particules) {
                var texture = particule.getTexture();
                if (texture != null)
                    drawer.Draw(texture, new Vector2(particule.position.X, particule.position.Y),
                        null, Color.White, 0, new Vector2(1, 1), particule.size / 64, 0, 0);
            }

            drawer.End();

            base.Draw(gameTime);
        }
    }
}