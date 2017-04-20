// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity
// Contributor:		Nystul

Shader "Daggerfall/TilemapTextureArray" {
	Properties {
		// These params are required to stop terrain system throwing errors
		// However we won't be using them as Unity likes to init these textures
		// and will overwrite any assignments we already made
		// TODO: Combine splat painting with tilemapping
		[HideInInspector] _MainTex("BaseMap (RGB)", 2D) = "white" {}
		[HideInInspector] _Control ("Control (RGBA)", 2D) = "red" {}
		[HideInInspector] _SplatTex3("Layer 3 (A)", 2D) = "white" {}
		[HideInInspector] _SplatTex2("Layer 2 (B)", 2D) = "white" {}
		[HideInInspector] _SplatTex1("Layer 1 (G)", 2D) = "white" {}
		[HideInInspector] _SplatTex0("Layer 0 (R)", 2D) = "white" {}

		// These params are used for our shader
		_TileTexArr("Tile Texture Array", 2DArray) = "" {}
		_TilemapTex("Tilemap (R)", 2D) = "red" {}
		//_BumpMap("Normal Map", 2D) = "bump" {}
		_TilemapDim("Tilemap Dimension (in tiles)", Int) = 128
		_MaxIndex("Max Tileset Index", Int) = 255
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma target 3.5
		#pragma surface surf Standard
		#pragma glsl

		sampler2D _TilemapTex;
		sampler2D _BumpMap;
		int _MaxIndex;
		int _TilemapDim;

		UNITY_DECLARE_TEX2DARRAY(_TileTexArr);

		struct Input
		{
			float2 uv_MainTex;
			//float2 uv_BumpMap;
		};

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			// Get offset to tile in atlas
			int index = tex2D(_TilemapTex, IN.uv_MainTex).x * _MaxIndex;

			// Offset to fragment position inside tile
			float xoffset = frac(IN.uv_MainTex.x * _TilemapDim);
			float yoffset = frac(IN.uv_MainTex.y * _TilemapDim);
			float2 uv = IN.uv_MainTex; // *float2(xoffset, yoffset);

			// Sample based on gradient and set output
			//half4 c = tex2Dgrad(_TileAtlasTex, uv, ddx(uvr), ddy(uvr));
			float3 uv3 = float3(fmod(uv.x * 128.0f, 1.0f), fmod(uv.y * 128.0f, 1.0f), index);
			half4 c = UNITY_SAMPLE_TEX2DARRAY(_TileTexArr, uv3);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			//o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.Metallic = 0;
		}
		ENDCG
	} 
	FallBack "Standard"
}
