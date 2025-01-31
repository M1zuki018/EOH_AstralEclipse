#ifndef NOISE_INCLUDED
#define NOISE_INCLUDED

// シンプルなノイズ関数
float NoiseFunction(float2 uv)
{
    return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
}

#endif
