using OpenTK.Mathematics;

namespace Sauce_Engine.Numerics;

public struct Triangle
{
    public Vector3 Vertex1;
    public Vector3 Vertex2;
    public Vector3 Vertex3;

    public Triangle(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
    {
        Vertex1 = vertex1;
        Vertex2 = vertex2;
        Vertex3 = vertex3;
    }
}