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


    public bool CalculateRaycastInMap(Vector3 origin, Vector3 direction)
    {
        foreach (MapObject obj in mapObjects)
        {
            if (obj is Brush b)
            {
                var result = CalculateRaycast(origin, direction, b);
                if (result != null) return true;
            }
        }
        return false;
    }

    Vector3? CalculateRaycast(Vector3 origin, Vector3 direction, MapObject target)
    {
        Transform modelTransform = new Transform(target.transform);
        modelTransform.RotateTo(new (-modelTransform.Rotation.X, -modelTransform.Rotation.Y, -modelTransform.Rotation.Z));
        Matrix4 trans = modelTransform.GetMat();
        Matrix4 arcTrans = trans.Inverted();

        Vector3 localPos = arcTrans.TransformPoint(origin);
        Vector3 localDir = arcTrans.TransformDir(direction);
        localDir.Normalize();

        for (float t = 0; t <= 1; t += SampleDensity)
        {
            Vector3 sample = localPos + t * localDir;
            bool xInBounds = sample.X < -.5f && sample.X > .5f;
            bool yInBounds = sample.Y < -.5f && sample.Y > .5f;
            bool zInBounds = sample.Z < -.5f && sample.Z > .5f;

            var pt = trans.TransformPoint(sample);
            DrawMarker(pt, .01f);

            if (xInBounds && yInBounds && zInBounds)
            {
                return pt;
            }
        }
        return null;
    }

    void DrawMarker(Vector3 Location, float Size = .2f)
    {
        imObjList.AddMapObject(new Brush(new Transform(Vector3.Zero, Location, new (Size, Size, Size))));
    }
}