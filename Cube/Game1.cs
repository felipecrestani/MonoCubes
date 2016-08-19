using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Cube
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;

        //camera
        Camera camera;

        //world vars
        Matrix worldTranslation = Matrix.Identity;
        Matrix worldRotation = Matrix.Identity;

        //cube stuff
        List<Cube> cubes;
        BasicEffect effect;

        //cube texture array
        Texture2D[] tex = new Texture2D[6];

        //Frame
        FrameCounter _frameCounter = new FrameCounter();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            //camera init
            camera = new Camera(this, new Vector3(250, 8, 10), Vector3.Zero, new Vector3(0,1,0));
            Components.Add(camera);
            cubes = new List<Cube>();

            PerlinNoise noise = new PerlinNoise(46544);
            noise.Octaves = 4;

            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    for (int z = 0; z < 2; z++)
                    {
                        if (y == 0)
                        {
                            Cube cube = new Cube(new Vector3(2, 2, 2), new Vector3(x * 4, y * 4, z * 4));
                            cube.buildCube();
                            cubes.Add(cube);
                        }
                        else if (noise.Compute(x, y, z) > 0)
                        {
                            Cube cube = new Cube(new Vector3(2, 2, 2), new Vector3(x * 4, y * 4, z * 4));
                            cube.buildCube();
                            cubes.Add(cube);
                        }
                    }
                }
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>("font");

            var terra = Content.Load<Texture2D>("paiva");
            var gramaLado = Content.Load<Texture2D>("paiva");
            var gramaTop = Content.Load<Texture2D>("paiva");

            tex[0] = gramaLado;
            tex[1] = gramaLado;
            tex[2] = gramaLado;
            tex[3] = gramaLado;
            tex[4] = gramaTop;
            tex[5] = terra;
        }

        protected override void UnloadContent()
        {
            //dispose the array of textures
            for (int i = 0; i < 6; ++i)
            {
                tex[i].Dispose();
            }
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            Window.Title = $"X {camera.cameraPosition.X} Y {camera.cameraPosition.Y} Z {camera.cameraPosition.Z}";

            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Left))
            {
                //worldTranslation *= Matrix.CreateTranslation(-.03f, 0, 0);
            }
            if (ks.IsKeyDown(Keys.Right))
            {
                //worldTranslation *= Matrix.CreateTranslation(.03f, 0, 0);
            }
            if (ks.IsKeyDown(Keys.Up))
            {
                //angle += .01f;
            }
            if (ks.IsKeyDown(Keys.Down))
            {
                //angle -= .01f;
            }
            //angles the cube for auto spin
            worldRotation *= Matrix.CreateFromYawPitchRoll(MathHelper.PiOver4 / 60, 0, 0);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            effect = new BasicEffect(GraphicsDevice);
            effect.World = worldRotation * worldTranslation;
            effect.View = camera.view;
            effect.Projection = camera.projection;

            foreach (var cube in cubes)
            {
                //loads a side, changes texture, loads next side, repeat
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    effect.Texture = tex[0];
                    effect.TextureEnabled = true;
                    pass.Apply();
                    GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, cube.ffront, 0, 2);
                }

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    effect.Texture = tex[1];
                    effect.TextureEnabled = true;
                    pass.Apply();
                    GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, cube.bback, 0, 2);
                }

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    effect.Texture = tex[2];
                    effect.TextureEnabled = true;
                    pass.Apply();
                    GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, cube.lleft, 0, 2);
                }

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    effect.Texture = tex[3];
                    effect.TextureEnabled = true;
                    pass.Apply();
                    GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, cube.rright, 0, 2);
                }

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    effect.Texture = tex[4];
                    effect.TextureEnabled = true;
                    pass.Apply();
                    GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, cube.ttop, 0, 2);
                }

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    effect.Texture = tex[5];
                    effect.TextureEnabled = true;
                    pass.Apply();
                    GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, cube.bbot, 0, 2);
                }
            }

            //var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //_frameCounter.Update(deltaTime);

            //var fps = string.Format("FPS: {0}", _frameCounter.AverageFramesPerSecond);

            //spriteBatch.Begin();

            //spriteBatch.DrawString(spriteFont, fps, new Vector2(1, 1), Color.Black);       

            //spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}