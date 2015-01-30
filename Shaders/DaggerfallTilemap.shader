// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

Shader "Daggerfall/Tilemap" {
	Properties {
		_MainTex ("Tileset Atlas (RGB)", 2D) = "white" {}
		_TilemapTex("Tilemap (R)", 2D) = "red" {}
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

		sampler2D _MainTex;
		sampler2D _TilemapTex;
		int _TilesetDim;
		int _TilemapDim;
		int _MaxIndex;
		float _AtlasSize;
		float _GutterSize;

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o)
		{
			// Get offset to tile in atlas
			int index = tex2D(_TilemapTex, IN.uv_MainTex).x * _MaxIndex;
			int xpos = index % _TilesetDim;
			int ypos = index / _TilesetDim;
			float2 uv = float2(xpos, ypos) / _TilesetDim;

			// Offset to fragment position inside tile
			float xoffset = frac(IN.uv_MainTex.x * _TilemapDim) / _GutterSize;
			float yoffset = frac(IN.uv_MainTex.y * _TilemapDim) / _GutterSize;
			uv += float2(xoffset, yoffset) + _GutterSize / _AtlasSize;

			// Sample based on gradient and set output
			float2 uvr = IN.uv_MainTex * ((float)_TilemapDim / _GutterSize);
			half4 c = tex2Dgrad(_MainTex, uv, ddx(uvr), ddy(uvr));
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
