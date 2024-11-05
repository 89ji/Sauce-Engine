using System;
using System.IO;
using Sauce_Engine.Types;
using Sauce_Engine.Numerics;

using Matrix4x4 = System.Numerics.Matrix4x4;
using SysVec3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;
using GlVec3 = OpenTK.Mathematics.Vector3;
using Matrix4 = OpenTK.Mathematics.Matrix4;

namespace Sauce_Engine.Util;

public static class Extensions
{
	public static float toDeg(this float radians) => radians * 180f / MathF.PI;
	public static float toRad(this float degrees) => degrees / 180f * MathF.PI;
	public static float ToFloat(this string num) => float.Parse(num);

	[Obsolete("Use MapObject's MakeGlModelMat() instead.")]
	public static Matrix4 ToGLMat4(this Matrix4x4 mat)
	{
		var ret = new Matrix4();
		ret.M11 = mat.M11;
		ret.M12 = mat.M12;
		ret.M13 = mat.M13;
		ret.M14 = mat.M14;

		ret.M21 = mat.M21;
		ret.M22 = mat.M22;
		ret.M23 = mat.M23;
		ret.M24 = mat.M24;

		ret.M31 = mat.M31;
		ret.M32 = mat.M32;
		ret.M33 = mat.M33;
		ret.M34 = mat.M34;

		ret.M41 = mat.M41;
		ret.M42 = mat.M42;
		ret.M43 = mat.M43;
		ret.M44 = mat.M44;

		return ret;
	}

	public static GlVec3 ToGlVec3(this SysVec3 vec) => new(vec.X, vec.Y, vec.Z);
	public static SysVec3 ToSysVec3(this GlVec3 vec) => new(vec.X, vec.Y, vec.Z);
	public static float Clamp(this float val, float floor, float ceil)
	{
		if (val < floor) return floor;
		if (val > ceil) return ceil;
		return val;
	}

	public static Position3d ToSauceCoord3d(this OpenTK.Mathematics.Vector3 vec) => new(vec.X, vec.Y, vec.Z);
	public static Direction3d ToSauceDir3d(this GlVec3 vec) => new(vec.X, vec.Y, vec.Z);
	public static Direction3d TransformPoint(this Matrix4 m, Direction3d rhs)
    {
        Vector4 res = new();
        res.X = m.M11 * rhs.X + m.M12 * rhs.Y + m.M13 * rhs.Z + m.M14;
        res.Y = m.M21 * rhs.X + m.M22 * rhs.Y + m.M23 * rhs.Z + m.M24;
        res.Z = m.M31 * rhs.X + m.M32 * rhs.Y + m.M33 * rhs.Z + m.M34;
        res.W = m.M41 * rhs.X + m.M42 * rhs.Y + m.M43 * rhs.Z + m.M44;
        return new(res.X / res.W, res.Y / res.W, res.Z / res.W);
    }

    public static Position3d TransformPoint(this Matrix4 m, Position3d rhs)
    {
        Vector4 res = new();
        res.X = m.M11 * rhs.X + m.M12 * rhs.Y + m.M13 * rhs.Z + m.M14;
        res.Y = m.M21 * rhs.X + m.M22 * rhs.Y + m.M23 * rhs.Z + m.M24;
        res.Z = m.M31 * rhs.X + m.M32 * rhs.Y + m.M33 * rhs.Z + m.M34;
        res.W = m.M41 * rhs.X + m.M42 * rhs.Y + m.M43 * rhs.Z + m.M44;
        return new(res.X / res.W, res.Y / res.W, res.Z / res.W);
    }
}