Shader "Kremen"
{
	Properties
	{
		
		_MainTex("Texture", 2D) = "white" {}
		[Toggle]_IsPixelated("Pixelated", Float) = 0
		[Toggle]_HardAttenuation("HardAttenuation", Float) = 0
		_HardAttenuationThreshold("HardAttenuationThreshold", Range(5,30)) = 15
		//[Toggle]_SoftAttenuation("SoftAttenuation", Float) = 0
		[Toggle]_SolidColor("SolidColor", Float) = 0
		[Toggle]_OldSchool("OldSchoolPixelated", Float) = 0
		_PixelTightness("PixelTightness",int) = 150
		_PixelColorShades("PixelColorShades",Range(2,256)) = 8

	}
		SubShader
	{
		Pass
		{
			Tags{ "LightMode" = "ForwardBase" "RenderType" = "Opaque" }
			CGPROGRAM
			#pragma multi_compile_fwdbase 
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc" 
			uniform float4 _LightColor0;

			sampler2D _MainTex;
			float4 _MainTex_ST;

			struct vertexInput {
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
			};
			struct v2f {
				float4 pos : SV_POSITION; //this shit does stuff, even when not used explicitly in fragment shader
				float2 uv : TEXCOORD0;
				//float4 worldPos : TEXCOORD1;
			};

			v2f vert(vertexInput v)
			{
				v2f o;
				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;
				//o.worldPos = mul(modelMatrix, v.vertex);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

				float4 frag(v2f i) : COLOR
				{
					float3 color = tex2D(_MainTex,i.uv).rgb*UNITY_LIGHTMODEL_AMBIENT.rgb;
					return float4(color,1.0);
				}
			ENDCG
		}
		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"


		Pass{

			Tags{ "LightMode" = "ForwardAdd"}
			Blend One One // additive blending 

			CGPROGRAM

			#pragma vertex vert  
			#pragma fragment frag 
			#include "UnityCG.cginc" 
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			// compile shader into multiple variants, with and without shadows
			// (we don't care about any lightmaps yet, so skip these variants)
			#pragma multi_compile_fwdadd_fullshadows nolightmap nodirlightmap nodynlightmap novertexlight //dunno what these other do. dunno if they improve performance
			// shadow helper functions and macros
			#include "UnityPBSLighting.cginc"
			#include "AutoLight.cginc"
			//#include "UnityShaderVariables.cginc" //included in autolight.cginc I think
			//uniform float4 _LightColor0; //included in Lighting.cginc
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _IsPixelated;
			float _HardAttenuation;
			int _HardAttenuationThreshold;
			int _PixelTightness;
			int _PixelColorShades;
			int _SolidColor;
			float _OldSchool;
			struct vertexInput {
				float4 vertex : POSITION;
				float4 uv : TEXCOORD2;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 worldPos : TEXCOORD0;
				SHADOW_COORDS(1)
				float2 uv : TEXCOORD2;
			};

			v2f vert(vertexInput v)
			{
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(UNITY_MATRIX_M, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				TRANSFER_SHADOW(o);

				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				float shadow = SHADOW_ATTENUATION(i);
				float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - i.worldPos.xyz;
				float distanceToLight = length(vertexToLightSource);
				float lightRange = 1 / _LightPositionRange.w;
				float attenuation = 1 - (distanceToLight / lightRange);
				
				if (_HardAttenuation == 1.0f)
				{
					//SMOOTH OUTWARD ATTENUATION
					attenuation *= 1000;
					attenuation += _HardAttenuationThreshold * 10;
					int remainder = attenuation % (_HardAttenuationThreshold*10);
					attenuation -= remainder;
					//attenuation += (remainder / 2);
					//attenuation += _HardAttenuationThreshold * 5;
					attenuation /= 1000;
					//color = tex2D(_MainTex, i.uv).rgb*_LightColor0*shadow*attenuation; //***use pos instead of uv to highlight light colors more***
					float4 finalColor = _LightColor0*attenuation;
					if (_OldSchool == 1.0f)
					{
						finalColor = _LightColor0*attenuation*_PixelColorShades;
						finalColor.r = round(finalColor.r);
						finalColor.g = round(finalColor.g);
						finalColor.b = round(finalColor.b);
						finalColor /= _PixelColorShades;
					}
				 	return float4(tex2D(_MainTex, i.pos).rgb*finalColor*shadow, 1.0);
				}
				if (_SolidColor == 1.0f && distanceToLight < lightRange)
				{
					//SMOOTH OUTWARD ATTENUATION
					return float4(tex2D(_MainTex, i.pos).rgb*_LightColor0*shadow, 1.0);
				}
				
				if (_IsPixelated == 1.0f)
				{
					vertexToLightSource *= 1000;
					int modValue = _PixelTightness;
					int3 pixelRemainder = int3(vertexToLightSource.x % modValue, vertexToLightSource.y % modValue, vertexToLightSource.z % modValue);
					vertexToLightSource.x -= pixelRemainder.x - modValue;
					vertexToLightSource.y -= pixelRemainder.y - modValue;
					vertexToLightSource.z -= pixelRemainder.z - modValue;
					vertexToLightSource /= 1000;
					distanceToLight = length(float3(vertexToLightSource.x, vertexToLightSource.y, vertexToLightSource.z));
					attenuation = 1 - (distanceToLight / lightRange);
					attenuation *= 3;
					

					//PIXEL LIGHT WITH/WITHOUT SMOOTH ATTENUATION //OOOOOOLD
					/*float3 pixelAttenuation = float3(1.0 - length((vertexToLightSource.x) / (lightRange)),
													 1.0 - length((vertexToLightSource.y) / (lightRange)),
													 1.0 - length((vertexToLightSource.z) / (lightRange)));*/
					//pixelAttenuation *= 1000; //multiply by 1000 to get better mod possibilities
					//int modValue = _PixelTightness / (lightRange); //lower = more "pixels"
					//int3 pixelRemainder = int3(pixelAttenuation.x % modValue, pixelAttenuation.y % modValue, pixelAttenuation.z % modValue);
					//pixelAttenuation.x -= pixelRemainder.x - (modValue);
					//pixelAttenuation.y -= pixelRemainder.y - (modValue);
					//pixelAttenuation.z -= pixelRemainder.z - (modValue);
					//pixelAttenuation /= 1000;
					//WITH ATTENUATION
					//attenuation *= pixelAttenuation.x*pixelAttenuation.y*pixelAttenuation.z * 3;
					//WITHOUT ATTENUATION
					//attenuation = pixelAttenuation.x * pixelAttenuation.y * pixelAttenuation.z*3;
					//need discard if no attenuation, not 100% sure why. maybe floating point error
					//NO DISCARD with attenuation
					//if (attenuation < 0.1)
					//{
					//	discard;
					//}
					////STRONGER LIGHT HIGHLIGHT
					//if (attenuation > 0.01)
					//{
					//	attenuation*= 0.96;
					//	attenuation += 0.04;
					//}
					float4 finalColor = _LightColor0*attenuation;
					if (_OldSchool == 1.0f)
					{
						finalColor = _LightColor0*attenuation*_PixelColorShades;
						finalColor.r = round(finalColor.r);
						finalColor.g = round(finalColor.g);
						finalColor.b = round(finalColor.b);
						finalColor /= _PixelColorShades;
					}
					return float4(tex2D(_MainTex, i.pos).rgb*finalColor *shadow, 1.0); //***use pos instead of uv to highlight light colors more***
				}
				
				//if (_OldSchool == 1.0f)
				//{
				//	float pixelAttenuation = attenuation;
				//	int bit = 32;
				//	float b = round(bit);
				//	float4 newColor = _LightColor0*pixelAttenuation*bit;
				//	//float4 addIfRoundUp;
				//	/*addIfRoundUp.r = newColor.r % bit < (bit * 0.5) ? 0.0 : 16;
				//	addIfRoundUp.g = newColor.g % bit < (bit * 0.5) ? 0.0 : 16;
				//	addIfRoundUp.b = newColor.b % bit < (bit * 0.5) ? 0.0 : 16;
				//	newColor.r = newColor.r - (newColor.r % bit) + addIfRoundUp.r;
				//	newColor.g = newColor.g - (newColor.g % bit) + addIfRoundUp.g;
				//	newColor.b = newColor.b - (newColor.b % bit) + addIfRoundUp.b;*/
				//	newColor.r = round(newColor.r);
				//	newColor.g = round(newColor.g);
				//	newColor.b = round(newColor.b);
				//	newColor /= bit;
				//	return float4(tex2D(_MainTex, i.pos).rgb*newColor*shadow, 1.0);
				//}
				float4 finalColor = _LightColor0*attenuation;
				if (_OldSchool == 1.0f)
				{
					finalColor = _LightColor0*attenuation*_PixelColorShades;
					finalColor.r = round(finalColor.r);
					finalColor.g = round(finalColor.g);
					finalColor.b = round(finalColor.b);
					finalColor /= _PixelColorShades;
				}
				return float4(tex2D(_MainTex, i.pos).rgb*finalColor*shadow, 1.0);
				
			}	
			ENDCG
		}


	}
}