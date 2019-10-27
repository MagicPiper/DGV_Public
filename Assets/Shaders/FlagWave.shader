// Upgrade NOTE: replaced 'glstate.matrix.mvp' with 'UNITY_MATRIX_MVP'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Original shader by cboe - Mar, 23, 2009
// Enhanced to 3 axis movement by Seon - Jan, 21, 2010
// Added _WaveSpeed - Jan, 26, 2010 Eric5h5
// Ajustement time function - April, 19, 2010 Fontmaster
//
// Requirements: assumes you are using a subdivided plane created with X (width) * Z (height) where Y is flat.
// Requirements: assumes UV as: left X (U0) is attatched to pole, and Top Z (V1) is at top of pole.
//
// Enjoy!
 
Shader "FX/FlagWave"
{
    Properties
    {
          _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Texture", 2D) = "white" { }
    _WaveSpeed ("Wave Speed", float) = 50.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
		CULL Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"			

			 float4 _Color;
      sampler2D _MainTex;
      float _WaveSpeed;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

           
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;

				 float sinOff=v.vertex.x+v.vertex.y+v.vertex.z;
        float t=_Time*_WaveSpeed;
        if(t < 0.0) t *= -1.0;
        float fx=v.uv.x;
        float fy=v.uv.x*v.uv.y;
 
        v.vertex.x+=sin(t*1.45+sinOff)*fx*0.06;
        v.vertex.y=sin(t*3.12+sinOff)*fx*0.02-fy*0.02;
        v.vertex.z-=sin(t*2.2+sinOff)*fx*0.04;   

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv)*UNITY_LIGHTMODEL_AMBIENT;;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}