// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2016 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity
// Contributors:     Nystul

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
        _TileTexArr("Tile Texture Array", 2DArray) = "white" {}
        _TileNormalMapTexArr("Tileset NormalMap Texture Array (RGBA)", 2DArray) = "bump" {}
        _TileParallaxMapTexArr("Tileset ParallaxMap Texture Array (R)", 2DArray) = "black" {}
        _Parallax("Parallax Scale", Range (0.005, 0.08)) = 0.01
        _TileMetallicGlossMapTexArr("Tileset MetallicGlossMap Texture Array (R)", 2DArray) = "black" {}
        _Smoothness("Smoothness", Range (0, 1)) = 0
        _TilemapTex("Tilemap (R)", 2D) = "red" {}
        _TilemapDim("Tilemap Dimension (in tiles)", Int) = 128
        _MaxIndex("Max Tileset Index", Int) = 255
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        CGPROGRAM
        #pragma target 3.5
        #pragma surface surf BlinnPhong
        #pragma glsl
        #pragma multi_compile_local __ _NORMALMAP
        #pragma multi_compile_local __ _PARALLAXMAP
        #pragma multi_compile_local __ _METALLICGLOSSMAP

        UNITY_DECLARE_TEX2DARRAY(_TileTexArr);
        #ifdef _NORMALMAP
            UNITY_DECLARE_TEX2DARRAY(_TileNormalMapTexArr);
        #endif
        #ifdef _PARALLAXMAP
            UNITY_DECLARE_TEX2DARRAY(_TileParallaxMapTexArr);
            float _Parallax;
        #endif
        #ifdef _METALLICGLOSSMAP
            UNITY_DECLARE_TEX2DARRAY(_TileMetallicGlossMapTexArr);
            float _Smoothness;
        #endif

        sampler2D _TilemapTex;
        float4 _TileTexArr_TexelSize;
        uint _MaxIndex;
        uint _TilemapDim;

        struct Input
        {
            float2 uv_MainTex;
            #ifdef _PARALLAXMAP
                float3 viewDir;
            #endif
        };

        // compute all 4 posible configurations of terrain tiles (normal, rotated, flipped, rotated and flipped)
        // rotations and translations not conflated together, because OpenGL ES 2.0 only supports square matrices
        static float2x2 rotations[4] = {
            float2x2(1.0, 0.0, 0.0, 1.0),
            float2x2(0.0, 1.0, -1.0, 0.0),
            float2x2(-1.0, 0.0, 0.0, -1.0),
            float2x2(0.0, -1.0, 1.0, 0.0)
        };
        static float2 translations[4] = {
            float2(0.0, 0.0),
            float2(0.0, 1.0),
            float2(1.0, 1.0),
            float2(1.0, 0.0)
        };

        #define MIPMAP_BIAS (-0.5)

        inline float GetMipLevel(float2 iUV, float4 iTextureSize)
        {
            float2 dx = ddx(iUV * iTextureSize.z);
            float2 dy = ddy(iUV * iTextureSize.w);
            float d = max(dot(dx, dx), dot(dy,dy));
            return 0.5 * log2(d) + MIPMAP_BIAS;
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            float2 unwrappedUV = IN.uv_MainTex * _TilemapDim;

            // Get offset to tile in atlas
            uint tileData = tex2D(_TilemapTex, floor(unwrappedUV) / _TilemapDim).a * _MaxIndex + 0.5;
            uint tileIndex = tileData >> 2; // compute correct texture array index from data
            uint tileTransformation = tileData & 0x3;

            // Offset to fragment position inside tile
            float2 tileUV = frac(unwrappedUV);
            float2 transformedTileUV = mul(rotations[tileTransformation], tileUV) + translations[tileTransformation];

            // Sample based on gradient and set output
            float3 uv3 = float3(transformedTileUV, tileIndex);

            // Get mipmap level
            float mipMapLevel = GetMipLevel(unwrappedUV, _TileTexArr_TexelSize);

            // Get parallax offset
            #ifdef _PARALLAXMAP
                half height = UNITY_SAMPLE_TEX2DARRAY_SAMPLER_LOD(_TileParallaxMapTexArr, _TileParallaxMapTexArr, uv3, mipMapLevel).r;
                uv3.xy += ParallaxOffset(height, _Parallax, IN.viewDir);
            #endif

            // Albedo (colour) map
            half4 albedo = UNITY_SAMPLE_TEX2DARRAY_SAMPLER_LOD(_TileTexArr, _TileTexArr, uv3, mipMapLevel);
            o.Albedo = albedo.rgb;
            o.Alpha = albedo.a;

            // Normal map
            #ifdef _NORMALMAP
                o.Normal = UnpackNormal(UNITY_SAMPLE_TEX2DARRAY_SAMPLER_LOD(_TileNormalMapTexArr, _TileNormalMapTexArr, uv3, mipMapLevel));
            #endif

            // Very rough approximation of metallic map using gloss and specular
            #ifdef _METALLICGLOSSMAP
                half4 metallicMap = UNITY_SAMPLE_TEX2DARRAY_SAMPLER_LOD(_TileMetallicGlossMapTexArr, _TileMetallicGlossMapTexArr, uv3, mipMapLevel);
                o.Gloss = 1 - metallicMap.r;
                o.Specular = _Smoothness;
            #endif
        }
        ENDCG
    }
    FallBack "Diffuse"
}
