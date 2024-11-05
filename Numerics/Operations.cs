namespace Sauce_Engine.Numerics;

static class Operations
{
    
    public static float Dot(Position3d lhs, Position3d rhs) => lhs.X * rhs.X + lhs.Y * rhs.Y + lhs.Z * rhs.Z;
    public static Direction3d Cross(Position3d a, Position3d b) => new(a.Y*b.Z - a.Z*b.Y, a.Z*b.X - a.X*b.Z, a.X*b.Y - a.Y*b.X);
}