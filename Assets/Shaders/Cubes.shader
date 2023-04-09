Shader "Unlit/Cubes"
{
    Properties
    {
        _MainTex ("tex2D", 2D) = "white" {}
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

            #define FOV_MORPH 1
            #define mod(x, y) (x-y*floor(x/y))

float box(float3 p) {
	return length(max(abs(p)-.5,0.0)) - 0.15;
}

float3 rot(float3 p, float f) {
	float s = sin(f);
	float c = cos(f);
	p.xy = mul(float2x2(c, -s, s, c), p.xy);
	p.yz = mul(float2x2(c, -s, s, c), p.yz);
	return p;
}


float3 trans(float3 p, out float rotout) {
	p.zx += _Time.y*35.0;

	float3 b = 4;
	float3 rep = floor(p/b);

	p = mod(p,b)-0.5*b;
	
	rotout = _Time.y*1.88 + (rep.x+rep.z+rep.y)*3;
	p = rot(p, rotout);
	return p;	
}

float scene(float3 p) {
	float dummy;
	return box(trans(p,dummy));
}


float3 normal(float3 p, float d) {
	float3 e = float3(0.04,.0,.0);
	return normalize(float3(
		scene(p+e.xyy)-d,
		scene(p+e.yxy)-d,
		scene(p+e.yyx)-d));
	
}

fixed4 frag (v2f i) : SV_Target
{
	float2 xy = i.uv - float2(0.5,0.5);
	//xy.y *= -iResolution.y / iResolution.x;

	float time = _Time.y*0.5;
	float3 ro = 1.5*normalize(float3(cos(time),cos(time)*1.2,sin(time)));
    float3 eyed = normalize(0.0 - ro);
    float3 ud = normalize(cross(float3(0.0,1.0,0.0), eyed));
    float3 vd = normalize(cross(eyed,ud));

#if FOV_MORPH
	float fov = 3.14 * 0.8 + sin(time*1.334)*7;
#else
	float fov = 3.14 * 0.7;
#endif
	
	float f = fov * length(xy);
	float3 rd = normalize(normalize(xy.x*ud + xy.y*vd) + (1.0/tan(f))*eyed);


	float3 p = ro + rd;

	float dall,d;
	for(int i = 0; i < 64; i++) {
		d = scene(p);
		if(d < 0.06) break;
		p += d*rd;
		dall += d;
	}

	float3 bg = normalize(p).zzz + 0.1;

	if(d < 0.06) {
		float3 n = normal(p,d); 
		float3 col = dot(float3(0.0,0.0,1.0), n);
		float objrot;
		float3 objp = trans(p,objrot);
		float3 objn = abs(rot(n,objrot));
		
		float2 uv = 
			(objn.y > 0.707) ? float2(objp.zx) : 
			(objn.x > 0.707) ? float2(objp.zy) :
							   float2(objp.xy) ;
		float3 tex = tex2D(_MainTex, uv).rgb;
		float3 hl = smoothstep(0.6, 1.0, col);
		col *= clamp(tex.xyz+0.3, 0.0, 1.0);

		col = col + hl*.4;
		float fog = clamp(dall/lerp(90.0,40.0,((rd.z+1.0)*0.5)), 0.0, 1.0);

		return float4(lerp(col, bg, fog),1.0);
	}
	else {
		return float4(bg, 1.0);
	}		
}ENDCG
}
}
}