// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

Shader "Daggerfall/BillboardBatchFaded" {
	// Efficient transparent-cutout styled billboard batch.
	// NOTES:
	//  - Use this shader for best billboard performance.
	//  - Does not work with VertexLit path.
	Properties {
		_Color("Color", Color) = (1,1,1,1)
		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
		_MainTex("Albedo Map", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		_EmissionMap("Emission Map", 2D) = "white" {}
		_EmissionColor("Emission Color", Color) = (0,0,0)
		_UpVector ("Up Vector (XYZ)", Vector) = (0,1,0,0)

		_billboardFractionalXposInBlock("fractional x pos in block of billboard", Float) = 0.0
		_billboardFractionalYposInBlock("fractional y pos in block of billboard", Float) = 0.0

		_blendWeightFarTerrainTop("blend weight at top side of terrain block", Float) = 0.0
		_blendWeightFarTerrainBottom("blend weight at bottom side of terrain block", Float) = 0.0
		_blendWeightFarTerrainLeft("blend weight on left side of terrain block", Float) = 0.0
		_blendWeightFarTerrainRight("blend weight on right side of terrain block", Float) = 0.0
	}
	SubShader {
		Tags { "Queue" = "Overlay" "RenderType" = "Transparent" }
		LOD 200
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard vertex:vert addshadow alpha:fade keepalpha

		half4 _Color;
		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _EmissionMap;
		half4 _EmissionColor;
		float3 _UpVector;

		float _blendWeightFarTerrainTop;
		float _blendWeightFarTerrainBottom;
		float _blendWeightFarTerrainLeft;
		float _blendWeightFarTerrainRight;

		float _billboardFractionalXposInBlock;
		float _billboardFractionalYposInBlock;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float2 uv_EmissionMap;
			//float3 worldPos; // interpolated vertex positions used for correct coast line texturing
		};

		void vert (inout appdata_full v)
		{
			// Direction we are viewing the billboard from
			float3 viewDirection = UNITY_MATRIX_V._m02_m12_m22;
			float3 rightVector = normalize(cross(viewDirection, _UpVector));

			// Transform billboard normal for lighting support
			// Comment out this line to stop light changing as billboards rotate
			v.normal = mul((float3x3)UNITY_MATRIX_V, v.normal);

			// Offset vertices based on corners scaled by size
			v.vertex.xyz += rightVector * (v.tangent.z - 0.5) * v.tangent.x;
			v.vertex.xyz += _UpVector * (v.tangent.w - 0.5) * v.tangent.y;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			half4 albedo = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			half3 emission = tex2D(_EmissionMap, IN.uv_EmissionMap).rgb * _EmissionColor;
			albedo.r = 1.0f;
			o.Albedo = albedo.rgb - emission; // Emission cancels out other lights
			if (albedo.a < 0.5f)
			{
				discard;
			}
			else
			{
				//float dist = distance(IN.worldPos.xz, _WorldSpaceCameraPos.xz); 
				float w_x = lerp(_blendWeightFarTerrainLeft, _blendWeightFarTerrainRight, _billboardFractionalXposInBlock);
				float w_y = lerp(_blendWeightFarTerrainTop, _blendWeightFarTerrainBottom, _billboardFractionalYposInBlock);
				o.Alpha = w_x * w_y; // albedo.a;
			}
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.Emission = emission;
			o.Metallic = 0;
		}
		ENDCG
	} 
	FallBack "Standard"
}