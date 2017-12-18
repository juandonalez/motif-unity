// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)
//
// based on the Default Sprite shader from Unity
// modifications by ChevyNoel
// mask shape using a sprite in the sprite renderer
// apply color using Texture2D in the Color Texture part of the material
//
Shader "Sprites/Texture"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _ColorTex ("Color Texture", 2D) = "white" {}
 
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
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
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha
 
        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"
 
            sampler2D _ColorTex;
            fixed4 _ColorTex_ST;
           
            struct v2f2
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };
           
            v2f2 vert(appdata_t IN)
            {
                v2f2 OUT;
 
                UNITY_SETUP_INSTANCE_ID (IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
 
            #ifdef UNITY_INSTANCING_ENABLED
                IN.vertex.xy *= _Flip.xy;
            #endif
 
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.texcoord1 = TRANSFORM_TEX (IN.texcoord, _ColorTex);
                OUT.color = IN.color * _Color * _RendererColor;
 
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif
 
                return OUT;
            }
 
            fixed4 frag(v2f2 IN) : SV_Target
            {
                fixed4 c = tex2D (_ColorTex, IN.texcoord1) * IN.color;
                c.a = SampleSpriteTexture (IN.texcoord).a * IN.color.a;
                c.rgb *= c.a;
                return c;
            }
        ENDCG
        }
    }
}