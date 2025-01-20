Shader "Unlit/CRT"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Distort ("Distortion",Float) = 0.1
        _Line("Line",Float) = 0.1
        _Thick("Thick",Float) = 0.1
        _Speed("Speed",Float) = 1
        _Flip("Flip",Float) = 1
        _FlipProb("FlipProb",Float) = 1
        _Rand("Rand",Float) = 1
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
            float _Thick;
            float _Speed;
            float _Flip;
            float _FlipProb;
            float _Rand;
            
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

            float RandomUV(float2 uv)
            {
                return frac(sin(_Time.x));
            }

            half4 frag(v2f i) : COLOR
            {
                float flip_up = step(_Rand,_FlipProb) * _Rand * 5;
                float flip_down = 1 - flip_up;
                i.uv.y -= ((1 - (i.uv.y + flip_up)) * step(i.uv.y, flip_up) + (1 - (i.uv.y - flip_down)) * step(flip_down, i.uv.y));

                float2 uv = i.uv - 0.5;
                float sub = 1.0 + _Distort * (uv.x * uv.x + uv.y * uv.y);
                uv *= float2(sub, sub);
                uv += 0.5;

                float4 color = tex2D(_MainTex,uv);
                float scanline = abs(sin((i.uv.y - 0.5 + frac(_Time.x * _Speed * 2)) * 1000 * _Thick));

                color.rgb *= lerp(_Line,1,scanline);

                return color;
            }
            ENDCG
        }
    }
}
