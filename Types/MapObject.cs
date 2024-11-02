using System.Collections.Generic;
using System.Numerics;
using Sauce_Engine.Util;
using Matrix4 = OpenTK.Mathematics.Matrix4;

namespace Sauce_Engine.Types;

public abstract class MapObject
{
	public Transform transform;

	public MapObject(Transform transform)
	{
		this.transform = transform;
	}

	public MapObject()
	{
		transform = new();
	}


	public void TranslateBy(Vector3 translation)
	{
		transform.TranslateBy(translation);
	}

	public void RotateBy(Vector3 rotate)
	{
		transform.RotateBy(rotate);
	}

	public virtual void ScaleBy(Vector3 scale)
	{
		transform.ScaleBy(scale);
	}

	public void TranslateTo(Vector3 translation)
	{
		transform.TranslateTo(translation);
	}

	public void RotateTo(Vector3 rotate)
	{
		transform.RotateTo(rotate);
	}

	public virtual void ScaleTo(Vector3 scale)
	{
		transform.ScaleTo(scale);
	}

	public Vector3 GetTranslate => transform.Translation;
	public Vector3 GetRotation => transform.Rotation;
	public Vector3 GetScale => transform.Scale;
	public Transform GetTransform() => transform;
	public Matrix4 MakeGlModelMat()
	{
		Matrix4 model = Matrix4.CreateScale(GetScale.ToGlVec3());
		model *= Matrix4.CreateRotationY(GetRotation.Y);
		model *= Matrix4.CreateRotationX(GetRotation.X);
		model *= Matrix4.CreateRotationZ(GetRotation.Z);
		model *= Matrix4.CreateTranslation(GetTranslate.ToGlVec3());
		return model;
	}
}