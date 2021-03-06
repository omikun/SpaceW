﻿// Procedural planet generator.
// 
// Copyright (C) 2015-2017 Denis Ovchinnikov [zameran] 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions
// are met:
// 1. Redistributions of source code must retain the above copyright
//    notice, this list of conditions and the following disclaimer.
// 2. Redistributions in binary form must reproduce the above copyright
//    notice, this list of conditions and the following disclaimer in the
//    documentation and/or other materials provided with the distribution.
// 3. Neither the name of the copyright holders nor the names of its
//    contributors may be used to endorse or promote products derived from
//    this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION)HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
// 
// Creation Date: Undefined
// Creation Time: Undefined
// Creator: zameran

Shader "SpaceEngine/NoiseEngineTest"
{
	Properties 
	{
		_Freq("Frequency", Float) = 1
	}
	SubShader 
	{
		Pass
		{
			ZTest Always

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "TCCommon.cginc"
			
			struct data
			{
				float4 vertex : POSITION;
				float3 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 uv : TEXCOORD0;
			};

			uniform float _Freq;

			v2f vert (data v)
			{
			    v2f o;

			    o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
			    o.uv = v.uv * _Freq;

			    return o;
			}

			float NoiseFunction(float3 pos)
			{
				return iNoise(pos, 1);
			}

			float3 FindNormal(float3 pos, float u)
			{
				float3 offsets[4];
				float hts[4];

				offsets[0] = pos + float3(-u, 0, 0);
				offsets[1] = pos + float3(u, 0, 0);
				offsets[2] = pos + float3(0, -u, 0);
				offsets[3] = pos + float3(0, u, 0);

				for(int i = 0; i < 4; i++)
				{
					hts[i] = NoiseFunction(offsets[i]);
				}

				float3 _step = float3(1, 0, 1);
			   
				float3 va = normalize(float3(_step.xy, hts[1] - hts[0]));
				float3 vb = normalize(float3(_step.yx, hts[3] - hts[2]));
			   
				return cross(va, vb); //you may not need to swizzle the normal
			}

			float4 frag(v2f i) : COLOR
			{
				float v = NoiseFunction(i.uv);
				
				return float4(v, v, v, 1);
				//return float4(FindNormal(i.uv, 1), 1);
			}
			ENDCG
		}
	} 
}