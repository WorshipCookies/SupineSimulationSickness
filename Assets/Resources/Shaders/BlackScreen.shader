Shader "Hidden/BlackScreen" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}

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

			// pixel shader; returns low precision RGBA color("fixed4" type)
            // color ("SV_Target" semantic)
			fixed4 frag (v2f i) : SV_Target {

				fixed4 effect = fixed4(0,0,0,0); //black

				return  effect;
			}
			ENDCG
		}
	}
}
