Shader "Custom/Fade" {

	Properties
	{
		_Color ("Main Color", Color) = (0.5,0.5,0.5,0.5)
	}
	
	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent+99"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Cull Off
		Lighting Off
		ZTest Always
		ZWrite Off
		Fog { Mode Off }
		Offset -1, -1
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				
				#include "UnityCG.cginc"
	
				struct appdata_t
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
				};
	
				struct v2f
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
				};
	
				fixed4 _Color;
				
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR
				{
					fixed4 col = i.color * _Color;
					return col;
				}
			ENDCG
		}
	}
	
	
}

