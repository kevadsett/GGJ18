Shader "Unlit/CustomerSatisfaction"
{
	Properties
	{
		_EmptyTex ("Empty", 2D) = "white" {}
		_FullTex1 ("Full 1", 2D) = "white" {}
		_FullTex2 ("Full 2", 2D) = "white" {}
		_T2Ratio ("Full 2 Ratio", Range(0, 1)) = 0.3
		_TopOffset ("Fill Top Offset", Range (0, 1)) = 1.0
		_BtmOffset ("Fill Bottom Offset", Range (0, 1)) = 0.0
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
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _EmptyTex;
			float4 _EmptyTex_ST;

			sampler2D _FullTex1;
			float4 _FullTex1_ST;

			sampler2D _FullTex2;
			float4 _FullTex2_ST;

			float _FillAmt;
			float _T2Ratio;
			float _TopOffset;
			float _BtmOffset;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _EmptyTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col;
				float n = lerp (_BtmOffset, _TopOffset, _FillAmt);

				if (i.uv.y > n) {
					col = tex2D(_EmptyTex, i.uv);
				} else {
					if (_FillAmt > _T2Ratio) {
						col = tex2D(_FullTex1, i.uv);
					} else {
						col = tex2D(_FullTex2, i.uv);
					}
				}

				return col;
			}
			ENDCG
		}
	}
}
