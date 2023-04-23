Shader "Custom/LightingShader"
{
    Properties
    {
        _MainTex ("Chunk Array", 2DArray) = "white" {}
        _Slice ("Slice", int) = 0
        _UVscale ("UV Scale", float) = 1.0
        _TerrainWidth ("Nmb chunks horizontal", int) = 0
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100
        Pass
        {
            CGPROGRAM
            #pragma vertex vert alpha
            #pragma fragment frag alpha
            #pragma target 4.0
            
            #include "UnityCG.cginc"
            #include "HLSLSupport.cginc"

            struct v2f
            {
                float3 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            int _Slice;
            float _UVScale;
            int _TerrainWidth;
            int nmbLights;
            float4 _MainTex_TexelSize;
            StructuredBuffer<float4> lights;

            v2f vert (float4 vertex : POSITION)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(vertex);
                o.uv.xy = vertex.xy * _UVScale;
                o.uv.z = _Slice;
                return o;
            }
            
            UNITY_DECLARE_TEX2DARRAY(_MainTex);

            half4 frag (v2f i) : SV_Target
            {

                float2 dir;
                float2 p;
                uint slice = _Slice;
                float2 l;
                float totalBrightness = 0.0f;

                int chunkx = int(uint(_Slice) % uint(_TerrainWidth));// fmod(_TerrainWidth, _Slice);
                int chunky = int(uint(_Slice - chunkx) / uint(_TerrainWidth));
                float2 chunks = float2(chunkx, chunky);
                float2 globalTarget = i.uv.xy+chunks;

                 //for (uint b = 0; b < nmbLights; ++b) 
                 //{
                 //    slice = _Slice;
                 //    float2 fromTarget;
                 //    float lengthFromTarget;
                 //    float prevLength = 100.0f;
                 //    chunkx = int(uint(_Slice) % uint(_TerrainWidth));// fmod(_TerrainWidth, _Slice);
                 //    chunky = int(uint(_Slice - chunkx) / uint(_TerrainWidth));
                 //    chunks = float2(chunkx, chunky);

                 //    float brightness = 2.0f;
                 //    l = (lights[b].xy+0.5f*float2(-_SinTime.w*1.0f, sin(_Time.z * 1.0f + b * 0.5f))) - float2(chunkx, chunky);  //
                 //    dir = i.uv.xy - l;
                 //    if (length(dir) < 0.01f) return float4(1.0f,1.0f,1.0f,1.0f);

                 //    if (length(dir) > 2.0f*lights[b].z)
                 //    {
                 //        continue;
                 //    } 

                 //    dir = normalize(dir) * _MainTex_TexelSize.x * 1.0f;
                 //    p = l;

                 //    for(uint s = 0; s < uint(_MainTex_TexelSize.z * lights[b].z * 1.0f); ++s) /// 1.0f
                 //    {

                 //        if (p.x < 0.0f || p.x > 1.0f )
                 //        {
                 //            slice += floor(p.x);
                 //            p -= float2(floor(p.x), 0.0f);
                 //            chunkx = int(uint(slice) % uint(_TerrainWidth));
                 //            chunky = int(uint(slice - chunkx) / uint(_TerrainWidth));
                 //        }

                 //        if (p.y < 0.0f || p.y > 1.0f)
                 //        {
                 //            slice += floor(p.y) * _TerrainWidth;
                 //            p -= float2(0.0f, floor(p.y));
                 //            chunkx = int(uint(slice) % uint(_TerrainWidth));
                 //            chunky = int(uint(slice - chunkx) / uint(_TerrainWidth));
                 //        }


                 //        if (length((lights[b].xy) - (i.uv.xy + chunks)) < 0.01f || brightness < 0.01f) break;
                 //        fromTarget = (p+float2(chunkx, chunky)) - globalTarget;
                 //        lengthFromTarget = dot(fromTarget, fromTarget);

                 //        if (UNITY_SAMPLE_TEX2DARRAY_LOD(_MainTex, float3(p, slice), 0).w < 0.1f)
                 //        {
                 //            brightness -= 0.015f;
                 //            //brightness *= 0.96f;
                 //        } else 
                 //        {
                 //            brightness *= 0.95f;
                 //            //brightness *= 0.5f;
                 //        }

                 //        if (prevLength > lengthFromTarget) 
                 //        {
                 //            prevLength = lengthFromTarget;
                 //        } 
                 //        else 
                 //        {
                 //            break;
                 //        }
                 //       
                 //        p += dir;
                 //    }

                 //    if(brightness < 2.0f && brightness > 0.01f)
                 //    {
                 //        totalBrightness += brightness;
                 //    }

                 //    if (totalBrightness > 2.0f) break;
                 //}

                 //if (totalBrightness > 2.0f) totalBrightness = 2.0f;
                totalBrightness = 1.0f;
                float4 c = UNITY_SAMPLE_TEX2DARRAY_LOD(_MainTex, float3 (i.uv.xyz), 0);
                c = float4(c.xyz * totalBrightness, c.w);
                return c;
            }
            ENDCG
        }
    }
}
