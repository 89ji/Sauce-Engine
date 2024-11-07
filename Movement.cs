using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Sauce_Engine;
using Sauce_Engine.Types;
using Sauce_Engine.Util;
using Sauce_Engine.Numerics;

using Matrix4x4 = System.Numerics.Matrix4x4;
using SysVec3 = System.Numerics.Vector3;


class Movement
{
    Camera cam;
    MapObjList objs;
    MapObjList imObjList;
    CollisionManager colMan;
    const float gravity = -9.8f;
    const float cameraSpeed = 10f * 10f;
    const float airControlPenalty = .3f;
    const float slideMeasureGap = .1f;



    Vector3 Velocity = new();
    public Movement(Camera cam, MapObjList objects, MapObjList immObjs)
    {
        this.cam = cam;
        objs = objects;
        imObjList = immObjs;
        colMan = new(objs, immObjs);
    }

    public void Process(KeyboardState input, FrameEventArgs e)
    {
        Vector3 Acceleration = new(0, gravity, 0);
        Acceleration.Y = 0;

        float airControl;
        bool Grounded = colMan.CalculateRaycastInMap(cam.Position - new Vector3(0, -2, 0), new (0, -1, 0));

        if (Grounded)
        {
            Velocity.Y = 0;
            Acceleration.Y = 0;
            airControl = 1;
        }
        else
        {
            airControl = airControlPenalty;
        }

        // Eliminate flying from the cameras range of movement
        Vector3 flatFront = cam.Front;
        Vector3 flatRight = cam.Right;
        flatFront.Y = 0;
        flatFront.Normalize();
        flatRight.Y = 0;
        flatRight.Normalize();

        // Checking collisions for movement and stuff
        //bool FrontBlocked = ComputeCollision(cam.Position, flatFront.Normalized() * .5f);
        //bool BackBlocked = ComputeCollision(cam.Position, flatFront.Normalized() * -.5f);
        //bool RightBlocked = ComputeCollision(cam.Position, flatRight.Normalized() * .5f);
        //bool LeftBlocked = ComputeCollision(cam.Position, flatRight.Normalized() * -.5f);
        bool TopBlocked = false;//colMan.CalculateRaycastInMap(cam.Position + new Vector3(0f, 3f, 0f), new Vector3(0, 1, 0));
        if (TopBlocked) throw new Exception("Top blocked!");

        // Check inputs and move and stuff
        Vector3 moveDirection = new();

        if (input.IsKeyDown(Keys.W)) moveDirection += flatFront;
        if (input.IsKeyDown(Keys.S)) moveDirection -= flatFront;
        if (input.IsKeyDown(Keys.A)) moveDirection -= flatRight;
        if (input.IsKeyDown(Keys.D)) moveDirection += flatRight;

        if (input.IsKeyDown(Keys.Space) && Grounded) Acceleration += new Vector3(0, 1, 0) * 10f / (float)e.Time;
        if (input.IsKeyDown(Keys.LeftShift)) Acceleration -= new Vector3(0, 1, 0) * 5 / (float)e.Time;

        /*
        if (moveDirection.Length > 0)
        {
            moveDirection.Normalize();

            Matrix4 rotate90 = Matrix4.CreateFromAxisAngle(Vector3.UnitY, 90f.toDeg());
            Vector3 rightMovement = rotate90.TransformPoint(moveDirection);

            
            float frontBlock = ComputeCollisionDistance(cam.Position + new Vector3(0, .25f, 0) + .25f * moveDirection, moveDirection * 2);
            float rightExtent = ComputeCollisionDistance(cam.Position + new (0, .25f, 0) + rightMovement * slideMeasureGap - moveDirection, moveDirection * 5, true);
            float leftExtent = ComputeCollisionDistance(cam.Position + new (0, .25f, 0) - rightMovement * slideMeasureGap - moveDirection, moveDirection * 5, true);
            if (!float.IsInfinity(frontBlock))
            {
                float Slope;

                switch (float.IsInfinity(leftExtent), float.IsInfinity(rightExtent))
                {
                    case (true, true):
                        Slope = 0;
                        break;
                    case (false, true):
                        Slope = -4;
                        break;
                    case (true, false):
                        Slope = 4;
                        break;
                    case (_, _):
                        float rise = rightExtent - leftExtent;
                        Slope = rise / (2 * slideMeasureGap);
                        break;
                }

                float theta = MathF.Atan(Slope);
                Console.WriteLine($"left: {leftExtent} right: {rightExtent} front: {frontBlock} theta: {theta} slope: {Slope}");
                


                moveDirection = new Vector3();
                Velocity.X = 0;
                Velocity.Z = 0;
            }
        }
        */

        /*
        if (Velocity.Length > 0)
        {
            var velDir = Velocity.Normalized();
            bool frontBlock = ComputeCollision(cam.Position + new Vector3(0, 1, 0) + velDir * .2f, velDir);
            bool leftExtent = ComputeCollision(cam.Position + new Vector3(0, 1, 0) - flatRight + velDir * .2f, velDir);
            if (frontBlock)
            {
                moveDirection = new Vector3();
                Velocity.X = 0;
                Velocity.Z = 0;
            }
        }*/

        Acceleration += moveDirection * cameraSpeed * airControl;

        Velocity.X = Velocity.X.Clamp(-10, 10);
        Velocity.Y = Velocity.Y.Clamp(-30, 30);
        Velocity.Z = Velocity.Z.Clamp(-10, 10);

        Velocity += Acceleration * (float)e.Time;

        if (TopBlocked) Velocity.Y = -1;

        cam.Position += Velocity * (float)e.Time;


        if (Grounded)
        {
            Velocity.X *= .98f;
            Velocity.Z *= .98f;
        }
        else
        {
            Velocity.X *= .999f;
            Velocity.Z *= .999f;
        }
    }        
}