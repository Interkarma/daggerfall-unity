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
		//_BumpMap("Normal Map", 2D) = "bump" {} // activating _BumpMap gives a warning in unity inspector for that shader/material
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

//#if defined(SHADER_API_D3D11) || defined(SHADER_API_XBOXONE) || defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)
//#define UNITY_SAMPLE_TEX2DARRAY_GRAD(tex,coord,dx,dy) tex.SampleGrad (sampler##tex,coord,dx,dy)
//#else
//#if defined(UNITY_COMPILER_HLSL2GLSL) || defined(SHADER_TARGET_SURFACE_ANALYSIS)
//#define UNITY_SAMPLE_TEX2DARRAY_GRAD(tex,coord,dx,dy) tex2DArray(tex,coord,dx,dy)
//#endif
//#endif

		UNITY_DECLARE_TEX2DARRAY(_TileTexArr);

		struct Input
		{
			float2 uv_MainTex;
			//float2 uv_BumpMap;
			float3 worldPos; // interpolated vertex positions used for correct coast line texturing
			//INTERNAL_DATA
		};

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			// Get offset to tile in atlas
			int index = tex2D(_TilemapTex, IN.uv_MainTex).x * _MaxIndex;

			// Offset to fragment position inside tile
			float2 uv = fmod(IN.uv_MainTex * 128.0f, 1.0f);

			// Sample based on gradient and set output
			float3 uv3 = float3(uv, index);
			//half4 c = UNITY_SAMPLE_TEX2DARRAY(_TileTexArr, uv3); // ugly seams due to automatic mip map level selection algorithm being confused by our texture coordinate computation
			//half4 c = UNITY_SAMPLE_TEX2DARRAY_LOD(_TileTexArr, uv3, 0); // no seems but either blurry or noisy when fixed to a certain level for every distance
			//half4 c = UNITY_SAMPLE_TEX2DARRAY_GRAD(_TileTexArr, uv3, ddx(uv), ddy(uv)); // (see https://forum.unity3d.com/threads/texture2d-array-mipmap-troubles.416799/) - would like to use, but even though it is mentioned that it "should" work, it does not because of shader errors

			// since there is currently no UNITY_SAMPLE_TEX2DARRAY_GRAD function in unity, this is used as workaround
			// mip map level is selected manually dependent on fragment's distance from camera
			float dist = distance(IN.worldPos.xz, _WorldSpaceCameraPos.xz);
			
			//half4 c = UNITY_SAMPLE_TEX2DARRAY_LOD(_TileTexArr, uv3, dist/20.0f);

			half4 c;
			if (dist < 10.0f)
			c = UNITY_SAMPLE_TEX2DARRAY_LOD(_TileTexArr, uv3, 0);
			else
			c = UNITY_SAMPLE_TEX2DARRAY(_TileTexArr, uv3);

			o.Albedo = c.rgb;
			o.Alpha = c.a;
			//o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.Metallic = 0;
		}
		ENDCG
	} 
	FallBack "Standard"
}
