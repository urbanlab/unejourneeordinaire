// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/YUVBiplanarFlipped" {
Properties {
    _MainTex ("Y Channel", 2D) = "white" { }
    _MainTex2 ("UV Channel", 2D) = "white" { }

}
SubShader {
    Pass {
 
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
 
#include "UnityCG.cginc"
 
sampler2D _MainTex;
sampler2D _MainTex2;

struct v2f {
    float4  pos : SV_POSITION;
    float2  uv : TEXCOORD0;
};
 
float4 _MainTex_ST;
 
v2f vert (appdata_base v)
{
    v2f o;
    o.pos = UnityObjectToClipPos (v.vertex);
    v.texcoord.y = 1.0 - v.texcoord.y;
    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
    return o;
}
 
half4 frag (v2f i) : COLOR
{
	half4 YUV;

//sample the y channel
    YUV.x = tex2D (_MainTex, i.uv).r;
    //sample the UV channel
    YUV.yz = tex2D (_MainTex2, i.uv).rg - half2(0.5, 0.5);
    YUV.rgb = mul(YUV.xyz, float3x3(      1.0,       1.0,      1.0,
			   0.0, -.18732, 1.8556,
               1.57481, -.46813,      0));
	YUV.w = 1.0;
    return YUV;
}
ENDCG
 
    }
}
Fallback "VertexLit"
} 