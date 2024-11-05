using Vector4 = System.Numerics.Vector4;
using SysVec3 = System.Numerics.Vector3;
using GlVec3 = OpenTK.Mathematics.Vector3;

namespace Sauce_Engine.Numerics;

public abstract class Vector
{
    Vector4 vector;
    public float X => vector.X / vector.W;
    public float Y => vector.Y / vector.W;
    public float Z => vector.Z / vector.W;

    public Vector(float x = 0, float y = 0, float z = 0)
    {
        vector = new(x, y, z, 1);
    }

    public SysVec3 ToSysVec3() => new(X, Y, Z);
}