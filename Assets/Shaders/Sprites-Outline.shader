// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sprites/SpriteOutliner"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
	_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[PerRendererData] _Outline("Outline", Float) = 0

	}

		SubShader
	{
		Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
		"PreviewType" = "Plane"
		"CanUseSpriteAtlas" = "True"
	}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile _ PIXELSNAP_ON
#include "UnityCG.cginc"

	struct appdata_t
	{
		float4 vertex   : POSITION;
		float4 color    : COLOR;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex   : SV_POSITION;
		fixed4 color : COLOR;
		float2 texcoord  : TEXCOORD0;
	};

	fixed4 _Color;

	v2f vert(appdata_t IN)
	{
		v2f OUT;
		OUT.vertex = UnityObjectToClipPos(IN.vertex);
		OUT.texcoord = IN.texcoord;
		OUT.color = IN.color * _Color;
#ifdef PIXELSNAP_ON
		OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif

		return OUT;
	}

	sampler2D _MainTex;
	sampler2D _AlphaTex;
	float _AlphaSplitEnabled;
	float4 _MainTex_TexelSize;
	float _Outline;
	fixed4 SampleSpriteTexture(float2 uv)
	{
		fixed4 color = tex2D(_MainTex, uv);

#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
		if (_AlphaSplitEnabled)
			color.a = tex2D(_AlphaTex, uv).r;
#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

		return color;
	}

	fixed4 frag(v2f IN) : SV_Target
	{
		fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
		if (c.r+ c.g+ c.b < 0.9)
		{
			//c.rgb = fixed3(1, 1, 1);
		}
		int outlineThickness = 10.0f;
		if (c.a > 0.1 && _Outline == 1)
		{
			fixed4 pixelUp = tex2D(_MainTex, IN.texcoord + fixed2(0, outlineThickness * _MainTex_TexelSize.y));
			fixed4 pixelDown = tex2D(_MainTex, IN.texcoord - fixed2(0, outlineThickness * _MainTex_TexelSize.y));
			fixed4 pixelRight = tex2D(_MainTex, IN.texcoord + fixed2(outlineThickness * _MainTex_TexelSize.x, 0));
			fixed4 pixelLeft = tex2D(_MainTex, IN.texcoord - fixed2(outlineThickness * _MainTex_TexelSize.x, 0));

			if (pixelUp.a*pixelDown.a*pixelRight.a*pixelLeft.a == 0)
			{
				c.rgb = fixed3(1, 1, 1);
			}
		}
		c.rgb *= c.a;
	return c;
	}
		ENDCG
	}
	}
}