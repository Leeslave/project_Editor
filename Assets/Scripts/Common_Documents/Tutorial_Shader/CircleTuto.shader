Shader "Custom/CircleTuto"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _BlackAlpha ("BlackAlpha",Range(0,1)) = 0.4
        _MainTex ("Texture", 2D) = "white" {}
        _MaskCenter ("MaskCenter", Vector) = (0,0,0,0)
        _Radius ("Radius", Range(0,1)) = 0.5
        _SideRatio ("SideRatio", Range(0,1)) = 0.2
        _BloomRadius ("BloomRadius", Range(0,1)) = 0.1
        _BlackAlpha("BlackAlpha",Range(0,1)) = 0.4
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
        float _BlackAlpha;
        float4 _MaskCenter;
        float _BloomRadius;
        float _SideRatio;
        float _Radius;

        struct Input
        {
            float2 uv_MainTex;
        };

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float4 Tex = tex2D(_MainTex, IN.uv_MainTex);

            float dist = distance(IN.uv_MainTex, _MaskCenter.xy) * 2;
            // 빈 공간 표시
            if (step(dist , _Radius)) discard;
            // 검은 공간 표시
            if (step(_Radius + _BloomRadius,dist))
            {
                o.Emission = float3(0,0,0);
                o.Alpha = _BlackAlpha;
                return;
            }

            
            dist = (dist - _Radius) / _BloomRadius;
            // 테두리 표시
            if(dist < _SideRatio)
            {
                o.Emission = _Color;
                o.Alpha = 1;
            }
            // Bloom 표시
            else
            {
                o.Emission = lerp(_Color, float3(0, 0, 0), dist);
                o.Alpha = lerp(1,_BlackAlpha,dist);
            }
            //붕괴처럼 테두리 추가하는거?
            //if (0.3 <= dist && dist <= 0.32) o.Emission += float3(0.15,0.15,0.15);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
