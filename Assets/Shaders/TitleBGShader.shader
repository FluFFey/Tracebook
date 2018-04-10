Shader "TitleBGShader" {
	Properties{
		_MainTex("Font Texture", 2D) = "white" {}
		_ColorT("Top Color", Color) = (1,1,1,1)
		_ColorB("Bot Color", Color) = (1,1,1,1)
	}

		SubShader{
		Blend One Zero

		Pass{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile _ UNITY_SINGLE_PASS_STEREO

#include "UnityCG.cginc"

	struct appdata_t {
		float4 vertex : POSITION;
		fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f {
		float4 vertex : SV_POSITION;
	//	fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
	};

	sampler2D _MainTex;
	uniform float4 _MainTex_ST;
	uniform fixed4 _ColorT;
	uniform fixed4 _ColorB;

	v2f vert(appdata_t v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		float fullY = _ScreenParams.y; //screen height in pixels I think
		int modVal = int(fullY / 16);
		float modifier = 0.7 + sin(_Time*70)*0.015;
		fixed4 col = _ColorT*((i.vertex.y ) / fullY) +
			_ColorB*(modifier- (i.vertex.y - i.vertex.y%modVal) / fullY);
		return col;
	}
		ENDCG
	}
	}
}

