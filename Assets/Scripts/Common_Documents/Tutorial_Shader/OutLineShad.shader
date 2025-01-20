Shader "Custom/OutLineShad"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Back("Back", Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {}
        _Ratio("Ratio", Float) = 1
    }
        SubShader
        {
            Tags { "Queue" = "Overlay" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
            LOD 200
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma surface surf Standard alpha:blend
            #pragma target 3.0

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _Back;
            float _Ratio;

            struct Input
            {
                float2 uv_MainTex;
            };

            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_INSTANCING_BUFFER_END(Props)

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                float distx = abs(IN.uv_MainTex.x - 0.5);
                float disty = abs(IN.uv_MainTex.y - 0.5);


                if (distx > 0.488 || disty > (0.5 - 0.012 * _Ratio))
                {
                    o.Emission = _Color;
                    o.Alpha = _Color.a;
                }

                else
                {
                    o.Emission = _Back;
                    o.Alpha = _Back.a;
                }

            }
            ENDCG
        }
            FallBack "Diffuse"
}
