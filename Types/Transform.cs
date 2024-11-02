using System;
using System.Numerics;
using Sauce_Engine.Enums;
using Sauce_Engine.Util;
using Vector3 = System.Numerics.Vector3;

namespace Sauce_Engine.Types;

public class Transform
{
    public Vector3 Rotation {private set; get;}
    public Vector3 Translation {private set; get;}
    public Vector3 Scale {private set; get;}

    bool isMatCurrent = false;
    public Matrix4x4 Matrix {private set; get;}

    public Transform(Vector3? rotation, Vector3? translation, Vector3? scale)
    {
        if(!rotation.HasValue) rotation = Vector3.Zero;
        if(!translation.HasValue) translation = Vector3.Zero;
        if(!scale.HasValue) scale = Vector3.One;

        Rotation = rotation.Value;
        Translation = translation.Value;
        Scale = scale.Value;
    }

    public Transform()
    {
        Rotation = Vector3.Zero;
        Translation = Vector3.Zero;
        Scale = Vector3.One;
    }

    public Transform(Transform original)
    {
        var oR = original.Rotation;
        var oT = original.Translation;
        var oS = original.Scale;

        Rotation = new Vector3(oR.X, oR.Y, oR.Z);
        Translation = new Vector3(oT.X, oT.Y, oT.Z);
        Scale = new(oS.X, oS.Y, oS.Z);
    }

    public void TranslateBy(Vector3 translation)
    {
        isMatCurrent = false;
        Translation += translation;
    }
    public void RotateBy(Vector3 rotation)
    {
        isMatCurrent = false;
        Rotation += rotation;
    }
    public void ScaleBy(Vector3 scale)
    { 
        isMatCurrent = false;
        Scale += scale;
    }
    
    public void TranslateTo(Vector3 translation)
    {
        isMatCurrent = false;
        Translation = translation;
    }
    public void RotateTo(Vector3 rotation)
    {
        isMatCurrent = false;
        Rotation = rotation;
    }
    public void ScaleTo(Vector3 scale)
    { 
        isMatCurrent = false;
        Scale = scale;
    }

    public Matrix4x4 GetMat()
    {
        if(isMatCurrent) return Matrix;
        isMatCurrent = true;

        Matrix4x4 composedRotation =
        ConstructRotationMatrix(Rotation.Y, Dimension.Y) *
        ConstructRotationMatrix(Rotation.X, Dimension.X) *
        ConstructRotationMatrix(Rotation.Z, Dimension.Z);

        Matrix4x4 composedScale = new (
            Scale.X, 0, 0, 0,
            0, Scale.Y, 0, 0,
            0, 0, Scale.Z, 0,
            0, 0, 0, 1
        );

        Matrix4x4 composedTranslate = new (
            1, 0, 0, Translation.X,
            0, 1, 0, Translation.Y,
            0, 0, 1, Translation.Z,
            0, 0, 0, 1
        );

        Matrix = composedTranslate * composedRotation * composedScale;
        return Matrix;
    }

    private Matrix4x4 ConstructRotationMatrix(float angle, Dimension axis)
    {
        float cos = MathF.Cos(angle);
        float sin = MathF.Sin(angle);

        return axis switch
        {
            Dimension.X => new Matrix4x4(
                1, 0, 0, 0,
                0, cos, -sin, 0,
                0, sin, cos, 0,
                0, 0, 0, 1
            ),
            Dimension.Y => new Matrix4x4(
                cos, 0, -sin, 0,
                0, 1, 0, 0,
                sin, 0, cos, 0,
                0, 0, 0, 1
            ),
            Dimension.Z => new Matrix4x4(
                cos, -sin, 0, 0,
                sin, cos, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1
            ),
            _ => throw new NotImplementedException()
        };
    }
}