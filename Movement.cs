using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Sauce_Engine;
using Sauce_Engine.Types;
using Sauce_Engine.Util;

class Movement
{
    Camera cam;
    MapObjList objs;
    const float gravity = -9.8f;
    const float cameraSpeed = 2.5f * 10f;
    const float airControlPenalty = .1f;



    Vector3 Velocity = new();
    public Movement(Camera cam, MapObjList objects)
    {
        this.cam = cam;
        objs = objects;
    }

    public void Process(KeyboardState input, FrameEventArgs e)
    {
        Vector3 Acceleration = new(0, gravity, 0);

        float airControl;

        // TODO: check groundednes
        bool Grounded = ComputeCollision(cam.Position, new Vector3(0, -1, 0));

        if(Grounded) 
        {
            Velocity.Y = 0;
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

        // Check inputs and move and stuff
        if (input.IsKeyDown(Keys.W)) Acceleration += flatFront * cameraSpeed * airControl;
        if (input.IsKeyDown(Keys.S)) Acceleration -= flatFront * cameraSpeed * airControl;
        if (input.IsKeyDown(Keys.A)) Acceleration -= flatRight * cameraSpeed * airControl;
        if (input.IsKeyDown(Keys.D)) Acceleration += flatRight * cameraSpeed * airControl;
        if (input.IsKeyDown(Keys.Space) && Grounded) Acceleration += new Vector3(0, 1, 0) * 5 / (float)e.Time;
        // if (input.IsKeyDown(Keys.LeftShift)) velocity -= _camera.Up * cameraSpeed * new Vector3(1, 0, 1);

        //Velocity.X = Velocity.X.Clamp(-3, 3);
        //Velocity.Y = Velocity.Y.Clamp(-30, 30);
        //Velocity.Z = Velocity.Z.Clamp(-3, 3);

        Velocity += Acceleration * (float)e.Time;
        cam.Position += Velocity * (float)e.Time;

        if(Grounded)
        {
            Velocity.X *= .98f;
            Velocity.Z *= .98f;
        }
        else
        {
            Velocity.X *= .99f;
            Velocity.Z *= .99f;
        }

        Console.WriteLine($"X:{Velocity.X:F2} Y:{Velocity.Y:F2} Z:{Velocity.Z:F2}");
    }

    const float cBound = .5f;
    const float playerHeight = 2;
    bool ComputeCollision(Vector3 position, Vector3 direction)
    {
        foreach (var obj in objs)
        {
            if(obj is Brush brush)
            {
                Matrix4 trans = brush.transform.GetMat().ToGLMat4();
                Matrix4 arcTrans = trans.Inverted();

                var localPos = Multiply(arcTrans, position);
                var localVel = Multiply(arcTrans, direction);

                for (float i = 0; i <= 1; i += .01f)
                {
                    var sample = localPos + i * localVel;
                    bool xInBounds = sample.X > -cBound && sample.X < cBound;
                    bool yInBounds = sample.Y - (playerHeight/brush.GetScale.Y) > -cBound && sample.Y - (playerHeight/brush.GetScale.Y) < cBound;
                    bool zInBounds = sample.Z > -cBound && sample.Z < cBound;
                    if (xInBounds && yInBounds && zInBounds) return true;
                }
            }
        }
        return false;
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
}