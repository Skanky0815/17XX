Shader "Custom/RegionHighlighter"
{
    Properties
    {
        _MainTex ("Map Texture", 2D) = "white" {}
        _RegionIDMap ("Region ID Map", 2D) = "white" {}
        _HoverColor ("Hover Color", Color) = (0,0,0,0)
        _HighlightColor ("Highlight Color", Color) = (0,0,0,0)
        _Tolerance ("Color Tolerance", Float) = 0.01
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            sampler2D _RegionIDMap;
            float4 _HoverColor;
            float4 _HighlightColor;
            float _Tolerance;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 baseColor = tex2D(_MainTex, i.uv);
                float4 regionColor = tex2D(_RegionIDMap, i.uv);

                float diff = distance(regionColor.rgb, _HighlightColor.rgb);

                if (_HighlightColor.a > 0.0 && diff < _Tolerance)
                {
                    // Highlighted region
                    //return lerp(baseColor, _HighlightColor, 0.5);
                    return _HighlightColor;
                }

                return baseColor;
            }
            ENDCG
        }
    }
}