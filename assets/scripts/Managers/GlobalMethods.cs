using UnityEngine;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.Generic;

public static class GlobalMethods 
{
	public static Vector2 v2DefaultHugeThumbSize = new Vector2 (648, 432);
	public static Vector2 v2DefaultBigThumbSize = new Vector2 (432, 288);
	public static Vector2 v2DefaultMediumThumbSize = new Vector2 (324, 216);
	public static Vector2 v2DefaultSmallThumbSize = new Vector2 (216, 144);
	public static Vector2 v2DefaultMiniThumbSize = new Vector2 (80,52);
	public static Vector2 v2DefaultBigAdvertisementThumbSize = new Vector2 (266, 259);
	public static Vector2 v2DefaultSmallAdvertisementThumbSize = new Vector2 (266, 144);

	public static void ResizeToDefault( ref Texture2D a_rTexture2D, TEXTURE_RESOLUTION a_resolution )
	{
		switch (a_resolution) 
		{
			case TEXTURE_RESOLUTION.ADVERTISEMENT_BIG:
				 ResizeImage (ref a_rTexture2D, v2DefaultBigAdvertisementThumbSize, 0.75F);
				break;
			case TEXTURE_RESOLUTION.ADVERTISEMENT_SMALL:
				ResizeImage (ref a_rTexture2D, v2DefaultSmallAdvertisementThumbSize, 0.75F);
				break;
			case TEXTURE_RESOLUTION.ZONE_HUGE:
				ResizeImage (ref a_rTexture2D, v2DefaultHugeThumbSize, 0.75F);
				break;
			case TEXTURE_RESOLUTION.ZONE_BIG:
				ResizeImage (ref a_rTexture2D, v2DefaultBigThumbSize, 0.75F);
				break;
			case TEXTURE_RESOLUTION.ZONE_MEDIUM:
				ResizeImage (ref a_rTexture2D, v2DefaultMediumThumbSize, 0.75F);
				break;
			case TEXTURE_RESOLUTION.OBJECT:
			case TEXTURE_RESOLUTION.ZONE_SMALL:
				ResizeImage (ref a_rTexture2D, v2DefaultSmallThumbSize, 0.75F);
				break;
			case TEXTURE_RESOLUTION.THUMBNAIL:
				ResizeImage (ref a_rTexture2D, v2DefaultMiniThumbSize, 0.75F);
				break;
			case TEXTURE_RESOLUTION.PHOTO_ASSET:
				if (a_rTexture2D.height > 750)
					ResizeImage (ref a_rTexture2D, 750);
				break;
			case TEXTURE_RESOLUTION.PHOTO_ASSET_360:
				if (a_rTexture2D.height > 2048)
					ResizeImage (ref a_rTexture2D, 2048);
				break;
			default:
				break;
		}
	}

    public static bool EndsWith2(this string a_sRef, string a_sExtension)
    {
        int iExtLen = a_sExtension.Length;
        int iLen = a_sRef.Length;
        return iLen < iExtLen ? false : a_sRef.Substring(a_sRef.Length - iExtLen) == a_sExtension;
    }

	///-----------------------------------------------------------------------------------
	/// 															  
	///-----------------------------------------------------------------------------------	
	public static void ResizeImage( ref Texture2D a_rTexture2D, Vector2 a_v2Resolution, float a_fFactor = 1.0F )
	{
		if( a_rTexture2D.width >= a_fFactor*a_v2Resolution.x && a_rTexture2D.height >= a_fFactor*a_v2Resolution.y )
		TextureScale.Bilinear( ref a_rTexture2D, (int)(a_v2Resolution.x*a_fFactor), (int)(a_v2Resolution.y*a_fFactor) );
	}
	public static void ResizeImage( ref Texture2D a_rTexture2D, float a_fMaxHeight )
	{
		if( a_rTexture2D.height > a_fMaxHeight )
		TextureScale.Bilinear( ref a_rTexture2D, (int)(a_fMaxHeight * a_rTexture2D.width * Mathf.Pow(a_rTexture2D.height, -1)), (int)(a_fMaxHeight) );
	}

	public static void CenterTexture( ref MeshRenderer a_rMeshRenderer)
	{
		Texture texture = a_rMeshRenderer.material.mainTexture;
		if (texture == null)
			return;

		Vector2 v2Tiling = Vector2.one;
		Vector2 v2Offset = Vector2.zero;

		if (texture.width >= texture.height) 
		{
			v2Tiling.y = texture.width * Mathf.Pow (1.5F * texture.height, -1);
			v2Offset.y = 0.5F*(1-v2Tiling.y);
		} 
		else 
		{
			v2Tiling.x = texture.width * Mathf.Pow (1.5F * texture.height, -1);
			v2Offset.x = 0.5F*(1-v2Tiling.x);
		}
		a_rMeshRenderer.material.SetTextureScale ("_MainTex", v2Tiling);
		a_rMeshRenderer.material.SetTextureOffset ("_MainTex", v2Offset);
	}
	
	///-----------------------------------------------------------------------------------
	/// 															  
	///-----------------------------------------------------------------------------------	
	public static string GetCaller(int level = 2)
	{
		var m = new StackTrace().GetFrame(level).GetMethod();
		
		// .Name is the name only, .FullName includes the namespace
		var className = m.DeclaringType.FullName;
		
		//the method/function name you are looking for.
		var methodName = m.Name;
		
		//returns a composite of the namespace, class and method name.
		return className + "->" + methodName;
	}

	public static void EnableChildrenColliderAndRenderer(GameObject a_oGameObject, bool a_bEnabled, ref bool a_bIsAlreadyEnabled, params GameObject[] exceptions)
	{
		if((a_bEnabled && a_bIsAlreadyEnabled) || (!a_bEnabled && !a_bIsAlreadyEnabled))
			return;
			
		Renderer renderer = a_oGameObject.GetComponent<Renderer>();
		Collider collider = a_oGameObject.GetComponent<Collider>();
		Transform transform = a_oGameObject.transform;
		int childCount = transform.childCount;

		a_bIsAlreadyEnabled = a_bEnabled;
		if(renderer != null)
			renderer.enabled = a_bEnabled;
			
		if(collider != null)
			collider.enabled = a_bEnabled;
		
		for(int i = 0; i < childCount; i++)
		{
			if(!IsGameObjectIn(transform.GetChild(i).gameObject, exceptions))
			{
				transform.GetChild(i).gameObject.SetActive(a_bEnabled);
			}
		}		
	}
	
	private static bool IsGameObjectIn(GameObject go, GameObject[] gos)
	{
		foreach(GameObject g in gos)
		{
			if(g.name == go.name)
			{
				return true;
			}
		}
		return false;
	}

	public static string Truncate( string a_strValue, int a_iMaxLength)
	{
		if (string.IsNullOrEmpty (a_strValue))
			return a_strValue;
		return (a_strValue.Length <= a_iMaxLength) ? a_strValue : a_strValue.Substring( 0, a_iMaxLength );
	}
	
	public static float GetTextMeshWidth(TextMesh a_TextMesh)
	{
		MeshRenderer mr = a_TextMesh.GetComponent<MeshRenderer>();
		Vector3 extents = mr.bounds.extents;

		Vector2 v2Horizontal = new Vector2 (extents.x, extents.z);
		return v2Horizontal.magnitude;
	}
		
	public static void TextMeshTruncate(TextMesh a_textMesh, float a_fWidth)
	{
		if(GetTextMeshWidth(a_textMesh) > a_fWidth)
		{
			a_textMesh.text = a_textMesh.text + "...";
			while(GetTextMeshWidth(a_textMesh) > a_fWidth)
			{
				a_textMesh.text = a_textMesh.text.Remove(a_textMesh.text.Length - 4, 1);
			}
		}
    }

	public static bool TextMeshWordWrap( TextMesh a_textMesh, float a_fWidth, int a_iHeight )
	{
		string strBuilder = "";
		int iHeight = 0;

		bool bIsTruncated = false;

		string strText = a_textMesh.text;
		a_textMesh.text = "";
		string[] strWords = strText.Split (' ');
		for( int iCount = 0; iCount < strWords.Length; iCount ++ )
		{
			a_textMesh.text += strWords[iCount] + " ";

			if( GetTextMeshWidth (a_textMesh) > a_fWidth )
			{
				iHeight++;
				if (iHeight < a_iHeight) 
				{
					a_textMesh.text = strBuilder.TrimEnd () + "\r\n" + strWords [iCount] + " ";
				} 
				else 
				{
					TextMeshTruncate( a_textMesh, a_fWidth);
					bIsTruncated = true;
					break;
				}
			}
			strBuilder = a_textMesh.text;
		}
		return bIsTruncated;
	}
		
	public static string FormatNumber( int a_iValue )
	{
		int iResult = 0;
		if (a_iValue >= 1000000000) 
		{
			iResult = (int)(0.000000001F * a_iValue);
			return iResult.ToString () + "B";
		} else 
			if (a_iValue >= 1000000) 
			{
				iResult = (int)(0.000001F * a_iValue);
				return iResult.ToString () + "M";
			} else if (a_iValue > 1000) {
				iResult = (int)(0.001F * a_iValue);
				return iResult.ToString () + "K";
			} 
			else 
			{
				return a_iValue.ToString ();
			}
	}

	public static Texture2D LoadTexture( string a_strName )
	{
		return Resources.Load ("Textures/" + a_strName) as Texture2D;
	}
	
	public static string FindSubstringAfterLastSlash( ref string a_strPath )
	{
		if ( a_strPath != null )
		{
			char[] charSlashSeparator = new char[] { '/' };
			string[] aStrings = a_strPath.Split( charSlashSeparator, System.StringSplitOptions.None );			
			int iNumberOfStrings = aStrings.Length;
			
			string result = aStrings[ iNumberOfStrings - 1];
			return result;
		}
		return null;
	}
	
	public static string FindSubstringBeforeLastSlash( ref string a_strPath )
	{
		if ( a_strPath != null )
		{
			char[] charSlashSeparator = new char[] { '/' };
			string[] aStrings = a_strPath.Split( charSlashSeparator, System.StringSplitOptions.None );
			int iNumberOfStrings = aStrings.Length;
			
			string result = aStrings[ iNumberOfStrings - 2];
			return result;
		}
		string strNone = "";
		return strNone;		
	}
	
	public static string FindSubstringBeforeLastTwoSlashes( ref string a_strPath )
	{
		if ( a_strPath != null )
		{
			char[] charSlashSeparator = new char[] { '/' };
			string[] aStrings = a_strPath.Split( charSlashSeparator, System.StringSplitOptions.None );
			int iNumberOfStrings = aStrings.Length;
			
			string result = aStrings[ iNumberOfStrings - 3];
			return result;
		}
		string strNone = "";
		return strNone;	
	}
	
	public static string RemoveSubstringAfterLastSlash( ref string a_strPath )
	{
		if ( a_strPath != null && a_strPath != "" )
		{
			char[] charSlashSeparator = new char[] { '/' };
			string[] aStrings = a_strPath.Split( charSlashSeparator, System.StringSplitOptions.None );			
			int iNumberOfStrings = aStrings.Length;
			
			string strLastSubstring = aStrings[ iNumberOfStrings - 1];

			ReplaceOnString( ref a_strPath, strLastSubstring, "");
		}
		return null;
	}

	///-----------------------------------------------------------------------------------
	/// <summary> removes the last 4 characters of a string </summary>
	/// <param name="a_strPath"> the given string (path) </param>
    ///-----------------------------------------------------------------------------------
	public static void RemoveExtension( ref string a_strPath )
	{
		
		if ( a_strPath == null )
		{
			return; 
		}
			
		a_strPath = Path.GetFileNameWithoutExtension (a_strPath);
	}
	
	///-----------------------------------------------------------------------------------
	/// <summary> replace a certain section of a string with other </summary>
	/// <param name="a_str"> the given string </param>
	/// <param name="a_strToReplace"> the fragment to replace </param>
	/// <param name="a_strNew"> the new fragent </param>
    ///-----------------------------------------------------------------------------------
	public static void ReplaceOnString( ref string a_str, string a_strToReplace, string a_strNew )
	{
		if( a_str != null && a_strToReplace != "" && a_str.Length > a_strToReplace.Length)
		{	
			a_str = a_str.Replace( @a_strToReplace, a_strNew);
		}
	}


	public static void RemoveInvalidCharacters( ref string a_str )
	{
		if (a_str != null && a_str != "") 
		{
			ReplaceOnString( ref a_str, "\\\"", "&#34;" );
		}
	}
	
	///-----------------------------------------------------------------------------------
	/// <summary> fix special characters in a string </summary>
	/// <param name="a_strPath"> the given string </param>
    ///-----------------------------------------------------------------------------------
	public static void FixCharacters( ref string a_str, bool a_bSimple )
	{
		if( a_str != null && a_str != "" )
		{	
			ReplaceOnString( ref a_str, "\\u00a0", " " );
			ReplaceOnString( ref a_str, "\\u00ab", a_bSimple ? "<<" : "«" );
			ReplaceOnString( ref a_str, "\\u00bb", a_bSimple ? ">>" : "»" );
			ReplaceOnString( ref a_str, "\\u2019", "'" );
			ReplaceOnString( ref a_str, "\\u00b0", "°" );
			ReplaceOnString( ref a_str, "\\u00d1", a_bSimple ? "n" : "ñ" );
			ReplaceOnString( ref a_str, "\\u00f1", a_bSimple ? "N" : "Ñ" );
			ReplaceOnString( ref a_str, "\\u0080", a_bSimple ? "EURO" : "€" );
			ReplaceOnString( ref a_str, "\\u20A3", a_bSimple ? "FRANC" : "₣" );
			ReplaceOnString( ref a_str, "\\u0152", a_bSimple ? "CE" : "Œ" );
			ReplaceOnString( ref a_str, "\\u0153", a_bSimple ? "ce" : "œ" );			
			ReplaceOnString( ref a_str, "\\u00c6", a_bSimple ? "AE" : "Æ" );
			ReplaceOnString( ref a_str, "\\u00e6", a_bSimple ? "ae" : "æ" );
			ReplaceOnString( ref a_str, "\\u00c7", a_bSimple ? "Z" : "Ç" );
			ReplaceOnString( ref a_str, "\\u00e7", a_bSimple ? "z" : "ç" );
			ReplaceOnString( ref a_str, "\\u00df", a_bSimple ? "s" : "ß" );

			ReplaceOnString( ref a_str, "\\u00c0", a_bSimple ? "A" : "À" );
			ReplaceOnString( ref a_str, "\\u00e0", a_bSimple ? "a" : "à" );
			ReplaceOnString( ref a_str, "\\u00c8", a_bSimple ? "E" : "È" );
			ReplaceOnString( ref a_str, "\\u00e8", a_bSimple ? "e" : "è" );
			ReplaceOnString( ref a_str, "\\u00cc", a_bSimple ? "I" : "Ì" );
			ReplaceOnString( ref a_str, "\\u00ec", a_bSimple ? "i" : "ì" );
			ReplaceOnString( ref a_str, "\\u00d2", a_bSimple ? "O" : "Ò" );
			ReplaceOnString( ref a_str, "\\u00f2", a_bSimple ? "o" : "ò" );
			ReplaceOnString( ref a_str, "\\u00d9", a_bSimple ? "U" : "Ù" );
			ReplaceOnString( ref a_str, "\\u00f9", a_bSimple ? "u" : "ù" );

			ReplaceOnString( ref a_str, "\\u00c2", a_bSimple ? "A" : "Â" );
			ReplaceOnString( ref a_str, "\\u00e2", a_bSimple ? "a" : "â" );
			ReplaceOnString( ref a_str, "\\u00ca", a_bSimple ? "E" : "Ê" );
			ReplaceOnString( ref a_str, "\\u00ea", a_bSimple ? "e" : "ê" );
			ReplaceOnString( ref a_str, "\\u00ce", a_bSimple ? "I" : "Î" );
			ReplaceOnString( ref a_str, "\\u00ee", a_bSimple ? "i" : "î" );
			ReplaceOnString( ref a_str, "\\u00d4", a_bSimple ? "O" : "Ô" );
			ReplaceOnString( ref a_str, "\\u00f4", a_bSimple ? "o" : "ô" );
			ReplaceOnString( ref a_str, "\\u00db", a_bSimple ? "U" : "Û" );
			ReplaceOnString( ref a_str, "\\u00fb", a_bSimple ? "u" : "û" );

			ReplaceOnString( ref a_str, "\\u00c1", a_bSimple ? "A" : "Á" );
			ReplaceOnString( ref a_str, "\\u00e1", a_bSimple ? "a" : "á" );
			ReplaceOnString( ref a_str, "\\u00c9", a_bSimple ? "E" : "É" );
			ReplaceOnString( ref a_str, "\\u00e9", a_bSimple ? "e" : "é" );
			ReplaceOnString( ref a_str, "\\u00cd", a_bSimple ? "I" : "Í" );
			ReplaceOnString( ref a_str, "\\u00ed", a_bSimple ? "i" : "í" );
			ReplaceOnString( ref a_str, "\\u00d3", a_bSimple ? "O" : "Ó" );
			ReplaceOnString( ref a_str, "\\u00f3", a_bSimple ? "o" : "ó" );
			ReplaceOnString( ref a_str, "\\u00da", a_bSimple ? "U" : "Ú" );
			ReplaceOnString( ref a_str, "\\u00fa", a_bSimple ? "u" : "ú" );

			ReplaceOnString( ref a_str, "\\u00c4", a_bSimple ? "A" : "Ä" );
			ReplaceOnString( ref a_str, "\\u00e4", a_bSimple ? "a" : "ä" );
			ReplaceOnString( ref a_str, "\\u00cb", a_bSimple ? "E" : "Ë" );
			ReplaceOnString( ref a_str, "\\u00eb", a_bSimple ? "e" : "ë" );
			ReplaceOnString( ref a_str, "\\u00ef", a_bSimple ? "i" : "ï" );
			ReplaceOnString( ref a_str, "\\u00cf", a_bSimple ? "I" : "Ï" );
			ReplaceOnString( ref a_str, "\\u00f6", a_bSimple ? "o" : "ö" );
			ReplaceOnString( ref a_str, "\\u00d6", a_bSimple ? "O" : "Ö" );
			ReplaceOnString( ref a_str, "\\u00fc", a_bSimple ? "u" : "ü" );
			ReplaceOnString( ref a_str, "\\u00dc", a_bSimple ? "U" : "Ü" );
			
			ReplaceOnString( ref a_str, "\\u2022", a_bSimple ? "•" : "•" );

			ReplaceOnString( ref a_str, "&#x00DF;", a_bSimple ? "s" : "ß" );
			ReplaceOnString( ref a_str, "&#x00C4;", a_bSimple ? "A" : "Ä" );
			ReplaceOnString( ref a_str, "&#x00E4;", a_bSimple ? "a" : "ä" );
			ReplaceOnString( ref a_str, "&#x00CB;", a_bSimple ? "E" : "Ë" );
			ReplaceOnString( ref a_str, "&#x00EB;", a_bSimple ? "e" : "ë" );
			ReplaceOnString( ref a_str, "&#x00EF;", a_bSimple ? "i" : "ï" );
			ReplaceOnString( ref a_str, "&#x00CF;", a_bSimple ? "I" : "Ï" );
			ReplaceOnString( ref a_str, "&#x00F6;", a_bSimple ? "o" : "ö" );
			ReplaceOnString( ref a_str, "&#x00D6;", a_bSimple ? "O" : "Ö" );
			ReplaceOnString( ref a_str, "&#x00FC;", a_bSimple ? "u" : "ü" );
			ReplaceOnString( ref a_str, "&#x00DC;", a_bSimple ? "U" : "Ü" );
			ReplaceOnString (ref a_str, "&#34;", "\"");

			ReplaceOnString( ref a_str, "\\r\\n", "\n" );
			ReplaceOnString( ref a_str, "\\n", "\n" );
			ReplaceOnString( ref a_str, "\\t", "\t" );
		}
	}

	///-----------------------------------------------------------------------------------
	/// <summary> Uses regular expression to check valid email </summary>
	/// <param name="a_strMail">the email</param>
	/// <returns>true if its valid; false if not.</returns>
	///-----------------------------------------------------------------------------------
	public static bool IsValidEmail( string a_strMail )
	{
		return Regex.IsMatch(a_strMail, @"^([\w-\._]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$" ); 
	}
}

public enum TEXTURE_RESOLUTION
{
	ZONE_HUGE,
	ZONE_BIG,
	ZONE_MEDIUM,
	ZONE_SMALL,
	OBJECT,
	THUMBNAIL,
	ADVERTISEMENT_BIG,
	ADVERTISEMENT_SMALL,
	PHOTO_ASSET,
	PHOTO_ASSET_360,
	NONE
}