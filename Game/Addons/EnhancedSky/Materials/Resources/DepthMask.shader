Shader "Transparent/DepthMask" 
{
	Properties 
	{
	}

	Category 
	{
		SubShader 
		{
         Tags {"Queue" = "Transparent-1" }       
         Lighting Off
         ZTest LEqual
         ZWrite On
         ColorMask 0
         Pass {}
		}
	
	}
}