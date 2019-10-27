Shader "Mobile/DiscShader"
{
	Properties
	{
		_Color("Color",COLOR) = (0.5,0.5,0.5,1.0)
		_PatternColor("PatternColor",COLOR) = (0.5,0.5,0.5,1.0)
		_StampColor("StampColor",COLOR) = (0.5,0.5,0.5,1.0)
		_Pattern("Pattern (RGB)", 2D) = "white" {}		
		_Stamp("Stamp (RGB)", 2D) = "white" {}
	}

		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 150
		CGPROGRAM
#pragma surface surf Lambert noforwardadd

		sampler2D _Pattern;
		sampler2D _Stamp;
	    fixed4 _Color;
		fixed4 _PatternColor;
		fixed4 _StampColor;		

	struct Input
	{
		float2 uv_Pattern;
		float2 uv_Stamp;
	};

	void surf(Input IN, inout SurfaceOutput o)
	{
		fixed4 c = tex2D(_Pattern, IN.uv_Pattern) * _PatternColor;
		fixed4 d = _Color;

		fixed4 s = tex2D(_Stamp, IN.uv_Stamp) * _StampColor;
		
		o.Albedo = lerp(lerp(d.rgb, c.rgb, c.a), s.rgb, s.a);		
	}
	ENDCG
	}
		Fallback "Mobile/VertexLit"
}