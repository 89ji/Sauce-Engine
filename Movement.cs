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
    const float slideMeasureGap = .1f;



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
        bool Grounded = ComputeCollision(cam.Position, new Vector3(0, -1, 0));

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
        bool TopBlocked = ComputeCollision(cam.Position + new Vector3(0f, 3f, 0f), new(0, 1f, 0));

        // Check inputs and move and stuff
        Vector3 moveDirection = new();

        if (input.IsKeyDown(Keys.W)) moveDirection += flatFront;
        if (input.IsKeyDown(Keys.S)) moveDirection -= flatFront;
        if (input.IsKeyDown(Keys.A)) moveDirection -= flatRight;
        if (input.IsKeyDown(Keys.D)) moveDirection += flatRight;

        if (input.IsKeyDown(Keys.Space) && Grounded) Acceleration += new Vector3(0, 1, 0) * 10f / (float)e.Time;
        if (input.IsKeyDown(Keys.LeftShift)) Acceleration -= new Vector3(0, 1, 0) * 5 / (float)e.Time;

        if (moveDirection.Length > 0)
        {
            moveDirection.Normalize();

            Matrix4 rotate90 = Matrix4.CreateFromAxisAngle(Vector3.UnitY, 90f.toDeg());
            Vector3 rightMovement = Multiply(rotate90, moveDirection);

            float frontBlock = ComputeCollisionDistance(cam.Position + new Vector3(0, .25f, 0) + .25f * moveDirection, moveDirection * 2);
            float rightExtent = ComputeCollisionDistance(cam.Position + new Vector3(0, .25f, 0) + rightMovement * slideMeasureGap - moveDirection, moveDirection * 5, true);
            float leftExtent = ComputeCollisionDistance(cam.Position + new Vector3(0, .25f, 0) - rightMovement * slideMeasureGap - moveDirection, moveDirection * 5, true);
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

    const float cLowBound = -.5f;
    const float cHighBound = .5f;
    const float cPadding = .5f;
    const float playerHeight = 2;
    bool ComputeCollision(Vector3 position, Vector3 direction, bool Debug = false)
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

                var localPos = Multiply(arcTrans, position);
                var localVel = Multiply(arcTrans, direction);

                // For drawing the extents of each collidable brush
                /*
                for(float i =- .5f; i <= .5; i += .25f)
                for(float j =- .5f; j <= .5; j += .25f)
                for(float k =- .5f; k <= .5; k += .25f)
                {
                    if (i != 0 && j != 0 && k != 0) continue;
                    Vector3 point = new(i, j, k);
                    point = Multiply(trans, point);
                    DrawMarker(point);
                }*/

                for (float i = 0; i <= 1; i += .05f)
                {
                    var sample = localPos + i * localVel;
                    bool xInBounds = sample.X > cLowBound - (cPadding / brush.GetScale.X) && sample.X < cHighBound + (cPadding / brush.GetScale.X);
                    bool yInBounds = sample.Y - (playerHeight / brush.GetScale.Y) > cLowBound && sample.Y - (playerHeight / brush.GetScale.Y) < cHighBound;
                    bool zInBounds = sample.Z > cLowBound - (cPadding / brush.GetScale.Z) && sample.Z < cHighBound + (cPadding / brush.GetScale.Z);

                    if (Debug)
                    {
                        sample = Multiply(trans, sample);
                        //sample.Y -= 2;
                        DrawMarker(sample);
                    }
                    if (xInBounds && yInBounds && zInBounds)
                    {
                        if (Debug)
                        {
                            sample = Multiply(trans, sample);
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

    // Similar to the other function but returns +inf if no collision and the distance if found
    float ComputeCollisionDistance(Vector3 position, Vector3 direction, bool Debug = false)
    {
        float CollisionFound = float.PositiveInfinity;
        foreach (var obj in objs)
        {
            if (obj is Brush brush)
            {
                Transform transf = new Transform(brush.transform);
                transf.RotateTo(new(-transf.Rotation.X, -transf.Rotation.Y, -transf.Rotation.Z));
                Matrix4 trans = transf.GetMat().ToGLMat4();
                Matrix4 arcTrans = trans.Inverted();

                var localPos = Multiply(arcTrans, position);
                var localVel = Multiply(arcTrans, direction);

                for (float i = 0; i <= 1; i += .05f)
                {
                    var sample = localPos + i * localVel;
                    bool xInBounds = sample.X > cLowBound - (cPadding / brush.GetScale.X) && sample.X < cHighBound + (cPadding / brush.GetScale.X);
                    bool yInBounds = sample.Y - (playerHeight / brush.GetScale.Y) > cLowBound && sample.Y - (playerHeight / brush.GetScale.Y) < cHighBound;
                    bool zInBounds = sample.Z > cLowBound - (cPadding / brush.GetScale.Z) && sample.Z < cHighBound + (cPadding / brush.GetScale.Z);

                    /*if (Debug)
                    {
                        sample = Multiply(trans, sample);
                        //sample.Y -= 2;
                        DrawMarker(sample);
                    }*/
                    if (xInBounds && yInBounds && zInBounds) 
                    {
                        DrawMarker(Multiply(trans, sample));
                        CollisionFound = MathF.Min(CollisionFound, Vector3.Distance(position, sample));
                    }
                }
            }
        }
        if (Debug) DrawMarker(position + direction);
        return CollisionFound;
    }

    Vector3 Multiply(Matrix4 m, Vector3 rhs)
    {
        Vector4 res = new();
        res.X = m.M11 * rhs.X + m.M12 * rhs.Y + m.M13 * rhs.Z + m.M14;
        res.Y = m.M21 * rhs.X + m.M22 * rhs.Y + m.M23 * rhs.Z + m.M24;
        res.Z = m.M31 * rhs.X + m.M32 * rhs.Y + m.M33 * rhs.Z + m.M34;
        res.W = m.M41 * rhs.X + m.M42 * rhs.Y + m.M43 * rhs.Z + m.M44;
        return new(res.X / res.W, res.Y / res.W, res.Z / res.W);
    }

    void DrawMarker(Vector3 Location, float Size = .2f)
    {
        imObjList.AddMapObject(new Brush(new Transform(Vec3.Zero, Location.ToSysVec3(), new Vec3(Size, Size, Size))));
    }
}