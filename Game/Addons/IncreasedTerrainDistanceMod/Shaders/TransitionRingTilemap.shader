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
		_FarTerrainTilesetDim("Tileset Dimension (in tiles)", Int) = 16
		_FarTerrainTilemapDim("Tilemap Dimension (in tiles)", Int) = 1000
		_TileAtlasTex ("Tileset Atlas (RGB)", 2D) = "white" {}
		_TilemapTex("Tilemap (R)", 2D) = "red" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		_TilesetDim("Tileset Dimension (in tiles)", Int) = 16
		_TilemapDim("Tilemap Dimension (in tiles)", Int) = 128
		_MaxIndex("Max Tileset Index", Int) = 255
		_AtlasSize("Atlas Size (in pixels)", Float) = 2048.0
		_GutterSize("Gutter Size (in pixels)", Float) = 32.0

		_SeaReflectionTex("Reflection Texture Sea Reflection", 2D) = "black" {}
		_UseSeaReflectionTex("specifies if sea reflection texture is used", Int) = 0

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
		//Tags { "RenderType"="Opaque" "Queue" = "Overlay"} // Overlay is workaround for otherwise incorrect rendering of WorldTerrain defined geometry in different layers than "WorldTerrain"		
		Tags { "RenderType"="Opaque"} // Overlay is workaround for otherwise incorrect rendering of WorldTerrain defined geometry in different layers than "WorldTerrain"		
		LOD 200

		// extra pass that renders to depth buffer only (world terrain is semi-transparent) - important for correct blending
		Pass {
			ZWrite On
			ColorMask 0		
		}
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Lambert alpha:fade keepalpha finalcolor:fcolor noforwardadd
		//#pragma surface surf Lambert //alpha:fade keepalpha noforwardadd
		#pragma glsl

		sampler2D _TileAtlasTex;
		sampler2D _TilemapTex;
		sampler2D _BumpMap;
		int _TilesetDim;
		int _TilemapDim;
		int _MaxIndex;
		float _AtlasSize;
		float _GutterSize;

		#pragma multi_compile __ ENABLE_WATER_REFLECTIONS

		#define PI 3.1416f

		sampler2D _TileAtlasTexDesert;
		sampler2D _TileAtlasTexWoodland;
		sampler2D _TileAtlasTexMountain;
		sampler2D _TileAtlasTexSwamp;
		sampler2D _SkyTex;
		sampler2D _FarTerrainTilemapTex;		
		int _FarTerrainTilesetDim;
		int _FarTerrainTilemapDim;

		#if defined(ENABLE_WATER_REFLECTIONS)
			sampler2D _SeaReflectionTex;
			int _UseSeaReflectionTex;
		#endif

		float _WaterHeightTransformed;
		int _TerrainDistance;
		int _MapPixelX;
		int _MapPixelY;
		int _PlayerPosX;
		int _PlayerPosY;
		int _TextureSetSeasonCode;
		float _BlendStart;
		float _BlendEnd;

		int _FogMode;
		float _FogDensity;
		float _FogStartDistance;
		float _FogEndDistance;

		int _FogFromSkyTex;

		struct Input
		{
			float2 uv_MainTex;
			//float2 uv_BumpMap;
			float3 worldPos; // interpolated vertex positions used for correct coast line texturing
			float3 worldNormal; // interpolated vertex normals used for texturing terrain based on terrain slope
			float4 screenPos;
		};

		void fcolor (Input IN, SurfaceOutput o, inout fixed4 color)
		{			
			float dist = distance(IN.worldPos.xz, _WorldSpaceCameraPos.xz); //max(abs(IN.worldPos.x - _WorldSpaceCameraPos.x), abs(IN.worldPos.z - _WorldSpaceCameraPos.z));
			
			float blendFacTerrain = 1.0f;
						
			if (_FogMode == 1) // linear
			{
				//blendFacTerrain = max(0.0f, min(1.0f, dist * unity_FogParams.z + unity_FogParams.w));
				blendFacTerrain = max(0.0f, min(1.0f, (_FogEndDistance - dist) / (_FogEndDistance - _FogStartDistance + 1.0)));
			}
			if (_FogMode == 2) // exp
			{
				// factor = exp(-density*z)
				float fogFac = 0.0;
				fogFac = _FogDensity * dist;
				blendFacTerrain = exp2(-fogFac);

			}
			if (_FogMode == 3) // exp2
			{
				// factor = exp(-(density*z)^2)
				float fogFac = 0.0;
				fogFac = _FogDensity * dist;
				blendFacTerrain = exp2(-fogFac*fogFac);
			}
			
			const float fadeRange = _BlendEnd - _BlendStart + 1.0f;
			float alphaFadeAmount = max(0.0f, min(1.0f, (_BlendEnd - dist) / fadeRange));

			if (_FogFromSkyTex == 1)
			{
				float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
				color.rgb = blendFacTerrain * color.rgb + (1.0f - blendFacTerrain) * tex2D(_SkyTex, screenUV).rgb;
			}
			else
			{
				color.rgb = blendFacTerrain * color.rgb + (1.0f - blendFacTerrain) * unity_FogColor.rgb;
			}
			color.a = alphaFadeAmount;
		}

		
		half4 getColorByTextureAtlasIndex(Input IN, uniform sampler2D textureAtlas, uint index, float2 uvTex)
		{			
			const float textureCrispness = 3.5f; // defines how crisp textures of extended terrain are (higher values result in more crispness)
			const float textureCrispnessDiminishingFactor = 0.075f; // defines how fast crispness of textures diminishes with more distance from the player (the camera)
			const float distanceAttenuation = 0.001; // used to attenuated computed distance

			float dist = max(abs(IN.worldPos.x - _WorldSpaceCameraPos.x), abs(IN.worldPos.z - _WorldSpaceCameraPos.z));
			dist = floor(dist*distanceAttenuation);

			uint xpos = index % _TilesetDim;
			uint ypos = index / _TilesetDim;
			float2 uv = float2(xpos, ypos) / _TilesetDim;

			// Offset to fragment position inside tile
			float xoffset;
			float yoffset;
			// changed offset computation so that tile texture repeats over tile		
			//xoffset = frac(uvTex.x * _FarTerrainTilemapDim * 1/(max(1,dist * textureCrispnessDiminishingFactor)) * textureCrispness ) / _GutterSize;
			//yoffset = frac(uvTex.y * _FarTerrainTilemapDim * 1/(max(1,dist * textureCrispnessDiminishingFactor)) * textureCrispness ) / _GutterSize;
			xoffset = frac(uvTex.x * _FarTerrainTilemapDim * 1/(max(1,dist * textureCrispnessDiminishingFactor)) * textureCrispness) / _GutterSize;
			yoffset = frac(uvTex.y * _FarTerrainTilemapDim * 1/(max(1,dist * textureCrispnessDiminishingFactor)) * textureCrispness) / _GutterSize;
			 
			uv += float2(xoffset, yoffset) + _GutterSize / _AtlasSize;

			// Sample based on gradient and set output
			//float2 uvr = IN.uv_MainTex; // * ((float)_FarTerrainTilemapDim / _GutterSize);
			float2 uvr = uvTex * ((float)_TilemapDim / _GutterSize);
			half4 c = tex2Dgrad(textureAtlas, uv, ddx(uvr), ddy(uvr));
			return(c);
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			const float limitAngleDirtTexture = 12.5f * PI / 180.0f; // tile will get dirt texture assigned if angles definied by surface normal and up-vector is larger than this value (and not larger than limitAngleStoneTexture)
			const float limitAngleStoneTexture = 20.5f * PI / 180.0f; // tile will get stone texture assigned if angles definied by surface normal and up-vector is larger than this value

			half4 c; // output color value
			
			half4 c_g; // color value from grass texture
			half4 c_d; // color value from dirt texture
			half4 c_s; // color value from stone texture

			float weightGrass = 1.0f;
			float weightDirt = 0.0f;
			float weightStone = 0.0f;

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

			const float3 upVec = float3(0.0f, 1.0f, 0.0f);
			float dotResult = dot(normalize(IN.worldNormal), upVec);


			if (acos(dotResult) < limitAngleDirtTexture) // between angles 0 to limitAngleDirtTexture interpolate between grass and dirt texture
			{
				weightGrass = 1.0f - acos(dotResult) / limitAngleDirtTexture;
				weightDirt = acos(dotResult) / limitAngleDirtTexture;
				weightStone = 0.0f;
			}
			else // between angles limitAngleDirtTexture to limitAngleStoneTexture interpolate between dirt and stone texture (limitAngleStoneTexture to 90 degrees -> also stone texture)
			{
				weightGrass = 0.0f;
				weightDirt = 1.0f - min(1.0f, (acos(dotResult) - limitAngleDirtTexture) / limitAngleStoneTexture);
				weightStone = min(1.0f, (acos(dotResult) - limitAngleDirtTexture) / limitAngleStoneTexture);
			}

			if ((index==223)||(IN.worldPos.y < _WaterHeightTransformed)) // water (either by tile index or by tile world position)
			{							
				c = getColorByTextureAtlasIndex(IN, _TileAtlasTexWoodland, 0, uvTex);
				#if defined(ENABLE_WATER_REFLECTIONS)
					if (_UseSeaReflectionTex)
					{
						float2 screenUV = IN.screenPos.xy / IN.screenPos.w;	
						c.rgb = 0.5f * c.rgb + 0.5f * tex2D(_SeaReflectionTex, screenUV).rgb;
					}
				#endif
				//discard;
			}
			else if ((index==224)||(index==225)||(index==229)) // desert
			{				
				c_g = getColorByTextureAtlasIndex(IN, _TileAtlasTexDesert, 8, uvTex);
				c_d = getColorByTextureAtlasIndex(IN, _TileAtlasTexDesert, 4, uvTex);
				c_s = getColorByTextureAtlasIndex(IN, _TileAtlasTexDesert, 12, uvTex);
				c = c_g * weightGrass + c_d * weightDirt + c_s * weightStone;
			}
			else if ((index==227)||(index==228)) // swamp
			{
				c_g = getColorByTextureAtlasIndex(IN, _TileAtlasTexSwamp, 8, uvTex);
				c_d = getColorByTextureAtlasIndex(IN, _TileAtlasTexSwamp, 4, uvTex);
				c_s = getColorByTextureAtlasIndex(IN, _TileAtlasTexSwamp, 12, uvTex);
				c = c_g * weightGrass + c_d * weightDirt + c_s * weightStone;
			}
			else if ((index==226)||(index==230)) // mountain
			{
				c_g = getColorByTextureAtlasIndex(IN, _TileAtlasTexMountain, 8, uvTex);
				c_d = getColorByTextureAtlasIndex(IN, _TileAtlasTexMountain, 4, uvTex);
				c_s = getColorByTextureAtlasIndex(IN, _TileAtlasTexMountain, 12, uvTex);
				c = c_g * weightGrass + c_d * weightDirt + c_s * weightStone;
			}
			else if ((index==231)||(index==232)||(index==233)) // woodland
			{
				c_g = getColorByTextureAtlasIndex(IN, _TileAtlasTexWoodland, 8, uvTex);
				c_d = getColorByTextureAtlasIndex(IN, _TileAtlasTexWoodland, 4, uvTex);
				c_s = getColorByTextureAtlasIndex(IN, _TileAtlasTexWoodland, 12, uvTex);
				c = c_g * weightGrass + c_d * weightDirt + c_s * weightStone;
			}
			else
			{
				c=half4(0.0f, 0.0f, 0.0f, 1.0f);
			}
			
			float treeCoverage = terrainTileInfo.g;

			uint locationRangeX = terrainTileInfo.b * _MaxIndex;
			uint locationRangeY = terrainTileInfo.a * _MaxIndex;
	
			half3 treeColor;

			// next line is the location placement I started with and which I understand...
			//  if ((locationXfract < (float)locationRangeX/8.0f) && (locationYfract < (float)locationRangeY / 8.0f)) // 8.0f is maximum location range (8 rmb blocks) - blocks are placed in the corner, so I played around with placement and came up with the following lines (no idea why it works but it does)

			// do not ask me what I am doing here, I simple don't know - just played around till it kind of fitted (locations are placed in the middle of its corresponding tile now...)
			float extraX = (2.0f - (float)locationRangeX) * (1.0f/64.0f); // 64? yeah don't ask
			float xDividor = (16.0f - (float)locationRangeX); // don't ask about the 16 as well :D

			float extraY = (2.0f - (float)locationRangeY) * (1.0f/64.0f);
			float yDividor = (16.0f - (float)locationRangeY);

			float locationXfract = (uvTex.x*(float)_FarTerrainTilemapDim)-0.5f - (float)mapPixelX;
			float locationYfract = (499 - uvTex.y*(float)_FarTerrainTilemapDim)-0.5f - (float)mapPixelY;

			if (_TextureSetSeasonCode == 0)
				treeColor = half3(0.125f, 0.165f, 0.061f);
			else if (_TextureSetSeasonCode == 1)
				treeColor = half3(0.96f, 0.98f, 0.94f);
			else if (_TextureSetSeasonCode == 2)
				treeColor = half3(0.10f, 0.14f, 0.04f);

			if ((abs(locationXfract - extraX) < (float)locationRangeX/xDividor) && (abs(locationYfract - extraY) < (float)locationRangeY/yDividor)) 
			{
			c.rgb = min(1.0f, 0.4f * c.rgb + 0.6f * ((1.0f - treeCoverage) * c.rgb + treeCoverage * treeColor));
			c.rgb = 1.0f * c.rgb;
			}
			else
			{		
			c.rgb = min(1.0f, 0.3f * c.rgb + 0.7f * ((1.0f - treeCoverage) * c.rgb + treeCoverage * treeColor));		
			c.rgb = 0.9f * c.rgb;
			}
			
			/*
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
			//o.Albedo = 0.5f * c2.rgb + 0.5f * half3(1.0f, 0.0f, 0.0f);
			//o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			*/

			o.Albedo = c.rgb; //0.5f * c.rgb + 0.5f * c2.rgb;
			//o.Albedo = tex2D(_FarTerrainTilemapTex, IN.uv_MainTex).rgba;
			//o.Albedo = tex2D(_FarTerrainTilemapTex, (IN.uv_MainTex)*0.5f).rgba;
		}

		ENDCG
	} 
	FallBack "Standard"
}
