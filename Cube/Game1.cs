using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
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
        int seed;
        public bool isCollide { get; set; }

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
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            //graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            //camera init
            camera = new Camera(this, new Vector3(8, 8, 4), Vector3.Zero, new Vector3(0,1,0));
            Components.Add(camera);
            cubes = new List<Cube>();
            Random rand = new Random();
            seed = rand.Next(0, 5);

            PerlinNoise noise = new PerlinNoise(4);
            noise.Octaves = 4;

            for (int x = 0; x < 25; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    for (int z = 0; z < 25; z++)
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

            var terra = Content.Load<Texture2D>("terra");
            var gramaLado = Content.Load<Texture2D>("grama");
            var gramaTop = Content.Load<Texture2D>("gramaTop");

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

            foreach (var cube in cubes)
            {
                if (cube.position == camera.cameraPosition)
                    isCollide = true;
                else                
                    isCollide = false;                
            }

            //angles the cube for auto spin
            //worldRotation *= Matrix.CreateFromYawPitchRoll(MathHelper.PiOver4 / 60, 0, 0);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

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

            spriteBatch.Begin();

            spriteBatch.DrawString(spriteFont, $"Seed: {seed} Jump: {camera.IsJump} IsCollide:{isCollide}", new Vector2(1, 1), Color.Black);
            spriteBatch.DrawString(spriteFont, $"Camera:{camera.cameraPosition}", new Vector2(10, 20), Color.Black);
            spriteBatch.DrawString(spriteFont, $"View:{camera.view}", new Vector2(15, 1), Color.Black);

            spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}