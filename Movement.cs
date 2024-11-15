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
    const float gravity = -20f;
    const float cameraSpeed = 10f * 10f;
    const float airControlPenalty = .3f;
    const float heightSlices = 1f / 20f;
    const float playerFatness = .75f;
    const float playerHeight = 1.75f;



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

        float airControl;
        bool Grounded = colMan.CalculateRaycastInMap(cam.Position - new Vector3(0, playerHeight, 0), new(0, -1, 0));
        //colMan.DrawRay(cam.Position - new Vector3(0, 2, 0), new (0, -1, 0));

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
        bool TopBlocked = colMan.CalculateRaycastInMap(cam.Position + new Vector3(0f, .25f, 0f), new Vector3(0, 1, 0));

        // Check inputs and move and stuff
        Vector3 moveDirection = new();

        if (input.IsKeyDown(Keys.W)) moveDirection += flatFront;
        if (input.IsKeyDown(Keys.S)) moveDirection -= flatFront;
        if (input.IsKeyDown(Keys.A)) moveDirection -= flatRight;
        if (input.IsKeyDown(Keys.D)) moveDirection += flatRight;

        if (input.IsKeyDown(Keys.Space) && Grounded) Acceleration += new Vector3(0, 1, 0) * 10f / (float)e.Time;
        if (input.IsKeyDown(Keys.LeftShift)) Acceleration -= new Vector3(0, 1, 0) * 5 / (float)e.Time;

        float newMoveSpeed = 1;
        if (moveDirection.Length > 0)
        {
            moveDirection.Normalize();
            for (float h = 0; h <= playerHeight; h += heightSlices)
            {
                bool moveBlocked = colMan.CalculateRaycastInMapNormal(cam.Position - new Vector3(0, h, 0), playerFatness * moveDirection, out Vector3 normal);
                if (moveBlocked)
                {
                    newMoveSpeed = Vector3.Dot(moveDirection, normal);
                    if (newMoveSpeed <= 0)
                    {
                        moveDirection -= normal;
                        moveDirection.Normalize();
                        moveDirection *= newMoveSpeed;
                    }

                    float newVel = Vector3.Dot(Velocity, normal);
                    if (newVel <= 0)
                    {
                        Vector3 newDir = Velocity.Normalized() + normal;
                        Velocity = -newVel * newDir;
                        break;
                    }
                }
            }
        }

        if (Velocity.Length > 0)
        {
            Vector3 velDir = Velocity.Normalized();
            for (float h = 0; h <= playerHeight; h += heightSlices)
            {
                bool moveBlocked = colMan.CalculateRaycastInMapNormal(cam.Position - new Vector3(0, h, 0), playerFatness * velDir, out Vector3 normal);
                if (moveBlocked)
                {
                    float newVel = Vector3.Dot(Velocity, normal);
                    if (newVel <= 0)
                    {
                        Vector3 newDir = Velocity.Normalized() + normal;
                        Velocity = -newVel * newDir;
                        break;
                    }
                }
            }
        }

        Acceleration += moveDirection * cameraSpeed * airControl;

        Velocity.X = Velocity.X.Clamp(-10, 10);
        Velocity.Y = Velocity.Y.Clamp(-30, 30);
        Velocity.Z = Velocity.Z.Clamp(-10, 10);

        Velocity += Acceleration * (float)e.Time;

        if (TopBlocked) Velocity.Y = -1;

        cam.Position += Velocity * (float)e.Time;


        if (Grounded)
        {
            Velocity.X *= .96f;
            Velocity.Z *= .96f;
        }
        else
        {
            Velocity.X *= .999f;
            Velocity.Z *= .999f;
        }
    }
}