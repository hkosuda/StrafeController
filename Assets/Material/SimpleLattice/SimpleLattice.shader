Shader "Custom/SimpleLattice"
{
    Properties
    {
        _LineColor ("LineColor", Color) = (1,1,1,1)
        _MainColor ("MainColor", Color) = (0,0,0,1)

		_LatticeSize("Lattice Size", float) = 10
		_LineWidth("Line Width", float) = 0.1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
		#pragma surface surf Standard 
		#pragma target 3.0

		half4 _LineColor;
		half4 _MainColor;

		half _LatticeSize;
		half _LineWidth;

		half delta(half pos, half size)
		{
			return pos - round(pos / size) * size;
		}

		struct Input
		{
			half3 worldPos;
			half3 worldNormal;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			half dx = abs(delta(IN.worldPos.x, _LatticeSize));
			half dy = abs(delta(IN.worldPos.y, _LatticeSize));
			half dz = abs(delta(IN.worldPos.z, _LatticeSize));

			if (abs(IN.worldNormal.x) > 0.99)
			{
				if (dy < _LineWidth || dz < _LineWidth)
				{
					o.Albedo = _LineColor;
				}

				else
				{
					o.Albedo = _MainColor;
				}
			}

			else if (abs(IN.worldNormal.z) > 0.99) 
			{
				if (dx < _LineWidth || dy < _LineWidth)
				{
					o.Albedo = _LineColor;
				}

				else
				{
					o.Albedo = _MainColor;
				}
			}

			else
			{
				if (dx < _LineWidth || dz < _LineWidth)
				{
					o.Albedo = _LineColor;
				}

				else
				{
					o.Albedo = _MainColor;
				}
			}
		}

	ENDCG
    }
    FallBack "Diffuse"
}
