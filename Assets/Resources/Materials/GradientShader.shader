Shader "Diffuse With Gradient Color" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" { }
		_Color("Main Color", Color) = (1, 1, 1, 1)
			_Color2("Main Color 2", Color) = (1, 1, 1, 1)
	}
	SubShader{
			Tags{ "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
#pragma surface surf Lambert

			sampler2D _MainTex;
			float4 _Color;
			float4 _Color2;

			struct Input {
				float2 uv_MainTex;
			};

			void surf(Input IN, inout SurfaceOutput o)
			{
				half4 main_color = lerp(_Color, _Color2, IN.uv_MainTex.x);
					half4 c = tex2D(_MainTex, IN.uv_MainTex) * main_color;
					o.Albedo = c.rgb;
				o.Alpha = c.a;
			}
			ENDCG
		}
		FallBack "Diffuse"
}