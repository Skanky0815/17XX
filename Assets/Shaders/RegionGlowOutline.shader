Shader "Rico/RegionGlowOutline"
{
    Properties
    {
        _RegionIDMap ("Region ID Map", 2D) = "white" {}
        _HoverColor ("Hover Color", Color) = (0, 0, 0, 0)
        _GlowColor ("Glow Color", Color) = (5, 5, 5, 1)
        _GlowStrength ("Glow Strength", Float) = 1.0
        _Tolerance ("Color Tolerance", Float) = 0.01
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend One One
        ZWrite Off
        ZTest LEqual
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _RegionIDMap;
            float4 _HoverColor;
            float4 _GlowColor;
            float _GlowStrength;
            float _Tolerance;
            float4 _RegionIDMap_TexelSize;

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 centerColor = tex2D(_RegionIDMap, i.uv);

                if (_HoverColor.a <= 0.0)
                    discard;

                float diffCenter = distance(centerColor.rgb, _HoverColor.rgb);
                if (diffCenter >= _Tolerance)
                    discard;

                float2 texelOffset = float2(_RegionIDMap_TexelSize.x * 2.0, _RegionIDMap_TexelSize.y * 2.0);
                float4 leftColor  = tex2D(_RegionIDMap, i.uv + float2(-texelOffset.x, 0));
                float4 rightColor = tex2D(_RegionIDMap, i.uv + float2(texelOffset.x, 0));
                float4 upColor    = tex2D(_RegionIDMap, i.uv + float2(0, texelOffset.y));
                float4 downColor  = tex2D(_RegionIDMap, i.uv + float2(0, -texelOffset.y));

                bool isBorder = distance(leftColor.rgb, centerColor.rgb) > _Tolerance ||
                                distance(rightColor.rgb, centerColor.rgb) > _Tolerance ||
                                distance(upColor.rgb, centerColor.rgb) > _Tolerance ||
                                distance(downColor.rgb, centerColor.rgb) > _Tolerance;

                if (!isBorder)
                    discard;

                float edgeWidth = fwidth(diffCenter);
                float glowFactor = max(smoothstep(0.0, edgeWidth * 2.0, diffCenter), 0.2);

                return _GlowColor * _GlowStrength * glowFactor;
            }
            ENDCG
        }
    }

    FallBack Off
}