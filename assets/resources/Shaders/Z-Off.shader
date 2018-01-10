﻿Shader "Custom/Z-Off" {
	Properties 
 {
     _Color ("Main Color", Color) = (1,1,1,1)
     _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
 }
 
 Category 
 {
     SubShader 
     { 
			 Tags { "Queue"="Overlay+1"
		 "RenderType"="Transparent"}
      
		 Pass
		 {
			 ZWrite Off
			 ZTest Greater
			 Lighting Off
			 Color [_Color]
		 }
      
		 Pass
		 {
			 Blend SrcAlpha OneMinusSrcAlpha
			 ZTest Less
			  SetTexture [_MainTex] {combine texture}
		 }
     }
 }
 
 FallBack "Specular", 1
}
