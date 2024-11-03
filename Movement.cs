using Matrix4x4 = System.Numerics.Matrix4x4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Sauce_Engine;
using Sauce_Engine.Types;
using Sauce_Engine.Util;
using Vec3 = System.Numerics.Vector3;

class Movement
{
    Camera cam;
    MapObjList objs;
    public MapObjList imObjList;
    const float gravity = -9.8f;
    const float cameraSpeed = 10f * 10f;
    const float airControlPenalty = .3f;



    Vector3 Velocity = new();
    public Movement(Camera cam, MapObjList objects)
    {
        this.cam = cam;
        objs = objects;
    }

    public void Process(KeyboardState input, FrameEventArgs e)
    {
        Console.Clear();
        Vector3 Acceleration = new(0, gravity, 0);

        float airControl;
        bool Grounded = ComputeCollision(cam.Position, new Vector3(0, -1, 0));

        if(Grounded) 
        {
            Velocity.Y = 0;
            Acceleration.Y = 0;
            airControl = 1;
        }
        else
        {
            airControl = airControlPenalty;
        }
        Console.WriteLine($"Grounded: {Grounded}");
        
        // Eliminate flying from the cameras range of movement
        var flatFront = cam.Front;
        var flatRight = cam.Right;
        flatFront.Y = 0;
        flatFront.Normalize();
        flatRight.Y = 0;
        flatRight.Normalize();

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
        cam.Position += Velocity * (float)e.Time;

        if(Grounded)
        {
            Velocity.X *= .98f;
            Velocity.Z *= .98f;
        }
        else
        {
            Velocity.X *= .999f;
            Velocity.Z *= .999f;
        }

        //Console.WriteLine($"X:{Velocity.X:F2} Y:{Velocity.Y:F2} Z:{Velocity.Z:F2}");
    }

    const float cLowBound = -.5f;
    const float cHighBound = .5f;
    const float cPadding = .5f;
    const float playerHeight = 2;
    bool ComputeCollision(Vector3 position, Vector3 direction)
    {
        bool CollisionFound = false;
        foreach (var obj in objs)
        {
            if(obj is Brush brush)
            {
                Transform transf = new Transform(brush.transform);
                transf.RotateTo(new (-transf.Rotation.X, -transf.Rotation.Y, -transf.Rotation.Z));
                Matrix4 trans = transf.GetMat().ToGLMat4();
                Matrix4 arcTrans = trans.Inverted();

                var localPos = Multiply(arcTrans, position);
                var localVel = Multiply(arcTrans, direction);

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
                    var sample = localPos + i * localVel;
                    bool xInBounds = sample.X > cLowBound - (cPadding / brush.GetScale.X) && sample.X < cHighBound + (cPadding / brush.GetScale.X);
                    bool yInBounds = sample.Y - (playerHeight/brush.GetScale.Y) > cLowBound && sample.Y - (playerHeight/brush.GetScale.Y) < cHighBound;
                    bool zInBounds = sample.Z > cLowBound - (cPadding / brush.GetScale.Z) && sample.Z < cHighBound + (cPadding / brush.GetScale.Z);
                    if (xInBounds && yInBounds && zInBounds) 
                    {
                        sample = Multiply(trans, sample);
                        sample.Y -= 2;
                        DrawMarker(sample);
                        CollisionFound = true;
                    }
                }
            }
        }
        return CollisionFound;
    }

    Vector3 Multiply(Matrix4 m, Vector3 rhs)
    {
        Vector4 res = new();
        res.X = m.M11 * rhs.X + m.M12 * rhs.Y + m.M13 * rhs.Z + m.M14;
        res.Y = m.M21 * rhs.X + m.M22 * rhs.Y + m.M23 * rhs.Z + m.M24;
        res.Z = m.M31 * rhs.X + m.M32 * rhs.Y + m.M33 * rhs.Z + m.M34;
        res.W = m.M41 * rhs.X + m.M42 * rhs.Y + m.M43 * rhs.Z + m.M44;
        return new (res.X/res.W, res.Y/res.W, res.Z/res.W);
    }

    void DrawMarker(Vector3 Location, float Size = .2f)
    {
        imObjList.AddMapObject(new Brush(new Transform(Vec3.Zero, Location.ToSysVec3(), new Vec3(Size, Size, Size))));
    }
}