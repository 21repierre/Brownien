using System;
using Brownien.Particules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Brownien {
    public class Game1 : Game {
        public static GraphicsDeviceManager graphics;
        public static bool stopDrawing;

        public static Game1 instance;
        private readonly int bigParticules = 1;
        private readonly int smallParticule = 10;
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
            Particule.particules = new Particule[10];

            graphics.PreferredBackBufferWidth = 1280; // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = 720; // set this value to the desired height of your window
            graphics.ApplyChanges();


            var screenScalingFactor = new Vector3(1, 1, 1);
            globalTransformation = Matrix.CreateScale(screenScalingFactor);

            base.Initialize();
        }

        protected override void LoadContent() {
            /*var big1 = new Big(new Vector3(100,100f,0));
            var big2 = new Big(new Vector3(300, 300, 0));
            var big3 = new Big(new Vector3(200, 200, 0));
            
            big1.texture = Content.Load<Texture2D>("pictures/big");
            big2.texture = Content.Load<Texture2D>("pictures/big_blue");
            big3.texture = Content.Load<Texture2D>("pictures/big_blue");*/

            var t1 = Content.Load<Texture2D>("pictures/big");
            var t2 = Content.Load<Texture2D>("pictures/big_blue");

            for (var i = 0; i < Particule.particules.Length; i++) {
                var b = new Big(new Vector3(i * 50, i * 50, 0));
                b.texture = new Random().Next(0, 2) == 0 ? t1 : t2;
            }

            drawer = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            foreach (var particule in Particule.particules) {
                particule.update(gameTime);
                particule.calcCollision();
            }

            base.Update(gameTime);
        }

        public void forceDraw() {
            Draw(new GameTime(new TimeSpan(0), TimeSpan.Zero));
        }

        protected override void Draw(GameTime gameTime) {
            if (stopDrawing) return;
            GraphicsDevice.Clear(Color.CornflowerBlue);

            drawer.Begin(SpriteSortMode.Immediate, null, null, null, null, null, globalTransformation);

            foreach (var particule in Particule.particules) {
                var texture = particule.getTexture();
                if (texture != null)
                    drawer.Draw(texture, new Vector2(particule.position.X, particule.position.Y),
                        null, Color.White, 0, new Vector2(1, 1), 1f, 0, 0);
            }

            drawer.End();

            base.Draw(gameTime);
        }
    }
}