Shader "Sprites/Sprites_Default-ColorFlash"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _FlashColor("Flash Color",Color) = (1,1,1,1)
        _FlashAmount("Flash Amount",Range(0.0,1.0)) = 0.0
        _Color("Color", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap("Pixel snap",Float) = 0.0
    }
    SubShader
    {
        Tags { "CanUseSpriteAtlas" = "true" "IGNOREPROJECTOR" = "true" "PreviewType" = "Plane" "QUEUE" = "Transparent" "RenderType" = "Transparent"}

        Cull Off
        Lighting Off
        ZWrite Off
        Fog{ Mode Off }
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
         #pragma surface surf Lambert alpha vertex:vert
         #pragma multi_compile DUMMY PIXELSNAP_ON

         sampler2D _MainTex;
         fixed4 _Color;
         fixed4 _FlashColor;
         float _FlashAmount,_SelfIllum;

         struct Input
         {
             float2 uv_MainTex;
             fixed4 color;
         };

         void vert(inout appdata_full v, out Input o)
         {
             #if defined(PIXELSNAP_ON) && !defined(SHADER_API_FLASH)
             v.vertex = UnityPixelSnap(v.vertex);
             #endif
             v.normal = float3(0,0,-1);

             UNITY_INITIALIZE_OUTPUT(Input, o);
             o.color = _FlashColor;
         }

         void surf(Input IN, inout SurfaceOutput o)
         {
             fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
             o.Albedo = lerp(c.rgb,float3(1.0,1.0,1.0),_FlashAmount);
             o.Emission = lerp(c.rgb,float3(1.0,1.0,1.0),_FlashAmount) * _SelfIllum;
             o.Alpha = c.a;
         }
         ENDCG
    }
    Fallback "Transparent/VertexLit"
}
