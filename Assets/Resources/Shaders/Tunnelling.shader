Shader "Hidden/Tunnelling" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_AV ("Angular Velocity", Float) = 0
		_Feather ("Feather", Float) = 0.1
	}
	SubShader {
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ TUNNEL_SKYBOX
			
			#include "UnityCG.cginc"
			
			// vertex shader inputs
			struct appdata {
				float4 vertex : POSITION; // vertex position
				float2 uv : TEXCOORD0; // texture coordinate
			};
			// vertex shader outputs ("vertex to fragment")
			struct v2f {
				float2 uv : TEXCOORD0; // texture coordinate
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
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _AV;
			float _Feather;

			float4x4 _EyeProjection[2];
			float4x4 _EyeToWorld[2];

			
			inline float4 screenCoords(float2 uv){
				float2 c = (uv - 0.5) * 2;//modify size and position of hole  initially * 2
				float4 vPos = mul(_EyeProjection[unity_StereoEyeIndex], float4(c, 0, 1));
				//unity_StereoEyeIndex 0 (reps. 1) for left (resp. right) eye
				vPos.xyz /= vPos.w;//3 first coordinates divided by the 4th
				return vPos;
			}
			// pixel shader; returns low precision RGBA color("fixed4" type)
            // color ("SV_Target" semantic)
			fixed4 frag (v2f i) : SV_Target {
				float2 uv = UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST); //with stereo rendering
				// sample texture
				fixed4 col = tex2D(_MainTex, uv);

				float4 coords = screenCoords(i.uv);
				float radius = length(coords.xy / (_ScreenParams.xy / 2)) / 2;
				float avMin = (1 - _AV) - _Feather;
				float avMax = (1 - _AV) + _Feather;
				float t = saturate((radius - avMin) / (avMax - avMin));

				fixed4 effect = fixed4(0,0,0,0); //black

				return lerp(col, effect, t);
			}
			ENDCG
		}
	}
}
