//ReflectionsMod for Daggerfall Tools For Unity
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: Michael Rauter (a.k.a. Nystul)
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)

Shader "Daggerfall/ReflectionsMod/TilemapWithReflections" {
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
		_ReflectionGroundTex("Reflection Texture Ground Reflection", 2D) = "black" {}
		_ReflectionSeaTex("Reflection Texture Sea Reflection", 2D) = "black" {}
		_TileAtlasReflectiveTex ("Tileset Reflection Atlas (RGB)", 2D) = "black" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		_TilesetDim("Tileset Dimension (in tiles)", Int) = 16
		_TilemapDim("Tilemap Dimension (in tiles)", Int) = 128
		_MaxIndex("Max Tileset Index", Int) = 255
		_AtlasSize("Atlas Size (in pixels)", Float) = 2048.0
		_GutterSize("Gutter Size (in pixels)", Float) = 32.0
		_SeaLevelHeight("sea level height", Float) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard
		#pragma glsl

		sampler2D _TileAtlasTex;
		sampler2D _TilemapTex;
		sampler2D _BumpMap;
		sampler2D _ReflectionGroundTex;
		sampler2D _ReflectionSeaTex;
		sampler2D _TileAtlasReflectiveTex;
		int _TilesetDim;
		int _TilemapDim;
		int _MaxIndex;
		float _AtlasSize;
		float _GutterSize;
		float _SeaLevelHeight;

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 worldPos;
			float3 worldNormal;
			float4 screenPos;
			INTERNAL_DATA
		};

		void surf (Input IN, inout SurfaceOutputStandard o)
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
			half4 c = tex2Dgrad(_TileAtlasTex, uv, ddx(uvr), ddy(uvr));

			float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
			
			half3 refl;
			if (IN.worldPos.y > _SeaLevelHeight + 0.01f)
			{					
				refl = tex2Dlod(_ReflectionGroundTex, float4(screenUV, 0.0f, 1.0f)).rgb; // 4th component is blurring of reflection				
			}
			else
			{
				refl = tex2Dlod(_ReflectionSeaTex, float4(screenUV, 0.0f, 1.5f)).rgb;
			}

			o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
			float3 worldNormal = normalize(WorldNormalVector (IN, o.Normal));

			float reflAmount;
			const float3 upVec = float3(0.0f, 1.0f, 0.0f);
			if (dot(worldNormal, upVec) > 0.99f)
			{
				//reflAmount = tex2D(_TileAtlasReflectiveTex, uv).r; //0.5f;
				reflAmount = tex2Dgrad(_TileAtlasReflectiveTex, uv, ddx(uvr), ddy(uvr));
			}
			else
			{
				reflAmount = 0.0f;
			}

			c.rgb = c.rgb * (1.0f - reflAmount) + reflAmount * refl.rgb;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.Metallic = 0.0f;
		}
		ENDCG
	} 
	FallBack "Standard"
}
