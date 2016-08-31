using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Cube
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Camera : GameComponent
    {
        public bool isCollide { get; set; }
        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }

        public Vector3 cameraPosition;
        Vector3 cameraDirection;
        Vector3 cameraUp;
        Vector3 oldCameraPosition;

        public List<BoundingSphere> cubes { get; set; }

        //defines speed of camera movement
        float speed = 0.5f;

        MouseState prevMouseState;

        //Jump
        const int GROUND_POSITION = 8;
        const int JUMP_SIZE = 18;
        public bool IsJump { get; set; }
        bool HighJump { get; set; }

        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up)
            : base(game)
        {
            // Build camera view matrix
            cameraPosition = pos;
            cameraDirection = target - pos;
            cameraDirection.Normalize();
            cameraUp = up;
            CreateLookAt();

            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)Game.Window.ClientBounds.Width / (float)Game.Window.ClientBounds.Height, 0.001f, 1000f);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            // Set mouse position and do initial get state
            Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);

            prevMouseState = Mouse.GetState();

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            var key = Keyboard.GetState();


            // Move forward and backward
            if (key.IsKeyDown(Keys.Up) || key.IsKeyDown(Keys.W)) cameraPosition += cameraDirection * speed;
            if (key.IsKeyDown(Keys.Down) || key.IsKeyDown(Keys.S)) cameraPosition -= cameraDirection * speed;
            if (key.IsKeyDown(Keys.Left) || key.IsKeyDown(Keys.A)) cameraPosition += Vector3.Cross(cameraUp, cameraDirection) * speed;
            if (key.IsKeyDown(Keys.Right) || key.IsKeyDown(Keys.D)) cameraPosition -= Vector3.Cross(cameraUp, cameraDirection) * speed;            

            // Rotation in the world
            cameraDirection = Vector3.Transform(cameraDirection, Matrix.CreateFromAxisAngle(cameraUp, (-MathHelper.PiOver4 / 150) * (Mouse.GetState().X - prevMouseState.X)));
            cameraDirection = Vector3.Transform(cameraDirection, Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUp, cameraDirection), (MathHelper.PiOver4 / 100) * (Mouse.GetState().Y - prevMouseState.Y)));

            BoundingSphere cameraBounding = new BoundingSphere(cameraPosition,3.2f);

            foreach (var cube in cubes)
            {
                if (cameraBounding.Intersects(cube))
                {
                    isCollide = true;
                    break;
                }
                else
                {
                    isCollide = false;
                }
            }

            if (!isCollide)
                oldCameraPosition = cameraPosition;

            if (key.IsKeyDown(Keys.Space))
                IsJump = true;

            if(IsJump)
            {
                Jump();
            }
            else
            {
                Vector3 fixY = cameraPosition;
                fixY.Y = GROUND_POSITION;
                cameraPosition = fixY;
            }
            
            // Reset prevMouseState
            prevMouseState = Mouse.GetState();

            //Camera Move Limit
            if (cameraPosition.X > 100)
                cameraPosition.X = 100;

            if (cameraPosition.X < 0)
                cameraPosition.X = 0;

            if (cameraPosition.Z > 100)
                cameraPosition.Z = 100;

            if (cameraPosition.Z < 0)
                cameraPosition.Z = 0;

            if (isCollide)
                cameraPosition = oldCameraPosition;

            CreateLookAt();

            base.Update(gameTime);
        }

        private void CreateLookAt()
        {
            view = Matrix.CreateLookAt(cameraPosition, cameraPosition + cameraDirection, cameraUp);
        }

        public void Jump()
        {
            if (cameraPosition.Y < JUMP_SIZE && !HighJump)
                cameraPosition.Y += 0.5f;

            if (cameraPosition.Y >= JUMP_SIZE)
            {
                HighJump = true;
            }

            if (HighJump)
            {
                cameraPosition.Y -= 0.3f;
                if (cameraPosition.Y < GROUND_POSITION)
                {
                    IsJump = false;
                    HighJump = false;
                }
            }
        }

    }
}