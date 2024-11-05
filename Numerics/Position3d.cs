using System;
using System.Numerics;

namespace Sauce_Engine.Numerics;

public class Position3d : Vector
{
    public Position3d(float x = 0, float y = 0, float z = 0)
	{
	}

    public float Length()
    {
        return MathF.Sqrt(X * X + Y * Y + Z * Z);
    }
    public Position3d Normalize()
    {
        float magnitude = Length();
        return new(X/magnitude, Y/magnitude, Z/magnitude);
    }
    
    public static Position3d operator*(Transform lhs, Position3d rhs)
    {
        Vector4 res = new();
        Matrix4x4 m = lhs.GetMat();
        res.X = m.M11 * rhs.X + m.M12 * rhs.Y + m.M13 * rhs.Z + m.M14;
        res.Y = m.M21 * rhs.X + m.M22 * rhs.Y + m.M23 * rhs.Z + m.M24;
        res.Z = m.M31 * rhs.X + m.M32 * rhs.Y + m.M33 * rhs.Z + m.M34;
        res.W = m.M41 * rhs.X + m.M42 * rhs.Y + m.M43 * rhs.Z + m.M44;
        return new Position3d(res.X/res.W, res.Y/res.W, res.Z/res.W);
    }
    public static Direction3d operator-(Position3d lhs, Position3d rhs) => new (lhs.X-rhs.X, lhs.Y-rhs.Y, lhs.Z-rhs.Z);

    public static Position3d operator-(Position3d lhs, Direction3d rhs) => new (lhs.X-rhs.X, lhs.Y-rhs.Y, lhs.Z-rhs.Z);
    public static Position3d operator+(Position3d lhs, Direction3d rhs) => new (lhs.X+rhs.X, lhs.Y+rhs.Y, lhs.Z+rhs.Z);

    public Direction3d Cross(Position3d b)
    {
        return new(Y * b.Z - Z * b.Y, Z * b.X - X * b.Z, X * b.Y - Y * b.X);
    }

    public Vector3 ToSysVec3() => new (X, Y, Z);
}