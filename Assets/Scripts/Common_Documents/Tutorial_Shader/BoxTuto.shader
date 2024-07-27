Shader "Custom/BoxTuto"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _MaskCenter("MaskCenter", Vector) = (0,0,0,0)
        _X("X",Range(0,1)) = 0.1
        _Y("Y",Range(0,1)) = 0.1
        _SideRadius ("SideRadius",Range(0,1)) = 0.2
        _BlackAlpha ("BlackAlpha",Range(0,1)) = 0.4
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
        float4 _MaskCenter;
        float _X;
        float _Y;
        float _SideRadius;
        float _BlackAlpha;

        struct Input
        {
            float2 uv_MainTex;
        };

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float4 Tex = tex2D(_MainTex, IN.uv_MainTex);

            float distx = abs(IN.uv_MainTex.x - _MaskCenter.x);
            float disty = abs(IN.uv_MainTex.y - _MaskCenter.y);

            _X *= 0.5; _Y *= 0.5; _SideRadius *= 0.5;

            if (distx < _X && disty < _Y) discard;
            if (distx > _X + _SideRadius || disty > _Y + _SideRadius)
            {
                o.Emission = float3(0,0,0);
                o.Alpha = _BlackAlpha;
            }
            else{
                o.Emission = _Color;
                o.Alpha = 1;
            }
        }
        ENDCG
    }
    FallBack "Diffuse"
}
