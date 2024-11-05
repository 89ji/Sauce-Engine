using System;
using System.Numerics;
using Sauce_Engine.Enums;
using Sauce_Engine.Util;

using SysVec3 = System.Numerics.Vector3;

namespace Sauce_Engine.Numerics;

public class Transform
{
    public SysVec3 Rotation {private set; get;}
    public SysVec3 Translation {private set; get;}
    public SysVec3 Scale {private set; get;}

    bool isMatCurrent = false;
    public Matrix4x4 Matrix {private set; get;}

    public Transform(SysVec3? rotation, SysVec3? translation, SysVec3? scale)
    {
        if(!rotation.HasValue) rotation = SysVec3.Zero;
        if(!translation.HasValue) translation = SysVec3.Zero;
        if(!scale.HasValue) scale = SysVec3.One;

        Rotation = rotation.Value;
        Translation = translation.Value;
        Scale = scale.Value;
    }

    public Transform()
    {
        Rotation = SysVec3.Zero;
        Translation = SysVec3.Zero;
        Scale = SysVec3.One;
    }

    public Transform(Transform original)
    {
        var oR = original.Rotation;
        var oT = original.Translation;
        var oS = original.Scale;

        Rotation = new SysVec3(oR.X, oR.Y, oR.Z);
        Translation = new SysVec3(oT.X, oT.Y, oT.Z);
        Scale = new(oS.X, oS.Y, oS.Z);
    }

    public void TranslateBy(SysVec3 translation)
    {
        isMatCurrent = false;
        Translation += translation;
    }
    public void RotateBy(SysVec3 rotation)
    {
        isMatCurrent = false;
        Rotation += rotation;
    }
    public void ScaleBy(SysVec3 scale)
    { 
        isMatCurrent = false;
        Scale += scale;
    }
    
    public void TranslateTo(SysVec3 translation)
    {
        isMatCurrent = false;
        Translation = translation;
    }
    public void RotateTo(SysVec3 rotation)
    {
        isMatCurrent = false;
        Rotation = rotation;
    }
    public void ScaleTo(SysVec3 scale)
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