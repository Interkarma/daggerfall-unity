//Increased Terrain Distance Mod for Daggerfall Tools For Unity
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: Michael Rauter (a.k.a. Nystul)
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)
//based on the DaggerfallTilemap shader by Gavin Clayton (interkarma@dfworkshop.net)

#ifndef FAR_TERRAIN_COMMON_CG_INCLUDED
#define FAR_TERRAIN_COMMON_CG_INCLUDED

struct Input
{
	float4  pos : SV_POSITION;
	float2 uv_MainTex;
	float2 uv_BumpMap;
	float3 worldPos; // interpolated vertex positions used for correct coast line texturing
	float3 worldNormal; // interpolated vertex normals used for texturing terrain based on terrain slope
	float4 screenPos;
	INTERNAL_DATA
};

#define PI 3.1416f

sampler2D _TileAtlasTex;
sampler2D _TilemapTex;
sampler2D _BumpMap;
int _TilesetDim;
int _TilemapDim;
int _MaxIndex;
float _AtlasSize;
float _GutterSize;

float _blendWeightFarTerrainTop;
float _blendWeightFarTerrainBottom;
float _blendWeightFarTerrainLeft;
float _blendWeightFarTerrainRight;

sampler2D _TileAtlasTexDesert;
sampler2D _TileAtlasTexWoodland;
sampler2D _TileAtlasTexMountain;
sampler2D _TileAtlasTexSwamp;
sampler2D _FarTerrainTilemapTex;		
int _FarTerrainTilesetDim; // used by FarTerrainTilemap shader, but not by TransitionRingTilemap shader
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
sampler2D _SkyTex;
float _BlendStart;
float _BlendEnd;
int _FogMode;
int _FogFromSkyTex;
float _FogDensity;
float _FogStartDistance;
float _FogEndDistance;

void fcolor (Input IN, SurfaceOutputStandard o, inout fixed4 color) //inout half4 outDiffuse : SV_Target0, inout half4 outSpecSmoothness : SV_Target1, inout half4 outNormal : SV_Target2, inout half4 outEmission : SV_Target3)
{
		//inout fixed4 color,
		//inout half4 outDiffuse : SV_Target0,            // RT0: diffuse color (rgb), occlusion (a)
        //inout half4 outSpecSmoothness : SV_Target1,    // RT1: spec color (rgb), smoothness (a)
        //inout half4 outNormal : SV_Target2,            // RT2: normal (rgb), --unused, very low precision-- (a)
        //inout half4 outEmission : SV_Target3            // RT3: emission (rgb), --unused-- (a))

	//half4 color = outDiffuse;

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

	//outDiffuse = half4(1.0f, 0.0f, 0.0f,0.5f); //color;
	o.Albedo = half3(1.0f, 0.0f, 0.0f);
}


half4 getColorByTextureAtlasIndex(Input IN, uniform sampler2D textureAtlas, int index, float2 uvTex, int texDim, int tilesetDim)
{			
	const float textureCrispness = 3.5f; // defines how crisp textures of extended terrain are (higher values result in more crispness)
	const float textureCrispnessDiminishingFactor = 0.075f; // defines how fast crispness of textures diminishes with more distance from the player (the camera)
	const float distanceAttenuation = 0.001; // used to attenuated computed distance

	float dist = max(abs(IN.worldPos.x - _WorldSpaceCameraPos.x), abs(IN.worldPos.z - _WorldSpaceCameraPos.z));
	dist = floor(dist*distanceAttenuation);

	int xpos = index % tilesetDim;
	int ypos = index / tilesetDim;
	float2 uv = float2(xpos, ypos) / tilesetDim;

	// Offset to fragment position inside tile
	float xoffset;
	float yoffset;
	// changed offset computation so that tile texture repeats over tile		
	xoffset = frac(uvTex.x * _FarTerrainTilemapDim * 1/(max(1,dist * textureCrispnessDiminishingFactor)) * textureCrispness) / _GutterSize;
	yoffset = frac(uvTex.y * _FarTerrainTilemapDim * 1/(max(1,dist * textureCrispnessDiminishingFactor)) * textureCrispness) / _GutterSize;
	 
	uv += float2(xoffset, yoffset) + _GutterSize / _AtlasSize;

	// Sample based on gradient and set output
	float2 uvr = uvTex * ((float)texDim / _GutterSize);
	half4 c = tex2Dgrad(textureAtlas, uv, ddx(uvr), ddy(uvr));
	return(c);
}

half4 getColorFromTerrain(Input IN, float2 uvTex, int texDim, int tilesetDim, int index)
{
	const float limitAngleDirtTexture = 12.5f * PI / 180.0f; // tile will get dirt texture assigned if angles definied by surface normal and up-vector is larger than this value (and not larger than limitAngleStoneTexture)
	const float limitAngleStoneTexture = 20.5f * PI / 180.0f; // tile will get stone texture assigned if angles definied by surface normal and up-vector is larger than this value

	half4 c;
	half4 c_g; // color value from grass texture
	half4 c_d; // color value from dirt texture
	half4 c_s; // color value from stone texture

	float weightGrass = 1.0f;
	float weightDirt = 0.0f;
	float weightStone = 0.0f;

	// there are several possibilities to get the tile surface normal...
	// float3 surfaceNormal = normalize(cross(ddx(IN.worldPos.xyz), ddy(IN.worldPos.xyz))); // approximate it from worldPosition with derivations			
	// float3 surfaceNormal = normalize(o.Normal); // interpolated vertex normal
	// float3 surfaceNormal = IN.worldNormal; // interpolated vertex normal (by input parameter)
	// float3 surfaceNormal = WorldNormalVector(IN, o.Normal); // don't know what the difference is (googled it - was mentioned that it does not get interpolated but i can't confirm this)
	// float3 surfaceNormal = 0.95f*(IN.worldNormal)+0.05f*normalize(cross(ddx(IN.worldPos.xyz), ddy(IN.worldPos.xyz))); // linear interpolation of interpolated vertex normal and approximated normal

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
		c = getColorByTextureAtlasIndex(IN, _TileAtlasTexWoodland, 0, uvTex, texDim, tilesetDim);
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
		c_g = getColorByTextureAtlasIndex(IN, _TileAtlasTexDesert, 8, uvTex, texDim, tilesetDim);
		c_d = getColorByTextureAtlasIndex(IN, _TileAtlasTexDesert, 4, uvTex, texDim, tilesetDim);
		c_s = getColorByTextureAtlasIndex(IN, _TileAtlasTexDesert, 12, uvTex, texDim, tilesetDim);
		c = c_g * weightGrass + c_d * weightDirt + c_s * weightStone;
	}
	else if ((index==227)||(index==228)) // swamp
	{
		c_g = getColorByTextureAtlasIndex(IN, _TileAtlasTexSwamp, 8, uvTex, texDim, tilesetDim);
		c_d = getColorByTextureAtlasIndex(IN, _TileAtlasTexSwamp, 4, uvTex, texDim, tilesetDim);
		c_s = getColorByTextureAtlasIndex(IN, _TileAtlasTexSwamp, 12, uvTex, texDim, tilesetDim);
		c = c_g * weightGrass + c_d * weightDirt + c_s * weightStone;
	}
	else if ((index==226)||(index==230)) // mountain
	{
		c_g = getColorByTextureAtlasIndex(IN, _TileAtlasTexMountain, 8, uvTex, texDim, tilesetDim);
		c_d = getColorByTextureAtlasIndex(IN, _TileAtlasTexMountain, 4, uvTex, texDim, tilesetDim);
		c_s = getColorByTextureAtlasIndex(IN, _TileAtlasTexMountain, 12, uvTex, texDim, tilesetDim);
		c = c_g * weightGrass + c_d * weightDirt + c_s * weightStone;
	}
	else if ((index==231)||(index==232)||(index==233)) // woodland
	{
		c_g = getColorByTextureAtlasIndex(IN, _TileAtlasTexWoodland, 8, uvTex, texDim, tilesetDim);
		c_d = getColorByTextureAtlasIndex(IN, _TileAtlasTexWoodland, 4, uvTex, texDim, tilesetDim);
		c_s = getColorByTextureAtlasIndex(IN, _TileAtlasTexWoodland, 12, uvTex, texDim, tilesetDim);
		c = c_g * weightGrass + c_d * weightDirt + c_s * weightStone;
	}
	else
	{
		c=half4(0.0f, 0.0f, 0.0f, 1.0f);
	}
	return c;
}

half3 updateColorWithInfoForTreeCoverageAndLocations(half3 colorIn, float treeCoverage, int locationRangeX, int locationRangeY, int mapPixelX, int mapPixelY, float2 uvTex)
{
	half3 c = colorIn;
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
	return c;
}

#endif // FAR_TERRAIN_COMMON_CG_INCLUDED