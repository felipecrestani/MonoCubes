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

        //camera
        Camera camera;

        //world vars
        Matrix worldTranslation = Matrix.Identity;
        Matrix worldRotation = Matrix.Identity;

        //angle var
        float angle = 0;

        //cube stuff
        List<Cube> cubes;
        BasicEffect effect;

        //cube texture array
        Texture2D[] tex = new Texture2D[6];

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            //camera init
            camera = new Camera(this, new Vector3(0, 0, 25), Vector3.Zero, Vector3.Up);
            Components.Add(camera);

            cubes = new List<Cube>();

            //cube init
            for (int x = 0; x < 25; x++)
            {
                for (int z = 0; z < 25; z++)
                {
                    for (int y = 0; y < 5; y++)
                    {
                        Cube cube = new Cube(new Vector3(2, 2, 2), new Vector3(x*6, y*6, z*6));
                        cube.buildCube();
                        cubes.Add(cube);
                    }
                }
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //texture loads
            tex[0] = Content.Load<Texture2D>("terra");
            tex[1] = Content.Load<Texture2D>("terra");
            tex[2] = Content.Load<Texture2D>("terra");
            tex[3] = Content.Load<Texture2D>("terra");
            tex[4] = Content.Load<Texture2D>("terra");
            tex[5] = Content.Load<Texture2D>("terra");
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

            //keyboard movement - left/right, speed up/slow down spin
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Left))
            {
                worldTranslation *= Matrix.CreateTranslation(-.03f, 0, 0);
            }
            if (ks.IsKeyDown(Keys.Right))
            {
                worldTranslation *= Matrix.CreateTranslation(.03f, 0, 0);
            }
            if (ks.IsKeyDown(Keys.Up))
            {
                angle += .01f;
            }
            if (ks.IsKeyDown(Keys.Down))
            {
                angle -= .01f;
            }
            //angles the cube for auto spin
            worldRotation *= Matrix.CreateFromYawPitchRoll(MathHelper.PiOver4 / 60, angle, 0);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            effect = new BasicEffect(GraphicsDevice);

            effect.World = worldRotation * worldTranslation;
            effect.View = camera.view;
            effect.Projection = camera.project;
            effect.Texture = tex[0];
            effect.TextureEnabled = true;

            foreach (var item in cubes)
            {            
            //loads a side, changes texture, loads next side, repeat
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, item.ffront, 0, 2);
                    effect.Texture = tex[1];
                    effect.TextureEnabled = true;

                    GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, item.bback, 0, 2);
                    effect.Texture = tex[2];
                    effect.TextureEnabled = true;

                    GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, item.lleft, 0, 2);
                    effect.Texture = tex[3];
                    effect.TextureEnabled = true;

                    GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, item.rright, 0, 2);
                    effect.Texture = tex[4];
                    effect.TextureEnabled = true;

                    GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, item.ttop, 0, 2);
                    effect.Texture = tex[5];
                    effect.TextureEnabled = true;

                    GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, item.bbot, 0, 2);

                }
            }
            base.Draw(gameTime);
        }
    }
}