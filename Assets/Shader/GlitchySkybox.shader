Shader "Custom/GlitchyCubeSkybox"
{
    Properties
    {
        _SkyboxCube ("Cubemap", CUBE) = "" {}
        _GlitchIntensity ("Glitch Intensity", Range(0, 1)) = 0.1
        _DistortionSpeed ("Distortion Speed", Range(0, 10)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Background" "Queue"="Background" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            samplerCUBE _SkyboxCube;
            float _GlitchIntensity;
            float _DistortionSpeed;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float3 texcoord : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            // 擬似ランダムノイズ関数
            float NoiseFunction(float3 pos)
            {
                return frac(sin(dot(pos, float3(12.9898, 78.233, 45.164))) * 43758.5453);
            }

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 uv = i.texcoord;

                // ノイズを使ってUV座標を歪ませる
                float distortion = (NoiseFunction(uv + _Time.y * _DistortionSpeed) - 0.5) * _GlitchIntensity;
                uv += distortion;

                // CubeMap からサンプリング
                fixed4 col = texCUBE(_SkyboxCube, uv);

                return col;
            }
            ENDCG
        }
    }
}
