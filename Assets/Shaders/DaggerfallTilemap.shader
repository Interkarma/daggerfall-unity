// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2016 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

Shader "Daggerfall/Tilemap" {
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
		_TileAtlasTex ("Tileset Atlas (RGB)", 2D) = "white" {}
		_TilemapTex("Tilemap (R)", 2D) = "red" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		_TilesetDim("Tileset Dimension (in tiles)", Int) = 16
		_TilemapDim("Tilemap Dimension (in tiles)", Int) = 128
		_MaxIndex("Max Tileset Index", Int) = 255
		_AtlasSize("Atlas Size (in pixels)", Float) = 2048.0
		_GutterSize("Gutter Size (in pixels)", Float) = 32.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Lambert
		#pragma glsl

		sampler2D _TileAtlasTex;
		sampler2D _TilemapTex;
		sampler2D _BumpMap;
		uint _TilesetDim;
		uint _TilemapDim;
		uint _MaxIndex;
		float _AtlasSize;
		float _GutterSize;

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
		};

		void surf (Input IN, inout SurfaceOutput o)
		{
			float2 unwrappedUV = IN.uv_MainTex * _TilemapDim;

			// Get offset to tile in atlas
			uint index = tex2D(_TilemapTex, floor(unwrappedUV) / _TilemapDim).a * _MaxIndex + 0.5;
			uint xpos = index % _TilesetDim;
			uint ypos = index / _TilesetDim;
			float2 uv = float2(xpos, ypos) / _TilesetDim;

			// Offset to fragment position inside tile
			float2 offset = frac(unwrappedUV) / _GutterSize;
			uv += offset + _GutterSize / _AtlasSize;

			// Sample based on gradient and set output
			float2 uvr = unwrappedUV / _GutterSize;
			half4 c = tex2Dgrad(_TileAtlasTex, uv, ddx(uvr), ddy(uvr));
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		}
		ENDCG
	} 
	FallBack "Mobile/VertexLit"
}
