//Increased Terrain Distance Mod for Daggerfall Tools For Unity
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: Michael Rauter (a.k.a. Nystul)
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)

Shader "Daggerfall/IncreasedTerrainTilemap" {
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
		_TileAtlasTexDesert ("Tileset Atlas (RGB)", 2D) = "white" {}
		_TileAtlasTexWoodland ("Tileset Atlas (RGB)", 2D) = "white" {}
		_TileAtlasTexMountain ("Tileset Atlas (RGB)", 2D) = "white" {}
		_TileAtlasTexSwamp ("Tileset Atlas (RGB)", 2D) = "white" {}
	    _SkyTex("Sky Texture", 2D) = "white" {}
		_FarTerrainTilemapTex("Tilemap (R)", 2D) = "red" {}
		_FarTerrainTilesetDim("Tileset Dimension (in tiles)", Int) = 16
		_FarTerrainTilemapDim("Tilemap Dimension (in tiles)", Int) = 1000
		_MaxIndex("Max Tileset Index", Int) = 255
		_AtlasSize("Atlas Size (in pixels)", Float) = 2048.0
		_GutterSize("Gutter Size (in pixels)", Float) = 32.0
		
		_SeaReflectionTex("Reflection Texture Sea Reflection", 2D) = "black" {}
		_UseSeaReflectionTex("specifies if sea reflection texture is used", Int) = 0

		_PlayerPosX("Player Position in X-direction on world map", Int) = 0
		_PlayerPosY("Player Position in Y-direction on world map", Int) = 0
		_TerrainDistance("Terrain Distance", Int) = 3
		_WaterHeightTransformed("water level on y-axis in world coordinates", Float) = 58.9
		_TextureSetSeasonCode("specifies which seasonal/weather texture set is used (0...Summer, 1...Winter/Snow, 2...Rain)", Int) = 0
		_BlendStart("blend start distance for blending distant terrain into skybox", Float) = 15000.0
		_BlendEnd("blend end distance for blending distant terrain into skybox", Float) = 200000.0
		_FogMode("Fog Mode", Int) = 1
		_FogDensity("Fog Density", Float) = 0.01 // since unity_FogParams are no longer working for some reason use a shaderlab parameter
		_FogStartDistance("Fog Start Distance", Float) = 0 // since unity_FogParams are no longer working for some reason use a shaderlab parameter
		_FogEndDistance("Fog End Distance", Float) = 2000 // since unity_FogParams are no longer working for some reason use a shaderlab parameter
		_FogFromSkyTex("specifies if fog color should be derived from sky texture or not", Int) = 0
	}
	SubShader {
		//Tags{ "RenderType" = "Opaque" "DisableBatching" = "True" "Queue" = "Geometry" }
		//LOD 200
		//ZWrite On

		//Tags { "RenderType"="Opaque" "Queue" = "Transparent-499"} // Transparent-499 ... first non-opaque object (otherwise reflections are unlimited in distance), workaround for otherwise incorrect rendering of WorldTerrain defined geometry in different layers than "WorldTerrain"
		Tags { "RenderType"="Transparent" "Queue" = "Transparent-499"} // Transparent-499 ... first non-opaque object (otherwise reflections are unlimited in distance), workaround for otherwise incorrect rendering of WorldTerrain defined geometry in different layers than "WorldTerrain"
		LOD 200
		
		ZWrite Off
		Cull Back

		CGPROGRAM

		#pragma target 3.0		
		//#pragma surface surf Lambert vertex:vert alpha:fade keepalpha finalcolor:fcolor noforwardadd
		#pragma surface surf Standard noforwardadd finalcolor:fcolor alpha:fade keepalpha
		#pragma glsl
		#pragma multi_compile __ ENABLE_WATER_REFLECTIONS

		#include "FarTerrainCommon.cginc"

		//void vert(inout appdata_full v, out Input OUT)
		//{	
		//	float4 vertex = v.vertex;
		//	float3 worldpos = mul(_Object2World, vertex).xyz;
		//	float dist = distance(worldpos.xz, _WorldSpaceCameraPos.xz);
		//	float curvature_start = 25000.0f;
		//	if (dist > curvature_start)
		//	{
		//		worldpos.y = worldpos.y * max(0.0f, ((curvature_start + 100000.0f - dist) / (curvature_start + 100000.0f)));
		//	}
		//	v.vertex = mul(_World2Object, float4(worldpos.x, worldpos.y, worldpos.z, 1.0f));
		//	UNITY_INITIALIZE_OUTPUT(Input, OUT);
		//	OUT.worldPos = worldpos;
		//}

		void surf (Input IN, inout SurfaceOutputStandard o)
		{		
			half4 c; // output color value

			float4 terrainTileInfo = tex2D(_FarTerrainTilemapTex, IN.uv_MainTex).rgba;

			int mapPixelX = IN.uv_MainTex.x*_FarTerrainTilemapDim;
			int mapPixelY = 499 - IN.uv_MainTex.y*_FarTerrainTilemapDim;

			// fragment discarding inside area spanned by _TerrainDistance and one extra ring of blocks (these will be rendered by TransitionRingTilemap shader)
			if ((abs(mapPixelX+1-_PlayerPosX)<=_TerrainDistance+1)&&(abs(mapPixelY+1-_PlayerPosY)<=_TerrainDistance+1))
			{
				// for debugging fragment discard area use red color (also used to debug world terrain positioning with floating origin script)
				//float4 ret = float4(1.0f,0.0f,0.0f,1.0f); o.Albedo = ret.rgb; o.Alpha = ret.a; return;
				discard;
			}

			// fragments more distant than _BlendEnd will be discarded as well
			const float fadeRange = _BlendEnd - _BlendStart + 1.0f;
			float dist = distance(IN.worldPos.xz, _WorldSpaceCameraPos.xz); //max(abs(IN.worldPos.x - _WorldSpaceCameraPos.x), abs(IN.worldPos.z - _WorldSpaceCameraPos.z));			
			if (dist>_BlendEnd)
			{
				discard;
			}

			int index = terrainTileInfo.r * _MaxIndex;

			c = getColorFromTerrain(IN, IN.uv_MainTex, _FarTerrainTilemapDim, _FarTerrainTilesetDim, index);
			
			float treeCoverage = terrainTileInfo.g;
			int locationRangeX = terrainTileInfo.b * _MaxIndex;
			int locationRangeY = terrainTileInfo.a * _MaxIndex;
			c.rgb = updateColorWithInfoForTreeCoverageAndLocations(c.rgb, treeCoverage, locationRangeX, locationRangeY, mapPixelX, mapPixelY, IN.uv_MainTex);

			o.Albedo = c.rgb;
		}
		ENDCG

		// extra pass that renders to depth buffer only (world terrain is semi-transparent) - important for reflections to work
		Pass {						
			ZWrite On
			Cull Back
			ColorMask 0
		}

		ZWrite On
		Cull Back

		CGPROGRAM

		#pragma target 3.0		
		#pragma surface surf Standard noforwardadd finalcolor:fcolor alpha:fade keepalpha
		#pragma glsl
		#pragma multi_compile __ ENABLE_WATER_REFLECTIONS

		#include "FarTerrainCommon.cginc"

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			half4 c; // output color value

			float4 terrainTileInfo = tex2D(_FarTerrainTilemapTex, IN.uv_MainTex).rgba;

			int mapPixelX = IN.uv_MainTex.x*_FarTerrainTilemapDim;
			int mapPixelY = 499 - IN.uv_MainTex.y*_FarTerrainTilemapDim;

			// fragment discarding inside area spanned by _TerrainDistance and one extra ring of blocks (these will be rendered by TransitionRingTilemap shader)
			if ((abs(mapPixelX+1-_PlayerPosX)<=_TerrainDistance+1)&&(abs(mapPixelY+1-_PlayerPosY)<=_TerrainDistance+1))
			{
				// for debugging fragment discard area use red color (also used to debug world terrain positioning with floating origin script)
				//float4 ret = float4(1.0f,0.0f,0.0f,1.0f); o.Albedo = ret.rgb; o.Alpha = ret.a; return;
				discard;
			}

			// fragments more distant than _BlendEnd will be discarded as well
			const float fadeRange = _BlendEnd - _BlendStart + 1.0f;
			float dist = distance(IN.worldPos.xz, _WorldSpaceCameraPos.xz); //max(abs(IN.worldPos.x - _WorldSpaceCameraPos.x), abs(IN.worldPos.z - _WorldSpaceCameraPos.z));			
			if (dist>_BlendEnd)
			{
				discard;
			}

			int index = terrainTileInfo.r * _MaxIndex;

			c = getColorFromTerrain(IN, IN.uv_MainTex, _FarTerrainTilemapDim, _FarTerrainTilesetDim, index);
			
			float treeCoverage = terrainTileInfo.g;
			int locationRangeX = terrainTileInfo.b * _MaxIndex;
			int locationRangeY = terrainTileInfo.a * _MaxIndex;
			c.rgb = updateColorWithInfoForTreeCoverageAndLocations(c.rgb, treeCoverage, locationRangeX, locationRangeY, mapPixelX, mapPixelY, IN.uv_MainTex);

			o.Albedo = c.rgb;
		}
		ENDCG
	} 


	FallBack "Diffuse"
}
