Shader "Custom/Chess"
{
	Properties
	{
		_Color1("Color1", Color) = (1,1,1,1)
		_Color2("Color2", Color) = (0,0,0,1)
		_SquareSize("SquareSize", Float) = 1.0
	}

		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard 
		#pragma target 3.0

		fixed4 _Color1;
		fixed4 _Color2;
		half _SquareSize;

		struct Input
		{
			float3 worldPos;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			uint row = round(abs(IN.worldPos.z / _SquareSize));
			uint col = round(abs(IN.worldPos.x / _SquareSize));

			if ((col + row) % 2 == 1) {
				o.Albedo = _Color1;
			}

			else {
				o.Albedo = _Color2;
			}
		}
	ENDCG
	}
		FallBack "Diffuse"
}
