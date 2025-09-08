Shader "Custom/OutLiner"{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Base",Color) = (1,1,1,1)
        _Ratio("Ratio",float) = 1
        _Border("Border",float) = 0.05
        _BorderColor("BorderColor",Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue" = "Overlay" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        LOD 200
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            float _Ratio;
            float _Border;
            float4 _Color;
            float4 _BorderColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float x = abs(i.uv.x - 0.5);
                float y = abs(i.uv.y - 0.5);

                float xBorder = 0.5 - _Border * _Ratio;
                float xLen = step(x,xBorder);
                float yBorder = 0.5 - _Border;
                float yLen = step(y,yBorder);

                float ToX = (x - xBorder)/(_Border*_Ratio);
                float ToY = (y - yBorder)/_Border;

                float BorderRatio = max(ToX,ToY);


                float4 fin = lerp(lerp(_Color,_BorderColor,BorderRatio),_Color,xLen * yLen);
                fin.a = 0.5;
                return fin;
            }
            ENDCG
        }
    }
}