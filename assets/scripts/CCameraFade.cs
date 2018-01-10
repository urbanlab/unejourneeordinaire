using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCameraFade : MonoBehaviour 
{
	private Material fadeMaterial;

	public void SetMaterial(ref Material mat)
	{
		fadeMaterial = mat;
	}

	void OnPostRender()
	{
		if (fadeMaterial == null)
			return;
		fadeMaterial.SetPass(0);
		GL.PushMatrix();
		GL.LoadOrtho();
		GL.Color(fadeMaterial.color);
		GL.Begin(GL.QUADS);
		GL.Vertex3(0f, 0f, -12f);
		GL.Vertex3(0f, 1f, -12f);
		GL.Vertex3(1f, 1f, -12f);
		GL.Vertex3(1f, 0f, -12f);
		GL.End();
		GL.PopMatrix();
	}
}
