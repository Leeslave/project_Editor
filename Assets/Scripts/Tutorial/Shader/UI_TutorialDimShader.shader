Shader "UI/TutorialDimShader"
{
    Properties
    {
        _DimColor ("Dim Color (RGBA)", Color) = (0,0,0,0.75)
        _OutlineColor ("Outline Color (RGBA)", Color) = (1,1,1,1)

        // 0 = Circle, 1 = Rect
        _ShapeType ("Shape Type (0 Circle, 1 Rect)", Float) = 1

        // UV(0~1)
        _HoleCenter ("Hole Center (UV)", Vector) = (0.5, 0.5, 0, 0)

        // Rect: (widthUV, heightUV), Circle: (radiusUV, unused)
        _HoleSize ("Hole Size (Rect wh UV / Circle r UV)", Vector) = (0.2, 0.2, 0, 0)

        // Rect only (UV). 0이면 직각
        _CornerRadius ("Corner Radius (UV)", Float) = 0.0

        // UV 두께 (픽셀 고정으로 쓰고 싶으면 C#에서 px->uv 변환해서 넣기)
        _OutlineThickness ("Outline Thickness (UV)", Float) = 0.005

        // UV feather (부드러운 가장자리). C#에서 px->uv 변환 가능
        _Feather ("Feather (UV)", Float) = 0.002
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

        Cull Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color    : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv     : TEXCOORD0;
                float4 color  : COLOR;
                float3 positionWS : TEXCOORD1;
            };

            fixed4 _DimColor;
            fixed4 _OutlineColor;

            float _ShapeType;
            float4 _HoleCenter;
            float4 _HoleSize;
            float _CornerRadius;
            float _OutlineThickness;
            float _Feather;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color;

                o.positionWS = mul(unity_ObjectToWorld, v.vertex);

                return o;
            }

            // Signed Distance: Rounded Rect (centered at origin)
            float sdRoundBox(float2 p, float2 halfSize, float r)
            {
                float2 q = abs(p) - halfSize + r;
                return length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - r;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // float2 uv = i.uv;

                float2 center = _HoleCenter.xy; // UV
                float2 p = i.positionWS.xy - center;

                // === Circle Aspect Correction ===
                // 원이 화면 종횡비 때문에 타원으로 보이는 걸 방지
                // x축을 (width/height)만큼 스케일
                // float aspect = _ScreenParams.x / max(1.0, _ScreenParams.y);
                // float2 pCircle = float2(p.x * 1, p.y);

                float feather = max(_Feather, 1e-6);
                float thick   = max(_OutlineThickness, 0.0);

                float dist;

                // 0 = Circle, 1 = Rect
                if (_ShapeType < 0.5)
                {
                    // Circle: _HoleSize.x = radius (UV 기준)
                    float radius = max(_HoleSize.x, 0.0);
                    dist = length(p) - radius;
                }
                else
                {
                    // Rect: _HoleSize.xy = width/height (UV 기준)
                    float2 halfSize = max(_HoleSize.xy * 0.5, 0.0);

                    // corner radius는 halfSize보다 클 수 없게 clamp
                    float r = clamp(_CornerRadius, 0.0, min(halfSize.x, halfSize.y));
                    dist = sdRoundBox(p, halfSize, r);
                }

                // inside: dist < 0
                float inside = 1.0 - smoothstep(0.0, feather, dist);

                // outline: 경계(dist=0) 주변 띠
                float outline = 1.0 - smoothstep(thick, thick + feather, abs(dist));

                // Dim: 기본 딤, 구멍(inside)은 투명
                fixed4 dimCol = _DimColor;
                dimCol.a *= (1.0 - inside);

                // Outline
                fixed4 outCol = _OutlineColor;
                outCol.a *= outline;

                // Composite (dim + outline)
                fixed4 col = dimCol;
                col.rgb = lerp(col.rgb, outCol.rgb, outCol.a);
                col.a = saturate(col.a + outCol.a);

                // UI Vertex Color 곱(필요 없으면 제거해도 됨)
                col *= i.color;

                return col;
            }
            ENDCG
        }
    }
}