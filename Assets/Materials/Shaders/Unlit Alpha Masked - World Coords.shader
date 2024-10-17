Shader "Alpha Masked/Unlit Alpha Masked - World Coords"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [Toggle] _ClampHoriz("Clamp Alpha Horizontally", Float) = 0
        [Toggle] _ClampVert("Clamp Alpha Vertically", Float) = 0
        [Toggle] _UseAlphaChannel("Use Mask Alpha Channel (not RGB)", Float) = 0
        _MaskRotation("Mask Rotation in Radians", Float) = 0
        _AlphaTex("Alpha Mask", 2D) = "white" {}
        _ClampBorder("Clamping Border", Float) = 0.01
        [KeywordEnum(X, Y, Z)] _Axis("Alpha Mapping Axis", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}

        ZWrite Off

        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB

        Pass {
            SetTexture[_MainTex] {
                Combine texture
            }
            SetTexture[_AlphaTex] {
                Combine previous, texture
            }
        }
    }
    Fallback "Unlit/Texture"
}
