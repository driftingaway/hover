Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
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

            // srtuss, 2014
// A 2D effect based on repetition of boxes and and motion-blur.

#define pi2 3.1415926535897932384626433832795

float tri(float x, float s)
{
    return (abs(frac(x / s) - 0.5) - 0.25) * s;
}

float hash(float x)
{
    return frac(sin(x * 171.2972) * 18267.978 + 31.287);
}

float3 pix(float2 p, float t, float s)
{
    s += floor(t * 2);
    float scl = (hash(s + 30.0) * 14.0);
    scl += sin(t * 2.0) * 1 + sin(t) * 0.5;
    t *= 3.0;
    float2 pol = float2(atan2(p.y, p.x), length(p));
    float v;
    float id = floor(pol.y * 2.0 * scl);
    pol.x += t * (hash(id + s) * 2.0 - .1) * 0.4;
    float si = hash(id + s * 2.0);
    float rp = floor(hash(id + s * 5) * .1 + 4.0);
    v = (abs(tri(pol.x, pi2 / rp)) - si * 0.1) * pol.y;
    v = max(v, abs(tri(pol.y, 1 / scl)) - (1.0 - si) * 0.11);
    v = smoothstep(0.01, 0.0, v);
    return v;
}

float3 pix2(float2 p, float t, float s)
{
    return clamp(pix(p, t, s) - pix(p, t, s + 8.0) + pix(p * 0.1, t, s + 80.0) * 0.2, 0.0, 1.0);
}

float2 hash2(in float2 p)
{
	return frac(1965.5786 * float2(sin(p.x * 591.32 + p.y * 154.077), cos(p.x * 391.32 + p.y * 49.077)));
}

#define globaltime (_Time.y - 2.555)

float3 blur(float2 p)
{
    float3 ite = 0.0;
    for(int i = 0; i < 20; i ++)
    {
        float tc = 0.15;
        ite += pix2(p, globaltime * 3.0 + (hash2(p + float(i)) - 0.5).x * tc, 5.0);
    }
    ite /= 20.0;
    ite += exp(frac(globaltime * 0.25 * 6.0) * -40.0) * 6.0;
    return ite;
}

fixed4 frag (v2f i) : SV_Target
{
	float2 uv = i.uv;
    uv = 2.0 * uv - 1.0;
    //uv.x *= uv.x / uv.y;
    uv += (float2(hash(globaltime), hash(globaltime + 9.999)) - 0.5) * 0.03;
    float3 c = float3(blur(uv + float2(0.005, 0.0)).x, blur(uv + float2(0.0, 0.005)).y, blur(uv).z);
    c = pow(c, float3(0.4, 0.6, 1.0) * 2.0) * 1.5;
    c *= exp(length(uv) * -1.0) * 2.5;
    c = pow(c, 1.0 / 2.2);
	float4 fragColor = float4(c, 1.0);
    return fragColor;
}            
            
            ENDCG
        }
    }
}
