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

    const float SampleDensity = 1 / 20;



    public bool CalculateRaycastInMap(Position3d origin, Direction3d direction)
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

    Position3d? CalculateRaycast(Position3d origin, Direction3d direction, MapObject target)
    {
        Transform modelTransform = new Transform(target.transform);
        modelTransform.RotateTo(new (-modelTransform.Rotation.X, -modelTransform.Rotation.Y, -modelTransform.Rotation.Z));
        Matrix4 trans = modelTransform.GetMat().ToGLMat4();
        Matrix4 arcTrans = trans.Inverted();

        Position3d localPos = arcTrans.TransformPoint(origin);
        Direction3d localDir = arcTrans.TransformPoint(direction);

        for (float t = 0; t <= 1; t += SampleDensity)
        {
            Position3d sample = localPos + t * localDir;
            bool xInBounds = sample.X < -.5f && sample.X > .5f;
            bool yInBounds = sample.Y < -.5f && sample.Y > .5f;
            bool zInBounds = sample.Z < -.5f && sample.Z > .5f;

            if (xInBounds && yInBounds && zInBounds)
            {
                return trans.TransformPoint(sample);
            }
        }
        return null;
    }

    void DrawMarker(GlVec3 Location, float Size = .2f)
    {
        imObjList.AddMapObject(new Brush(new Transform(SysVec3.Zero, Location.ToSysVec3(), new SysVec3(Size, Size, Size))));
    }

    void DrawMarker(Position3d Location, float Size = .2f)
    {
        imObjList.AddMapObject(new Brush(new Transform(SysVec3.Zero, Location.ToSysVec3(), new SysVec3(Size, Size, Size))));
    }
}