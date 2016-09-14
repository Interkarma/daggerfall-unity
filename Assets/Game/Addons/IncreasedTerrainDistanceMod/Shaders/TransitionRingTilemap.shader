//Increased Terrain Distance Mod for Daggerfall Tools For Unity
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: Michael Rauter (a.k.a. Nystul)
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)
//based on the DaggerfallTilemap shader by Gavin Clayton (interkarma@dfworkshop.net)

Shader "Daggerfall/TransitionRingTilemap" {
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
		//_FarTerrainTilesetDim("Tileset Dimension (in tiles)", Int) = 16
		_FarTerrainTilemapDim("Tilemap Dimension (in tiles)", Int) = 1000
		_TileAtlasTex ("Tileset Atlas (RGB)", 2D) = "white" {}
		_TilemapTex("Tilemap (R)", 2D) = "red" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		_TilesetDim("Tileset Dimension (in tiles)", Int) = 16
		_TilemapDim("Tilemap Dimension (in tiles)", Int) = 128
		_MaxIndex("Max Tileset Index", Int) = 255
		_AtlasSize("Atlas Size (in pixels)", Float) = 2048.0
		_GutterSize("Gutter Size (in pixels)", Float) = 32.0

		_blendWeightFarTerrainTop("blend weight at top side of terrain block", Float) = 0.0
		_blendWeightFarTerrainBottom("blend weight at bottom side of terrain block", Float) = 0.0
		_blendWeightFarTerrainLeft("blend weight on left side of terrain block", Float) = 0.0
		_blendWeightFarTerrainRight("blend weight on right side of terrain block", Float) = 0.0

		_SeaReflectionTex("Reflection Texture Sea Reflection", 2D) = "black" {}
		_UseSeaReflectionTex("specifies if sea reflection texture is used", Int) = 0

		_TileAtlasReflectiveTex("Tileset Reflection Atlas (RGB)", 2D) = "black" {}		

		_MapPixelX("MapPixelX on world map", Int) = 0
		_MapPixelY("MapPixelY on world map", Int) = 0
		_PlayerPosX("Player Position in X-direction on world map", Int) = 0
		_PlayerPosY("Player Position in X-direction on world map", Int) = 0
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
		Tags { "RenderType"="Opaque" "Queue" = "Transparent-499"} // Transparent-499 ... first non-opaque object (otherwise reflections are unlimited in distance), workaround for otherwise incorrect rendering of WorldTerrain defined geometry in different layers than "WorldTerrain"
		//Tags { "RenderType"="Opaque"}
		LOD 200

		// extra pass that renders to depth buffer only (world terrain is semi-transparent) - important for reflections to work
		Pass {
			ZWrite On
			Cull Back
			ColorMask 0		
		}
		
		CGPROGRAM

		#include "FarTerrainCommon.cginc"

		// only used in transition ring tilemap shader, so not in FarTerrainCommon.cginc
		sampler2D _TileAtlasReflectiveTex;

		#pragma target 3.0
		#pragma surface surf Standard noforwardadd finalcolor:fcolor alpha:fade keepalpha
		#pragma glsl

		#pragma multi_compile __ ENABLE_WATER_REFLECTIONS

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			half4 c; // output color value
		
			int mapPixelX = _MapPixelX;
			int mapPixelY = _MapPixelY;

			int posX = mapPixelX;
			int posY = 500 - mapPixelY;

			float2 uvTex =	float2(
									(float)posX/(float)_FarTerrainTilemapDim + (1.0f/_FarTerrainTilemapDim)*IN.uv_MainTex.x,
									(float)posY/(float)(_FarTerrainTilemapDim) + (1.0f/_FarTerrainTilemapDim)*IN.uv_MainTex.y
							);

			float4 terrainTileInfo = tex2D(_FarTerrainTilemapTex, uvTex).rgba;

			// fragments more distant than _BlendEnd will be discarded as well
			const float fadeRange = _BlendEnd - _BlendStart + 1.0f;
			float dist = distance(IN.worldPos.xz, _WorldSpaceCameraPos.xz); //max(abs(IN.worldPos.x - _WorldSpaceCameraPos.x), abs(IN.worldPos.z - _WorldSpaceCameraPos.z));			
			if (dist>_BlendEnd)
			{
				discard;
			}

			int index = terrainTileInfo.r * _MaxIndex;

			c = getColorFromTerrain(IN, uvTex, _TilemapDim, _TilesetDim, index);
			
			float treeCoverage = terrainTileInfo.g;
			int locationRangeX = terrainTileInfo.b * _MaxIndex;
			int locationRangeY = terrainTileInfo.a * _MaxIndex;
			c.rgb = updateColorWithInfoForTreeCoverageAndLocations(c.rgb, treeCoverage, locationRangeX, locationRangeY, mapPixelX, mapPixelY, uvTex);			
			
			// Get offset to tile in atlas
			index = tex2D(_TilemapTex, IN.uv_MainTex).x * _MaxIndex;
			int xpos = index % _TilesetDim;
			int ypos = index / _TilesetDim;
			float2 uv = float2(xpos, ypos) / _TilesetDim;

			// Offset to fragment position inside tile
			float xoffset = frac(IN.uv_MainTex.x * _TilemapDim) / _GutterSize;
			float yoffset = frac(IN.uv_MainTex.y * _TilemapDim) / _GutterSize;
			uv += float2(xoffset, yoffset) + _GutterSize / _AtlasSize;

			// Sample based on gradient and set output
			float2 uvr = IN.uv_MainTex * ((float)_TilemapDim / _GutterSize);
			half4 c2 = tex2Dgrad(_TileAtlasTex, uv, ddx(uvr), ddy(uvr));
			//o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));


			float blendWeightX = lerp(_blendWeightFarTerrainLeft, _blendWeightFarTerrainRight, IN.uv_MainTex.x);
			float blendWeightY = lerp(_blendWeightFarTerrainTop, _blendWeightFarTerrainBottom, IN.uv_MainTex.y);
			float blendWeightCombined = 1.0f - max(blendWeightX, blendWeightY);

			#if defined(ENABLE_WATER_REFLECTIONS)
			if (_UseSeaReflectionTex)
			{
				float2 screenUV = IN.screenPos.xy / IN.screenPos.w;

				fixed3 normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
				float3 worldNormal = normalize(WorldNormalVector(IN, normal));

				float reflAmount;

				reflAmount = tex2D(_TileAtlasReflectiveTex, uv).r; //tex2Dgrad(_TileAtlasReflectiveTex, uv, ddx(uvr), ddy(uvr));

				if (reflAmount > 0.25f)
				{
					float3 refl = tex2D(_SeaReflectionTex, screenUV).rgb;

					c2.rgb = c2.rgb * (1.0f - reflAmount) + reflAmount * refl.rgb;
					//c2.rgb = 0.5f * c2.rgb + 0.5f * refl.rgb;
					blendWeightCombined = 1.0f;
				}				
			}
			#endif

			c.rgb = lerp(c.rgb, c2.rgb, blendWeightCombined);

			o.Albedo = c.rgb;
		}

		ENDCG
	} 
	FallBack "Standard"
}
