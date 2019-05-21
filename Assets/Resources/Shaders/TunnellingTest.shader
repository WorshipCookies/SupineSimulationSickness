Shader "Hidden/TunnellingTest" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_AV ("Angular Velocity", Float) = 0
		_Feather ("Feather", Float) = 0.1
		_BlurSizeXY("BlurSizeXY", Range(0,10)) = 5
	}
	SubShader {
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ TUNNEL_SKYBOX
			#ifndef SHADER_API_D3D11

				#pragma target 3.0

			#else

				#pragma target 4.0

			#endif
			
			#include "UnityCG.cginc"
			
			// vertex shader inputs
			struct appdata {
				float4 vertex : POSITION; // vertex position
				float4 uv : TEXCOORD0; // texture coordinate
			};
			// vertex shader outputs ("vertex to fragment")
			struct v2f {
				float4 uv : TEXCOORD0; // texture coordinate
				float4 vertex : SV_POSITION; // clip space position
			};
			// vertex shader
			v2f vert (appdata v) {
				// transform position to clip space
                // (multiply with model*view*projection matrix)
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				// just pass the texture coordinate
				o.uv = v.uv;
				return o;
			}
			// texture we will sample
			sampler2D _GrabTexture : register(s0);
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _AV;
			float _Feather;
			float _BlurSizeXY;

			float4x4 _EyeProjection[2];
			float4x4 _EyeToWorld[2];

			
			inline float4 screenCoords(float2 uv){
				float2 c = (uv - 0.5) * 2;//modify size and position of hole
				float4 vPos = mul(_EyeProjection[unity_StereoEyeIndex], float4(c, 0, 1));
				//unity_StereoEyeIndex 0 (reps. 1) for left (resp. right) eye
				vPos.xyz /= vPos.w;//3 first coordinates divided by the 4th
				return vPos;
			}

			//blur

			// pixel shader; returns low precision RGBA color("fixed4" type)
            // color ("SV_Target" semantic)
			fixed4 frag (v2f i) : SV_Target {
				float2 uv = UnityStereoScreenSpaceUVAdjust(float2(i.uv.x, i.uv.y), _MainTex_ST); //with stereo rendering
				// sample texture
				fixed4 col = tex2D(_MainTex, uv);

				float4 coords = screenCoords(i.uv);
				float radius = length(coords.xy / (_ScreenParams.xy / 2)) / 2;
				float avMin = (1 - _AV) - _Feather;
				float avMax = (1 - _AV) + _Feather;
				float t = saturate((radius - avMin) / (avMax - avMin));

				fixed4 effect = fixed4(0,0,0,0); //black

				//blur 
				float depth= _BlurSizeXY*0.0005 ; //adde *2

				fixed4 sum = fixed4(0,0,0,0);
				sum += tex2D( _MainTex, float2(uv.x-5.0 * depth, uv.y+5.0 * depth)) * 0.025;    
				sum += tex2D( _MainTex, float2(uv.x+5.0 * depth, uv.y-5.0 * depth)) * 0.025;

				sum += tex2D( _MainTex, float2(uv.x-5.0 * depth, uv.y+5.0 * depth)) * 0.025;    
				sum += tex2D( _MainTex, float2(uv.x+5.0 * depth, uv.y-5.0 * depth)) * 0.025;
    
				sum += tex2D( _MainTex, float2(uv.x-4.0 * depth, uv.y+4.0 * depth)) * 0.05;
				sum += tex2D( _MainTex, float2(uv.x+4.0 * depth, uv.y-4.0 * depth)) * 0.05;

    
				sum += tex2D( _MainTex, float2(uv.x-3.0 * depth, uv.y+3.0 * depth)) * 0.09;
				sum += tex2D( _MainTex, float2(uv.x+3.0 * depth, uv.y-3.0 * depth)) * 0.09;
    
				sum += tex2D( _MainTex, float2(uv.x-2.0 * depth, uv.y+2.0 * depth)) * 0.12;
				sum += tex2D( _MainTex, float2(uv.x+2.0 * depth, uv.y-2.0 * depth)) * 0.12;
    
				sum += tex2D( _MainTex, float2(uv.x-1.0 * depth, uv.y+1.0 * depth)) *  0.15;
				sum += tex2D( _MainTex, float2(uv.x+1.0 * depth, uv.y-1.0 * depth)) *  0.15;
    
	

				sum += tex2D( _MainTex, uv-5.0 * depth) * 0.025;    
				sum += tex2D( _MainTex, uv-4.0 * depth) * 0.05;
				sum += tex2D( _MainTex, uv-3.0 * depth) * 0.09;
				sum += tex2D( _MainTex, uv-2.0 * depth) * 0.12;
				sum += tex2D( _MainTex, uv-1.0 * depth) * 0.15;    
				sum += tex2D( _MainTex, uv) * 0.16; 
				sum += tex2D( _MainTex, uv+5.0 * depth) * 0.15;
				sum += tex2D( _MainTex, uv+4.0 * depth) * 0.12;
				sum += tex2D( _MainTex, uv+3.0 * depth) * 0.09;
				sum += tex2D( _MainTex, uv+2.0 * depth) * 0.05;
				sum += tex2D( _MainTex, uv+1.0 * depth) * 0.025;

				sum /= 2;
       
				return lerp(col, sum, t);
				//return sum/2;
			}
			ENDCG
		}
	}
}
