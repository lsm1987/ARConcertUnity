Shader "Custom/Visualizer_ARConcert"
{
	Properties
	{
		_Spectra ("Spectra", Vector) = (0, 0, 0, 0)

		_Center ("Center", Vector) = (0.0, 0.0, 0.0)
		_GridColor ("GridColor", Vector) = (0.2, 0.3, 0.5)
		_GridEmission ("GridEmission", Float) = 8.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf Lambert
		#pragma glsl
		#pragma target 3.0

		float4 _Spectra;
		float3 _Center;
		float4 _GridColor;
		float _GridEmission;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
			float4 screenPos;
		};

		float  _gl_mod(float  a, float  b) { return a - b * floor(a/b); }
		float2 _gl_mod(float2 a, float2 b) { return a - b * floor(a/b); }
		float3 _gl_mod(float3 a, float3 b) { return a - b * floor(a/b); }
		float4 _gl_mod(float4 a, float4 b) { return a - b * floor(a/b); }

		float Grid(float3 pos)
		{
			float grid_size = 0.4;
			float line_thickness = 0.015;

			float2 m = _gl_mod(abs(pos.xz*sign(pos.xz)), grid_size);
			float s = 0.0;
			if(m.x-line_thickness < 0.0 || m.y-line_thickness < 0.0) {
				return 1.0;
			}
			return 0.0;
		}

		float Circle(float3 pos)
		{
			float o_radius = 5.0;
			float i_radius = 4.0;
			float d = length(pos.xz);
			float c = max(o_radius-(o_radius-_gl_mod(d-_Time.y*1.5, o_radius))-i_radius, 0.0);
			return c;
		}

		float Hex( float2 p, float2 h )
		{
			float2 q = abs(p);
			return max(q.x-h.y,max(q.x+q.y*0.57735,q.y*1.1547)-h.x);
		}

		float HexGrid(float3 p)
		{
			float scale = 1.2;
			float2 grid = float2(0.692, 0.4) * scale;
			//float radius = 0.22 * scale;
			float radius = 0.21 * scale;

			float2 p1 = _gl_mod(p.xz, grid) - grid*0.5;
			float c1 = Hex(p1, radius);

			float2 p2 = _gl_mod(p.xz+grid*0.5, grid) - grid*0.5;
			float c2 = Hex(p2, radius);
			return min(c1, c2);
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			float2 coord = (IN.screenPos.xy / IN.screenPos.w);

			float3 center = IN.worldPos - _Center;
			float grid_d = HexGrid(center);
			float grid = grid_d > 0.0 ? 1.0 : 0.0;
			float circle = Circle(center);

			o.Albedo = 0.0;
			o.Alpha = (grid * circle);
			o.Emission = 0.0;
			//o.Albedo += _GridColor * grid * 0.1;
			o.Emission += _GridColor * (grid * circle) * _GridEmission;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
