Shader "Custom/Tile"
{
    Properties
	{
		_PositionX("PositionX", Float) = 0
		_PositionZ("PositionZ", Float) = 0
		_LineColor("LineColor", Color) = (1,1,1,1)
		_TileColor("TileColor", Color) = (0,0,0,1)
		_LineWidth("LineWidth", Float) = 0.1
		_TileSize("TileSize", Float) = 1
	}

		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard 
		#pragma target 3.0

		half _PositionX;
		half _PositionZ;

		fixed4 _LineColor;
		fixed4 _TileColor;

		half _LineWidth;
		half _TileSize;

		struct Input
		{
			float3 worldPos;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			half border = _TileSize * 0.5 - _LineWidth;

			half dz = IN.worldPos.z - _PositionZ;
			half dx = IN.worldPos.x - _PositionX;

			if (abs(dz) > border || abs(dx) > border){
				o.Albedo = _LineColor;
				o.Emission = _LineColor * 1;
			}

			else{
				o.Albedo = _TileColor;
				o.Emission = _TileColor * 1;
			}
		}
	ENDCG
	}
	FallBack "Diffuse"
}
