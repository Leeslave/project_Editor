Shader "Unlit/Sight"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Base",Color) = (1,1,1,1)
        _SightVar("Sight",float) = 0.0025
    }
SubShader{
    Tags { "Queue" = "Overlay" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
    Cull Off
    ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha
    Pass 
    {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            
            struct appdata{float4 vertex : POSITION;};

            struct v2f {
			    float4 pos : SV_POSITION;
			    float2 uv : TEXCOORD0;
		    };
		
		    v2f vert( appdata_img v )
		    {
			    v2f o;
			    o.pos = UnityObjectToClipPos (v.vertex);
			    o.uv = v.vertex.xy;
    
			    return o;
		    }

            float4 _Color;
            float _SightVar;

            half4 frag(v2f i) : COLOR
            {
                float dist = dot(i.uv,i.uv) * _SightVar;

                if(dist > 1) discard;

                float4 color;
                color.rgb = _Color.rgb;
                color. a = 1 - saturate(pow(dist,0.35));

                return color;
            }
            ENDCG
        }
    }
}
