Shader "UI/TutorialDimShader"
{
    Properties
    {
        // UI Sprite 기본 텍스처(안 써도 되지만 UI Image 요구사항 때문에 둠)
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        // 딤 컬러(일반적으로 검정) + 알파는 _DimAlpha로 제어
        _Color ("Tint", Color) = (0,0,0,1)

        // 구멍 중심 (0~1, Screen UV)
        _Center ("Center (UV)", Vector) = (0.5, 0.5, 0, 0)

        // 원형 반지름 (UV 스케일)
        _Radius ("Radius (UV)", Float) = 0.2

        // 사각형 half size (UV 스케일)
        _RectHalfSize ("Rect Half Size (UV)", Vector) = (0.2, 0.1, 0, 0)

        // 0 = Circle, 1 = Square
        _Shape ("Shape", Float) = 0

        // 경계 부드러움 (UV 스케일)
        _Feather ("Feather (UV)", Float) = 0.01

        // 딤 강도 (0~1). 1이면 완전 검정
        _DimAlpha ("Dim Alpha", Range(0,1)) = 0.75
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        // UI는 알파 블렌딩
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 uv       : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            fixed4 _Color;

            float2 _Center;
            float _Radius;
            float2 _RectHalfSize;
            float _Shape;
            float _Feather;
            float _DimAlpha;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color * _Color;

                // ScreenPos: 화면 UV 계산에 사용
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            // 사각형 SDF (axis-aligned)
            float sdBox(float2 p, float2 b)
            {
                // p: 중심 기준 좌표, b: half-size
                float2 d = abs(p) - b;
                return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 화면 UV(0~1). Overlay UI Image가 화면 전체를 덮는다는 전제
                float2 suv = (i.screenPos.xy / i.screenPos.w);

                // p: 구멍 중심 기준 좌표(uv 단위)
                float2 p = suv - _Center;

                // 거리(SDF): 내부면 음수/0, 외부면 양수
                float d;
                if (_Shape < 0.5)
                {
                    // Circle: 길이 - 반지름
                    d = length(p) - _Radius;
                }
                else
                {
                    // Square: 박스 SDF
                    d = sdBox(p, _RectHalfSize);
                }

                // feather로 경계 부드럽게:
                // d <= 0 : 구멍 내부(투명)
                // d > 0  : 딤 영역(불투명)
                // 경계 근처는 smoothstep로 그라데이션
                float feather = max(_Feather, 1e-6);
                float mask = smoothstep(0.0, feather, d); // 0(내부) -> 1(외부)

                fixed4 col = i.color;
                // 딤 알파 적용: 내부는 0, 외부는 _DimAlpha
                col.a *= (mask * _DimAlpha);

                return col;
            }
            ENDCG
        }
    }
}
