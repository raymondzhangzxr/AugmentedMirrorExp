Shader "Custom/MirrorSurface" {
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_MainTex2("Texture2", 2D) = "white" {}
		_RimPower("Rim Power", Range(0.01,8.0)) = 0.25
	}
		SubShader{
		Tags{ "Queue" = "Transparent-1" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Pass{
			ZWrite On
			ColorMask 0
		}
		ZWrite On
		Cull Back
		CGPROGRAM
#pragma surface surf NoLighting alpha:fade 

		struct Input {
		float2 uv_MainTex;
		float4 screenPos;
		float3 viewDir;
	};
	sampler2D _MainTex;
	sampler2D _MainTex2;
	float _RimPower;

	void surf(Input IN, inout SurfaceOutput o) {
		float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
#if UNITY_SINGLE_PASS_STEREO
		float4 scaleOffset = unity_StereoScaleOffset[unity_StereoEyeIndex];
		screenUV = (screenUV - scaleOffset.zw) / scaleOffset.xy;
#endif
		screenUV.x = 1 - screenUV.x;
		half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
		if (unity_CameraProjection[0][2] >= 0) {
			//left eye
			o.Emission = tex2D(_MainTex, screenUV).rgb;
			o.Alpha = tex2D(_MainTex, screenUV).a * pow(rim, _RimPower);
		}
		else {
			//right eye
			o.Emission = tex2D(_MainTex2, screenUV).rgb;
			o.Alpha = tex2D(_MainTex2, screenUV).a * pow(rim, _RimPower);
		}
	}

	fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) {
		fixed4 c;
		c.rgb = s.Albedo;
		c.a = s.Alpha;
		return c;
	}


	ENDCG
		}
			Fallback "Diffuse"
}
