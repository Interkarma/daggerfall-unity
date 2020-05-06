Shader "Daggerfall/RetroPalettization"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
       Lighting Off 
		Cull Off ZWrite Off ZTest Always
       Fog { Mode Off } 

		Pass
		{
			CGPROGRAM
           #pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
            
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;

// Beginning of generated code
// See https://github.com/petchema/ocaml-palettisation-shader-generator

// ART_PAL+NIGHTSKY_blues.txt - 287 unique colors - max color cluster size 37
struct ColorMatch
{
  fixed4 color;
  fixed minDistSqr;
};

fixed4 targetColor;

fixed disSqr(fixed4 t, fixed4 c)
{
  return dot(t - c, t - c);
}

ColorMatch buildColorMatch(fixed4 color)
{
  ColorMatch match;
  match.color = color;
  match.minDistSqr = disSqr(targetColor, color);
  return match;
}

void fillColorMatch(fixed4 color, out ColorMatch match)
{
  match.color = color;
  match.minDistSqr = disSqr(targetColor, color);
}

ColorMatch findColorLLL() // 36 colors in (0,0,0)-(255,34,67)
{
  ColorMatch best = buildColorMatch(fixed4(111.0/255,34.0/255,0.0/255,1.0));
  fixed4 color;
  fixed distSqr;
  color = fixed4(92.0/255,33.0/255,3.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(102.0/255,33.0/255,1.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(83.0/255,32.0/255,10.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(31.0/255,31.0/255,31.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(47.0/255,31.0/255,15.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(27.0/255,27.0/255,67.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(27.0/255,27.0/255,64.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(27.0/255,27.0/255,27.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(26.0/255,26.0/255,60.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(26.0/255,26.0/255,56.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(25.0/255,25.0/255,53.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(25.0/255,25.0/255,49.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(24.0/255,24.0/255,46.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(154.0/255,24.0/255,8.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(23.0/255,23.0/255,42.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(23.0/255,23.0/255,38.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(23.0/255,23.0/255,23.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(35.0/255,23.0/255,11.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(22.0/255,22.0/255,35.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(22.0/255,22.0/255,31.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(130.0/255,22.0/255,0.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(21.0/255,21.0/255,27.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(21.0/255,21.0/255,24.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(20.0/255,20.0/255,20.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(19.0/255,19.0/255,19.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(15.0/255,15.0/255,15.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(147.0/255,12.0/255,4.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(0.0/255,0.0/255,67.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(0.0/255,0.0/255,62.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(0.0/255,0.0/255,57.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(0.0/255,0.0/255,53.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(0.0/255,0.0/255,48.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(0.0/255,0.0/255,44.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(0.0/255,0.0/255,39.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(0.0/255,0.0/255,0.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  return best;
}

ColorMatch findColorLLR() // 36 colors in (0,35,0)-(255,59,67)
{
  ColorMatch best = buildColorMatch(fixed4(35.0/255,35.0/255,35.0/255,1.0));
  fixed4 color;
  fixed distSqr;
  color = fixed4(162.0/255,36.0/255,12.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(38.0/255,38.0/255,38.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(59.0/255,39.0/255,19.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(74.0/255,39.0/255,27.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(40.0/255,40.0/255,40.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(65.0/255,41.0/255,33.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(57.0/255,43.0/255,39.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(44.0/255,44.0/255,44.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(44.0/255,44.0/255,45.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(46.0/255,44.0/255,46.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(50.0/255,45.0/255,34.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(56.0/255,45.0/255,52.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(43.0/255,46.0/255,45.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(75.0/255,47.0/255,23.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(40.0/255,47.0/255,40.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(63.0/255,47.0/255,56.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(169.0/255,48.0/255,15.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(45.0/255,48.0/255,48.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(44.0/255,48.0/255,49.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(48.0/255,48.0/255,50.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(54.0/255,50.0/255,40.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(58.0/255,51.0/255,25.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(34.0/255,51.0/255,34.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(38.0/255,51.0/255,40.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(51.0/255,51.0/255,51.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(67.0/255,51.0/255,63.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(53.0/255,53.0/255,59.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(61.0/255,54.0/255,38.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(66.0/255,54.0/255,41.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(87.0/255,55.0/255,27.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(30.0/255,55.0/255,30.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(50.0/255,55.0/255,55.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(43.0/255,56.0/255,39.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(58.0/255,58.0/255,58.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(47.0/255,59.0/255,60.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  return best;
}

ColorMatch findColorLL() // 72 colors in (0,0,0)-(255,59,67)
{
  fixed diff = targetColor.g - 35.0/255;
  if (diff >= 0)
  {
    ColorMatch best = findColorLLR();
    if (best.minDistSqr <= diff * diff) return best;
    ColorMatch otherBest = findColorLLL();
    if (otherBest.minDistSqr >= best.minDistSqr) return best; else return otherBest;
  }
  else
  {
    ColorMatch best = findColorLLL();
    if (best.minDistSqr <= diff * diff) return best;
    ColorMatch otherBest = findColorLLR();
    if (otherBest.minDistSqr >= best.minDistSqr) return best; else return otherBest;
  }
}

ColorMatch findColorLRL() // 36 colors in (0,60,0)-(255,88,67)
{
  ColorMatch best = buildColorMatch(fixed4(104.0/255,87.0/255,11.0/255,1.0));
  fixed4 color;
  fixed distSqr;
  color = fixed4(53.0/255,87.0/255,34.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(143.0/255,87.0/255,51.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(175.0/255,87.0/255,67.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(140.0/255,86.0/255,55.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(109.0/255,85.0/255,54.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(190.0/255,84.0/255,27.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(139.0/255,83.0/255,43.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(129.0/255,79.0/255,48.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(163.0/255,79.0/255,59.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(93.0/255,78.0/255,14.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(52.0/255,77.0/255,45.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(96.0/255,76.0/255,51.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(51.0/255,75.0/255,35.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(127.0/255,75.0/255,39.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(122.0/255,75.0/255,43.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(155.0/255,75.0/255,51.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(183.0/255,72.0/255,23.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(83.0/255,71.0/255,44.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(147.0/255,71.0/255,47.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(112.0/255,70.0/255,40.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(81.0/255,69.0/255,18.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(46.0/255,68.0/255,37.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(115.0/255,67.0/255,35.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(91.0/255,67.0/255,38.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(67.0/255,67.0/255,67.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(45.0/255,64.0/255,37.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(103.0/255,64.0/255,39.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(99.0/255,63.0/255,31.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(69.0/255,63.0/255,42.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(79.0/255,63.0/255,43.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(47.0/255,63.0/255,63.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(61.0/255,61.0/255,67.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(176.0/255,60.0/255,19.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(69.0/255,60.0/255,21.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(39.0/255,60.0/255,39.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  return best;
}

ColorMatch findColorLRR() // 36 colors in (0,89,0)-(255,255,67)
{
  ColorMatch best = buildColorMatch(fixed4(61.0/255,89.0/255,53.0/255,1.0));
  fixed4 color;
  fixed distSqr;
  color = fixed4(151.0/255,91.0/255,55.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(155.0/255,91.0/255,47.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(123.0/255,92.0/255,60.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(152.0/255,93.0/255,63.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(155.0/255,95.0/255,59.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(198.0/255,95.0/255,31.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(116.0/255,97.0/255,7.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(65.0/255,98.0/255,37.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(68.0/255,99.0/255,67.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(159.0/255,99.0/255,63.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(167.0/255,103.0/255,67.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(134.0/255,103.0/255,65.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(127.0/255,106.0/255,4.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(205.0/255,107.0/255,35.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(76.0/255,108.0/255,42.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(137.0/255,112.0/255,66.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(139.0/255,115.0/255,0.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(84.0/255,118.0/255,48.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(212.0/255,119.0/255,39.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(92.0/255,127.0/255,54.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(164.0/255,130.0/255,67.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(219.0/255,131.0/255,43.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(154.0/255,133.0/255,0.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(101.0/255,137.0/255,60.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(226.0/255,143.0/255,46.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(109.0/255,146.0/255,66.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(168.0/255,150.0/255,0.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(234.0/255,155.0/255,50.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(241.0/255,167.0/255,54.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(183.0/255,168.0/255,0.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(197.0/255,185.0/255,0.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(212.0/255,203.0/255,0.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(226.0/255,220.0/255,0.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(241.0/255,238.0/255,45.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(0.0/255,255.0/255,0.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  return best;
}

ColorMatch findColorLR() // 72 colors in (0,60,0)-(255,255,67)
{
  fixed diff = targetColor.g - 89.0/255;
  if (diff >= 0)
  {
    ColorMatch best = findColorLRR();
    if (best.minDistSqr <= diff * diff) return best;
    ColorMatch otherBest = findColorLRL();
    if (otherBest.minDistSqr >= best.minDistSqr) return best; else return otherBest;
  }
  else
  {
    ColorMatch best = findColorLRL();
    if (best.minDistSqr <= diff * diff) return best;
    ColorMatch otherBest = findColorLRR();
    if (otherBest.minDistSqr >= best.minDistSqr) return best; else return otherBest;
  }
}

ColorMatch findColorL() // 144 colors in (0,0,0)-(255,255,67)
{
  fixed diff = targetColor.g - 60.0/255;
  if (diff >= 0)
  {
    ColorMatch best = findColorLR();
    if (best.minDistSqr <= diff * diff) return best;
    ColorMatch otherBest = findColorLL();
    if (otherBest.minDistSqr >= best.minDistSqr) return best; else return otherBest;
  }
  else
  {
    ColorMatch best = findColorLL();
    if (best.minDistSqr <= diff * diff) return best;
    ColorMatch otherBest = findColorLR();
    if (otherBest.minDistSqr >= best.minDistSqr) return best; else return otherBest;
  }
}

ColorMatch findColorRLL() // 36 colors in (0,0,68)-(90,141,255)
{
  ColorMatch best = buildColorMatch(fixed4(87.0/255,87.0/255,87.0/255,1.0));
  fixed4 color;
  fixed distSqr;
  color = fixed4(87.0/255,137.0/255,205.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(86.0/255,58.0/255,77.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(85.0/255,85.0/255,96.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(82.0/255,116.0/255,86.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(78.0/255,78.0/255,78.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(77.0/255,110.0/255,78.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(75.0/255,52.0/255,71.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(75.0/255,75.0/255,85.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(70.0/255,135.0/255,135.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(68.0/255,68.0/255,80.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(68.0/255,112.0/255,179.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(68.0/255,124.0/255,192.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(62.0/255,105.0/255,167.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(62.0/255,124.0/255,124.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(55.0/255,97.0/255,154.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(54.0/255,112.0/255,112.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(52.0/255,69.0/255,87.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(51.0/255,77.0/255,102.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(50.0/255,62.0/255,73.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(49.0/255,90.0/255,142.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(46.0/255,103.0/255,103.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(45.0/255,72.0/255,72.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(45.0/255,82.0/255,122.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(40.0/255,83.0/255,83.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(39.0/255,91.0/255,91.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(28.0/255,28.0/255,71.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(0.0/255,0.0/255,71.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(0.0/255,0.0/255,76.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(0.0/255,0.0/255,80.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(0.0/255,0.0/255,85.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(0.0/255,0.0/255,90.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(0.0/255,0.0/255,94.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(0.0/255,0.0/255,99.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(0.0/255,0.0/255,103.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(0.0/255,0.0/255,108.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  return best;
}

ColorMatch findColorRLR() // 36 colors in (91,0,68)-(255,141,255)
{
  ColorMatch best = buildColorMatch(fixed4(93.0/255,130.0/255,94.0/255,1.0));
  fixed4 color;
  fixed distSqr;
  color = fixed4(94.0/255,94.0/255,109.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(99.0/255,99.0/255,99.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(101.0/255,65.0/255,96.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(103.0/255,103.0/255,116.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(109.0/255,69.0/255,102.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(110.0/255,110.0/255,110.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(112.0/255,94.0/255,72.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(114.0/255,114.0/255,127.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(118.0/255,105.0/255,93.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(119.0/255,119.0/255,119.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(122.0/255,122.0/255,137.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(127.0/255,77.0/255,106.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(132.0/255,132.0/255,132.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(132.0/255,119.0/255,107.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(132.0/255,114.0/255,82.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(135.0/255,135.0/255,149.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(137.0/255,121.0/255,94.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(140.0/255,129.0/255,119.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(143.0/255,84.0/255,119.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(151.0/255,110.0/255,69.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(155.0/255,98.0/255,130.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(160.0/255,118.0/255,74.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(164.0/255,141.0/255,94.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(165.0/255,100.0/255,70.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(171.0/255,107.0/255,71.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(173.0/255,127.0/255,78.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(175.0/255,111.0/255,144.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(175.0/255,111.0/255,75.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(175.0/255,95.0/255,75.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(179.0/255,115.0/255,79.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(179.0/255,107.0/255,83.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(180.0/255,113.0/255,80.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(183.0/255,140.0/255,88.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(188.0/255,127.0/255,158.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(193.0/255,133.0/255,100.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  return best;
}

ColorMatch findColorRL() // 72 colors in (0,0,68)-(255,141,255)
{
  fixed diff = targetColor.r - 91.0/255;
  if (diff >= 0)
  {
    ColorMatch best = findColorRLR();
    if (best.minDistSqr <= diff * diff) return best;
    ColorMatch otherBest = findColorRLL();
    if (otherBest.minDistSqr >= best.minDistSqr) return best; else return otherBest;
  }
  else
  {
    ColorMatch best = findColorRLL();
    if (best.minDistSqr <= diff * diff) return best;
    ColorMatch otherBest = findColorRLR();
    if (otherBest.minDistSqr >= best.minDistSqr) return best; else return otherBest;
  }
}

ColorMatch findColorRRL() // 35 colors in (0,142,68)-(255,255,142)
{
  ColorMatch best = buildColorMatch(fixed4(77.0/255,142.0/255,142.0/255,1.0));
  fixed4 color;
  fixed distSqr;
  color = fixed4(255.0/255,215.0/255,141.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(148.0/255,176.0/255,141.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(208.0/255,185.0/255,134.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(255.0/255,229.0/255,129.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(222.0/255,198.0/255,128.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(213.0/255,174.0/255,128.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(185.0/255,205.0/255,127.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(255.0/255,205.0/255,127.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(161.0/255,147.0/255,125.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(231.0/255,206.0/255,123.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(231.0/255,198.0/255,122.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(179.0/255,160.0/255,121.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(123.0/255,156.0/255,118.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(207.0/255,152.0/255,118.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(247.0/255,206.0/255,115.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(239.0/255,206.0/255,115.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(255.0/255,195.0/255,112.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(107.0/255,144.0/255,109.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(255.0/255,206.0/255,107.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(247.0/255,206.0/255,107.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(196.0/255,154.0/255,105.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(255.0/255,246.0/255,103.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(159.0/255,183.0/255,101.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(255.0/255,206.0/255,99.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(255.0/255,198.0/255,99.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(255.0/255,185.0/255,98.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(255.0/255,206.0/255,90.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(255.0/255,197.0/255,86.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(255.0/255,175.0/255,83.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(228.0/255,178.0/255,80.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(247.0/255,189.0/255,79.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(130.0/255,162.0/255,77.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(185.0/255,148.0/255,76.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(206.0/255,159.0/255,73.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  return best;
}

ColorMatch findColorRRR() // 36 colors in (0,142,143)-(255,255,255)
{
  ColorMatch best = buildColorMatch(fixed4(227.0/255,180.0/255,144.0/255,1.0));
  fixed4 color;
  fixed distSqr;
  color = fixed4(147.0/255,147.0/255,147.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(186.0/255,174.0/255,147.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(176.0/255,164.0/255,148.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(229.0/255,193.0/255,150.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(87.0/255,154.0/255,154.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(165.0/255,156.0/255,156.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(254.0/255,225.0/255,156.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(145.0/255,145.0/255,159.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(162.0/255,162.0/255,162.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(216.0/255,227.0/255,162.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(244.0/255,202.0/255,167.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(175.0/255,200.0/255,168.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(204.0/255,146.0/255,170.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(109.0/255,170.0/255,170.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(254.0/255,235.0/255,170.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(245.0/255,212.0/255,172.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(165.0/255,165.0/255,174.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(174.0/255,174.0/255,174.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(185.0/255,185.0/255,185.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(254.0/255,245.0/255,185.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(134.0/255,187.0/255,187.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(220.0/255,166.0/255,188.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(202.0/255,221.0/255,196.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(197.0/255,197.0/255,197.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(188.0/255,188.0/255,199.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(254.0/255,255.0/255,199.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(232.0/255,188.0/255,200.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(158.0/255,202.0/255,202.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(104.0/255,152.0/255,217.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(220.0/255,220.0/255,220.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(205.0/255,205.0/255,224.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(182.0/255,218.0/255,227.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(123.0/255,164.0/255,230.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(147.0/255,185.0/255,244.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  color = fixed4(176.0/255,205.0/255,255.0/255,1.0);
  distSqr = disSqr(targetColor, color);
  if (distSqr < best.minDistSqr) { best.color = color; best.minDistSqr = distSqr; }
  return best;
}

ColorMatch findColorRR() // 71 colors in (0,142,68)-(255,255,255)
{
  fixed diff = targetColor.b - 143.0/255;
  if (diff >= 0)
  {
    ColorMatch best = findColorRRR();
    if (best.minDistSqr <= diff * diff) return best;
    ColorMatch otherBest = findColorRRL();
    if (otherBest.minDistSqr >= best.minDistSqr) return best; else return otherBest;
  }
  else
  {
    ColorMatch best = findColorRRL();
    if (best.minDistSqr <= diff * diff) return best;
    ColorMatch otherBest = findColorRRR();
    if (otherBest.minDistSqr >= best.minDistSqr) return best; else return otherBest;
  }
}

ColorMatch findColorR() // 143 colors in (0,0,68)-(255,255,255)
{
  fixed diff = targetColor.g - 142.0/255;
  if (diff >= 0)
  {
    ColorMatch best = findColorRR();
    if (best.minDistSqr <= diff * diff) return best;
    ColorMatch otherBest = findColorRL();
    if (otherBest.minDistSqr >= best.minDistSqr) return best; else return otherBest;
  }
  else
  {
    ColorMatch best = findColorRL();
    if (best.minDistSqr <= diff * diff) return best;
    ColorMatch otherBest = findColorRR();
    if (otherBest.minDistSqr >= best.minDistSqr) return best; else return otherBest;
  }
}

ColorMatch findColor() // 287 colors in (0,0,0)-(255,255,255)
{
  fixed diff = targetColor.b - 68.0/255;
  if (diff >= 0)
  {
    ColorMatch best = findColorR();
    if (best.minDistSqr <= diff * diff) return best;
    ColorMatch otherBest = findColorL();
    if (otherBest.minDistSqr >= best.minDistSqr) return best; else return otherBest;
  }
  else
  {
    ColorMatch best = findColorL();
    if (best.minDistSqr <= diff * diff) return best;
    ColorMatch otherBest = findColorR();
    if (otherBest.minDistSqr >= best.minDistSqr) return best; else return otherBest;
  }
}

fixed4 nearestColor(fixed4 color)
{
  targetColor = color;
  ColorMatch best = findColor();
  return best.color;
}

// End of generated code

	        fixed4 frag (v2f i) : SV_Target
	        {
                fixed4 target = tex2D(_MainTex, i.uv);
                
                // ART_PAL.COL
                return nearestColor(target);
            }
			ENDCG
		}
	}
}
