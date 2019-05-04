// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:
// 
// Notes:
//

Shader "Daggerfall/BillboardTextureArray" {
    Properties {
        _MainTexArray("Texture", 2DArray) = "" {}
        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        _EmissionMap("Emission", 2DArray) = "" {}
        _EmissionColor("Emission Color", Color) = (0,0,0)
    }
    SubShader {
        Tags { "RenderType" = "Transparent" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alphatest:_Cutoff vertex:vert
        #pragma target 3.5

        UNITY_DECLARE_TEX2DARRAY(_MainTexArray);
        UNITY_DECLARE_TEX2DARRAY(_EmissionMap);

        half4 _EmissionColor;

        struct Input {
            float2 uv_MainTexArray;
            float2 uv_EmissionMap;
            float layer;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.layer = v.texcoord.z;
        }

        void surf (Input IN, inout SurfaceOutputStandard o) {
            half4 albedo = UNITY_SAMPLE_TEX2DARRAY(_MainTexArray, float3(IN.uv_MainTexArray, IN.layer));
            o.Albedo = albedo.rgb;
            o.Alpha = albedo.a;

            half4 emission = UNITY_SAMPLE_TEX2DARRAY(_EmissionMap, float3(IN.uv_EmissionMap, IN.layer));
            o.Emission = emission.rgb * _EmissionColor;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
