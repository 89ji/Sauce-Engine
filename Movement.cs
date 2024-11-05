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
    const float gravity = -9.8f;
    const float cameraSpeed = 10f * 10f;
    const float airControlPenalty = .3f;



    Vector3 Velocity = new();
    public Movement(Camera cam, MapObjList objects, MapObjList immObjs)
    {
        this.cam = cam;
        objs = objects;
        imObjList = immObjs;
    }

    public void Process(KeyboardState input, FrameEventArgs e)
    {
        Vector3 Acceleration = new(0, gravity, 0);

        float airControl;
        bool Grounded = ComputeCollision(cam.Position.ToSauceCoord3d(), new Direction3d(0, -1, 0));

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
        var flatFront = cam.Front;
        var flatRight = cam.Right;
        flatFront.Y = 0;
        flatFront.Normalize();
        flatRight.Y = 0;
        flatRight.Normalize();

        // Checking collisions for movement and stuff
        //bool FrontBlocked = ComputeCollision(cam.Position, flatFront.Normalized() * .5f);
        //bool BackBlocked = ComputeCollision(cam.Position, flatFront.Normalized() * -.5f);
        //bool RightBlocked = ComputeCollision(cam.Position, flatRight.Normalized() * .5f);
        //bool LeftBlocked = ComputeCollision(cam.Position, flatRight.Normalized() * -.5f);
        bool TopBlocked = ComputeCollision(cam.Position.ToSauceCoord3d() + new Direction3d(0f, 3f, 0f), new Direction3d(0, 1f, 0), true);
        if (TopBlocked) throw new Exception("Top blocked!");

        // Check inputs and move and stuff
        if (input.IsKeyDown(Keys.W)) Acceleration += flatFront * cameraSpeed * airControl;
        if (input.IsKeyDown(Keys.S)) Acceleration -= flatFront * cameraSpeed * airControl;
        if (input.IsKeyDown(Keys.A)) Acceleration -= flatRight * cameraSpeed * airControl;
        if (input.IsKeyDown(Keys.D)) Acceleration += flatRight * cameraSpeed * airControl;
        if (input.IsKeyDown(Keys.Space) && Grounded) Acceleration += new Vector3(0, 1, 0) * 10f / (float)e.Time;
        if (input.IsKeyDown(Keys.LeftShift)) Acceleration -= new Vector3(0, 1, 0) * 5 / (float)e.Time;

        Velocity.X = Velocity.X.Clamp(-10, 10);
        Velocity.Y = Velocity.Y.Clamp(-30, 30);
        Velocity.Z = Velocity.Z.Clamp(-10, 10);


        Velocity += Acceleration * (float)e.Time;

        if (TopBlocked && Velocity.Y > 0) Velocity.Y = -1;
        Console.WriteLine(TopBlocked);

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

    const float cLowBound = -.5f;
    const float cHighBound = .5f;
    const float cPadding = .5f;
    const float playerHeight = 2;
    bool ComputeCollision(Position3d position, Direction3d direction, bool Debug = false)
    {
        bool CollisionFound = false;
        foreach (var obj in objs)
        {
            if (obj is Brush brush)
            {
                Transform transf = new Transform(brush.transform);
                transf.RotateTo(new(-transf.Rotation.X, -transf.Rotation.Y, -transf.Rotation.Z));
                Matrix4 trans = transf.GetMat().ToGLMat4();
                Matrix4 arcTrans = trans.Inverted();

                Position3d localPos = arcTrans.TransformPoint(position);
                Direction3d localDir = arcTrans.TransformPoint(direction);

                // For drawing the extents of each collidable brush
                /*
                for(float i =- .5f; i <= .5; i += .25f)
                for(float j =- .5f; j <= .5; j += .25f)
                for(float k =- .5f; k <= .5; k += .25f)
                {
                    Vector3 point = new(i, j, k);
                    point = Multiply(trans, point);
                    DrawMarker(point);
                }
                */

                for (float i = 0; i <= 1; i += .01f)
                {
                    Position3d sample = localPos + (i * localDir);
                    bool xInBounds = sample.X > cLowBound - (cPadding / brush.GetScale.X) && sample.X < cHighBound + (cPadding / brush.GetScale.X);
                    bool yInBounds = sample.Y - (playerHeight / brush.GetScale.Y) > cLowBound && sample.Y - (playerHeight / brush.GetScale.Y) < cHighBound;
                    bool zInBounds = sample.Z > cLowBound - (cPadding / brush.GetScale.Z) && sample.Z < cHighBound + (cPadding / brush.GetScale.Z);

                    if (Debug)
                    {
                        sample = trans.TransformPoint(sample);
                        //sample.Y -= 2;
                        DrawMarker(sample);
                    }
                    if (xInBounds && yInBounds && zInBounds)
                    {
                        if (Debug)
                        {
                            sample = trans.TransformPoint(sample);
                            //sample.Y -= 2;
                            DrawMarker(sample);
                        }
                        CollisionFound = true;
                    }
                }
            }
        }
        if (Debug) DrawMarker(position + direction);
        return CollisionFound;
    }

    void DrawMarker(Vector3 Location, float Size = .2f)
    {
        imObjList.AddMapObject(new Brush(new Transform(SysVec3.Zero, Location.ToSysVec3(), new SysVec3(Size, Size, Size))));
    }

    void DrawMarker(Position3d Location, float Size = .2f)
    {
        imObjList.AddMapObject(new Brush(new Transform(SysVec3.Zero, Location.ToSysVec3(), new SysVec3(Size, Size, Size))));
    }
}