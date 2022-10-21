Shader "CylinderImageEffect" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
    }

        SubShader{
            Cull Off ZWrite Off ZTest Always

            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct v2f {
                    float4 vertex : SV_POSITION;
                    float2 uv : TEXCOORD0;
                };

                v2f vert(float4 p : POSITION, float2 t : TEXCOORD0) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(p);
                    o.uv = t;
                    return o;
                }

                sampler2D _MainTex;

                fixed4 frag(v2f i) : SV_Target {
                    i.uv.x = 1 - acos(i.uv.x * 2 - 1) / 3.14159265;
                    return tex2D(_MainTex, i.uv);
                }
                ENDCG
            }
    }
}