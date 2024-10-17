Shader "Hollow Knight/Grass-Default"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" { }
        _Color("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
        _RendererColor("RendererColor", Color) = (1,1,1,1)
        _Flip("Flip", Vector) = (1,1,1,1)
        _AlphaTex("External Alpha", 2D) = "white" { }
        _EnableExternalAlpha("Enable External Alpha", Float) = 0
        _SwaySpeed("SwaySpeed", Float) = 1
        _SwayAmount("Sway Amount", Float) = 1
        _WorldOffset("World Offset", Float) = 1
        _HeightOffset("Height Offset", Float) = 0
        _ClampZ("Clamp Z Position", Float) = 1
        _PushAmount("Push Amount (Player)", Float) = 0
    }
    SubShader
    {
        Tags { "CanUseSpriteAtlas" = "true" "DisableBatching" = "true" "IGNOREPROJECTOR" = "true" "PreviewType" = "Plane" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        Pass
        {
            Tags { "CanUseSpriteAtlas" = "true" "DisableBatching" = "true" "IGNOREPROJECTOR" = "true" "PreviewType" = "Plane" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
            Blend One OneMinusSrcAlpha, One OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
Fallback "Sprites/Diffuse"
}
