using OpenTK.Mathematics;
using Sauce_Engine.Numerics;
using Sauce_Engine.Types;
using Sauce_Engine.Util;

using SysVec3 = System.Numerics.Vector3;
using GlVec3 = OpenTK.Mathematics.Vector3;

class CollisionManager
{
    MapObjList mapObjects;
    MapObjList imObjList;
    public CollisionManager(MapObjList mapObjects, MapObjList imObjects)
    {
        this.mapObjects = mapObjects;
        this.imObjList = imObjects;
    }

    const float SampleDensity = 1f / 20f;
    const float cLowBound = -.5f;
    const float cHighBound = .5f;
    const float cPadding = .5f;
    const float playerHeight = 2;


    Triangle[] cubeTriangles = new Triangle[]
{
    // Front Face (z = 0.5)
    new Triangle(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f)),
    new Triangle(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(-0.5f, 0.5f, 0.5f)),

    // Back Face (z = -0.5)
    new Triangle(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0.5f, 0.5f, -0.5f)),
    new Triangle(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0.5f, -0.5f, -0.5f)),

    // Left Face (x = -0.5)
    new Triangle(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-0.5f, 0.5f, 0.5f)),
    new Triangle(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(-0.5f, 0.5f, -0.5f)),

    // Right Face (x = 0.5)
    new Triangle(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.5f, -0.5f, 0.5f)),
    new Triangle(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f)),

    // Top Face (y = 0.5)
    new Triangle(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f)),
    new Triangle(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.5f, -0.5f)),

    // Bottom Face (y = -0.5)
    new Triangle(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, 0.5f)),
    new Triangle(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, 0.5f), new Vector3(-0.5f, -0.5f, 0.5f))
};


    public bool CalculateRaycastInMap(Vector3 origin, Vector3 direction)
    {
        //DrawRay(origin, direction);
        bool found = false;
        foreach (MapObject obj in mapObjects)
        {
            if (obj is Brush b)
            {
                var result = CalculateRaycast(origin, direction, b);
                if (result != null) 
                {
                    DrawMarker(result.Value);
                    found = true;
                }
            }
        }
        return found;
    }

    public bool CalculateRaycastInMapNormal(Vector3 origin, Vector3 direction, out Vector3 normal)
    {
        normal = Vector3.Zero;
        foreach (MapObject obj in mapObjects)
        {
            if (obj is Brush b)
            {
                var result = CalculateRaycastNormal(origin, direction, b);
                if (result != null) normal = result.Value;
            }
        }
        return normal != Vector3.Zero;
    }

    // Crude implementation of the moller trombone algorithm
    Vector3? CalculateRaycast(Vector3 origin, Vector3 direction, MapObject target)
    {
        Matrix4 trans = target.MakeRealModelMat();
        Vector3? found = null;
        foreach (Triangle tri in cubeTriangles)
        {
            Vector3 P0 = trans.TransformDir(tri.Vertex1) + target.GetTranslate;
            Vector3 P1 = trans.TransformDir(tri.Vertex2) + target.GetTranslate;
            Vector3 P2 = trans.TransformDir(tri.Vertex3) + target.GetTranslate;

            //DrawMarker(P0);
            //DrawMarker(P1);
            //DrawMarker(P2);

            Vector3 E1 = P1 - P0;
            Vector3 E2 = P2 - P0;

            Vector3 S = origin - P0;
            Vector3 S1 = Vector3.Cross(direction, E2);
            Vector3 S2 = Vector3.Cross(S, E1);

            float invDet = 1 / Vector3.Dot(S1, E1);
            Vector3 rVec = new(Vector3.Dot(S2, E2), Vector3.Dot(S1, S), Vector3.Dot(S2, direction));

            Vector3 result = invDet * rVec;
            float t = result.X;
            float b1 = result.Y;
            float b2 = result.Z;

            if (t >= 0 &&
                b1 >= 0 && b1 <= 1 &&
                b2 >= 0 && b2 <= 1 &&
                b1 + b2 <= 1 &&
                t < direction.Length)
            {
                //DrawMarker(origin + t * direction);
                found = origin + t * direction;
            }
        }
        return found;
    }

    // Same moller trombone algorithm but returns the normal of the triangle instead
    Vector3? CalculateRaycastNormal(Vector3 origin, Vector3 direction, MapObject target)
    {
        Matrix4 trans = target.MakeRealModelMat();
        Vector3? found = null;
        foreach (Triangle tri in cubeTriangles)
        {
            Vector3 P0 = trans.TransformDir(tri.Vertex1) + target.GetTranslate;
            Vector3 P1 = trans.TransformDir(tri.Vertex2) + target.GetTranslate;
            Vector3 P2 = trans.TransformDir(tri.Vertex3) + target.GetTranslate;

            //DrawMarker(P0);
            //DrawMarker(P1);
            //DrawMarker(P2);

            Vector3 E1 = P1 - P0;
            Vector3 E2 = P2 - P0;

            Vector3 S = origin - P0;
            Vector3 S1 = Vector3.Cross(direction, E2);
            Vector3 S2 = Vector3.Cross(S, E1);

            float invDet = 1 / Vector3.Dot(S1, E1);
            Vector3 rVec = new(Vector3.Dot(S2, E2), Vector3.Dot(S1, S), Vector3.Dot(S2, direction));

            Vector3 result = invDet * rVec;
            float t = result.X;
            float b1 = result.Y;
            float b2 = result.Z;

            if (t >= 0 &&
                b1 >= 0 && b1 <= 1 &&
                b2 >= 0 && b2 <= 1 &&
                b1 + b2 <= 1 &&
                t < direction.Length)
            {
                found = Vector3.Cross(P0-P1, P1-P2).Normalized();
                DrawRay(origin + t * direction, found.Value);
            }
        }
        return found;
    }

    public void DrawMarker(Vector3 Location, float Size = .2f)
    {
        imObjList.AddMapObject(new Brush(new Transform(Vector3.Zero, Location, new(Size, Size, Size))));
    }

    public void DrawRay(Vector3 origin, Vector3 direction)
    {
        for (float t=0; t <= 1; t+=.05f)
        {
            DrawMarker(origin + t * direction);
        }
    }
}