// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

Shader "Daggerfall/BillboardBatch/TransparentCutout" {
	// Efficient transparent-cutout styled billboard batch.
	// NOTES:
	//  - Use this shader for best billboard performance.
	//  - Does not work with VertexLit path.
	//	- Receive Shadows causes self-shadowing artifacts due to rotating plane.
	//	- Receive Shadows is forced on in Deferred path, which forces on artifacts.
	//  - Consider using "Daggerfall/BillboardBatch/TransparentCutoutForceForward" with deferred.
	Properties {
		_MainTex ("Color (RGB) Alpha (A)", 2D) = "white" {}
		_UpVector ("Up Vector (XYZ)", Vector) = (0,1,0,0)
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
		_Color ("Main Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
		LOD 200
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Lambert alphatest:_Cutoff vertex:vert addshadow

		sampler2D _MainTex;
		float3 _UpVector;
		half4 _Color;

		struct Input {
			float2 uv_MainTex;
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

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Transparent/Cutout/Diffuse"
}