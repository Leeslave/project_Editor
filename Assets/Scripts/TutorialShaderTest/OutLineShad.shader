Shader "Custom/OutLineShad"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Back ("Back", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _X("X",Range(0,1)) = 0.1
        _Y("Y",Range(0,1)) = 0.1
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
        float _X;
        float _Y;

        struct Input
        {
            float2 uv_MainTex;
        };

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float4 Tex = tex2D(_MainTex, IN.uv_MainTex);

            float distx = abs(IN.uv_MainTex.x - 0.5);
            float disty = abs(IN.uv_MainTex.y - 0.5);

            _X *= 0.5; _Y *= 0.5;


            if (distx > _X || disty > _Y)
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
