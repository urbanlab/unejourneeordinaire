using UnityEngine;
using System.Collections;

public class CTextureOffset : MonoBehaviour 
{
	[SerializeField] private Vector2 _v2OffsetSpeed;
	private Vector2 v2TextureOffset = Vector2.zero;

	private Material _mMainMaterial;

	///-----------------------------------------------------------------------------------
	/// 
	///-----------------------------------------------------------------------------------
	void Start ()
	{
		_v2OffsetSpeed *= 0.005F;
		_mMainMaterial = transform.GetComponent<Renderer> ().sharedMaterial;	
	}

	///-----------------------------------------------------------------------------------
	/// 
	///-----------------------------------------------------------------------------------
	void Update ()
	{
		if (_mMainMaterial != null) 
		{			
			v2TextureOffset += _v2OffsetSpeed * Time.deltaTime;
			if( _mMainMaterial.HasProperty("_MainTex" ))
				_mMainMaterial.SetTextureOffset( "_MainTex", v2TextureOffset );
			if( _mMainMaterial.HasProperty( "_Normals"))
				_mMainMaterial.SetTextureOffset( "_Normals", v2TextureOffset );
			if( _mMainMaterial.HasProperty("_Lights" ))
				_mMainMaterial.SetTextureOffset( "_Lights", v2TextureOffset );
		}
	}
}
