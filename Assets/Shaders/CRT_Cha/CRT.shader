Shader "Unlit/CRT"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Distort ("Distortion",Float) = 0.1
        _Line("Line",Float) = 0.1
    }
SubShader{
    Pass {
            ZTest Always Cull Off ZWrite Off
            Fog { Mode off }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest 

            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            float _Distort;
            float _Line;

            struct v2f {
			    float4 pos : POSITION;
			    float2 uv : TEXCOORD0;
		    };
		
		    v2f vert( appdata_img v )
		    {
			    v2f o;
			    o.pos = UnityObjectToClipPos (v.vertex);
			    o.uv = v.texcoord.xy;
    
			    return o;
		    }

            half4 frag (v2f i) : COLOR
            {
                float2 uv = i.uv - 0.5;
                float sub = 1.0 + _Distort * (uv.x * uv.x + uv.y * uv.y);
                uv *= float2(sub, sub);
                uv += 0.5;

                float4 color = tex2D(_MainTex,uv);
                float scanline = sin((i.uv.y + _SinTime.w)* _ScreenParams.y * 200.0) * 0.5 + 0.5;
                
                color.rgb *= lerp(1,scanline,_Line);

                return color;
            }
            ENDCG
        }
    }
}
