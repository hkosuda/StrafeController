Shader "Custom/Chess"
{
	Properties
	{
		_Color1("Color1", Color) = (1,1,1,1)
		_Color2("Color2", Color) = (0,0,0,1)
		_SideColor("Side Color", Color) = (0,0,0,1)

		_SquareSize("SquareSize", Float) = 1.0
		_OffsetX ("Offset X", Float) = 0.0
		_OffsetZ ("Offset Z", Float) = 0.0
	}

		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard 
		#pragma target 3.0

		half4 _Color1;
		half4 _Color2;
		half4 _SideColor;

		half _SquareSize;
		half _OffsetX;
		half _OffsetZ;

		struct Input
		{
			half3 worldPos;
			half3 worldNormal;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			if (IN.worldNormal.y > 0.99)
			{
				uint row = round(abs(IN.worldPos.z - _OffsetZ) / _SquareSize);
				uint col = round(abs(IN.worldPos.x - _OffsetX) / _SquareSize);

				if ((col + row) % 2 == 1) {
					o.Albedo = _Color1;
				}

				else {
					o.Albedo = _Color2;
				}
			}
			
			else
			{
				o.Albedo = _SideColor;
			}
		}
	ENDCG
	}
		FallBack "Diffuse"
}
