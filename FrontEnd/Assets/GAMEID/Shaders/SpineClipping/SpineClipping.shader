Shader "Spine/Clipping" {
	Properties {
		_Cutoff ("Shadow alpha cutoff", Range(0,1)) = 0.1
		[NoScaleOffset] _MainTex ("Main Texture", 2D) = "black" {}
		[Toggle(_STRAIGHT_ALPHA_INPUT)] _StraightAlphaInput("Straight Alpha Texture", Int) = 0
		[HideInInspector] _StencilRef("Stencil Reference", Float) = 1.0
		[HideInInspector][Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comparison", Float) = 8 // Set to Always as default

		// Outline properties are drawn via custom editor.
		[HideInInspector] _OutlineWidth("Outline Width", Range(0,8)) = 3.0
		[HideInInspector] _OutlineColor("Outline Color", Color) = (1,1,0,1)
		[HideInInspector] _OutlineReferenceTexWidth("Reference Texture Width", Int) = 1024
		[HideInInspector] _ThresholdEnd("Outline Threshold", Range(0,1)) = 0.25
		[HideInInspector] _OutlineSmoothness("Outline Smoothness", Range(0,1)) = 1.0
		[HideInInspector][MaterialToggle(_USE8NEIGHBOURHOOD_ON)] _Use8Neighbourhood("Sample 8 Neighbours", Float) = 1
		[HideInInspector] _OutlineMipLevel("Outline Mip Level", Range(0,3)) = 0

		// Clipping Mask Properties - [ SAG ]
		[HideInInspector] Vector4_ClippingOrigin 		("Clipping Origin", Vector) = 			(0.000000,0.000000,0.000000,0.000000)
 		[HideInInspector] Vector4_ClippingXAxis 		("Clipping X Axis", Vector) = 			(1.000000,0.000000,0.000000,0.000000)
 		[HideInInspector] Vector4_ClippingYAxis 		("Clipping Y Axis", Vector) = 			(0.000000,1.000000,0.000000,0.000000)
 		[HideInInspector] Vector4_ClippingZAxis 		("Clipping Z Axis", Vector) = 			(0.000000,0.000000,1.000000,0.000000)
 		[HideInInspector] Vector4_ClippingBoundingSize 	("Clipping Bounding Size", Vector) = 	(1000.000,1000.000,1000.000,0.000000)
 		                  Vector4_TintValue 	        ("Tint Value", Vector) =                (1.000,1.000,1.000,1.000)
		                  _DepthScalar                  ("Depth Scalar", Range(0,4)) =          1.0
	}

	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }

		Fog { Mode Off }
		Cull Off
		ZWrite Off

		Blend One OneMinusSrcAlpha
		Lighting Off

		Stencil {
			Ref[_StencilRef]
			Comp[_StencilComp]
			Pass Keep
		}

		Pass {
			Name "Normal"

			CGPROGRAM
			#pragma shader_feature _ _STRAIGHT_ALPHA_INPUT
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			sampler2D _MainTex;

			// Clipping Mask Properties - [ SAG ]
			float4 Vector4_ClippingOrigin;
			float4 Vector4_ClippingXAxis;
			float4 Vector4_ClippingYAxis;
			float4 Vector4_ClippingZAxis;
			float4 Vector4_ClippingBoundingSize;
			float4 Vector4_TintValue;
			float  _DepthScalar;

			struct VertexInput {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
			};

			struct VertexOutput {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
				// World position for clipping - [ SAG ]
				float3 worldPos : TEXCOORD1;
			};

			// Output white if inside mask, black if outside - [ SAG ]
			float4 clippingMask (VertexOutput i) {
				float3 posDiff = i.worldPos - Vector4_ClippingOrigin;

				float xComponentAmt = abs( dot(Vector4_ClippingXAxis, posDiff) );
				float xComponentDiff = max(Vector4_ClippingBoundingSize.x - xComponentAmt, 0.0);

				float yComponentAmt = abs( dot(Vector4_ClippingYAxis, posDiff) );
				float yComponentDiff = max(Vector4_ClippingBoundingSize.y - yComponentAmt, 0.0);

				float zComponentAmt = abs( dot(Vector4_ClippingZAxis, posDiff) );
				float zComponentDiff = max(Vector4_ClippingBoundingSize.z - zComponentAmt, 0.0);

				if (xComponentDiff * yComponentDiff * zComponentDiff == 0) {
					return float4(0,0,0,0);
				}
				return float4(1,1,1,1);
			}

			VertexOutput vert (VertexInput v) {
				VertexOutput o;

				// Calculate world position for clipping calculations - [ SAG ]
				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				float4 depthScaled = float4((worldPos.xyz - _WorldSpaceCameraPos) * _DepthScalar + _WorldSpaceCameraPos, worldPos.w);
				float4 objectPos = mul(unity_WorldToObject, depthScaled);
				
				o.pos = UnityObjectToClipPos(objectPos);
				o.uv = v.uv;
				o.vertexColor = v.vertexColor;
				
				o.worldPos = worldPos;
				return o;
			}

			float4 frag (VertexOutput i) : SV_Target {
				float4 texColor = tex2D(_MainTex, i.uv);

				#if defined(_STRAIGHT_ALPHA_INPUT)
				texColor.rgb *= texColor.a;
				#endif

				// multiply frag output by result of clipping test - [ SAG ]
				return (texColor * i.vertexColor * clippingMask(i) * Vector4_TintValue);
			}
			ENDCG
		}

		Pass {
			Name "Caster"
			Tags { "LightMode"="ShadowCaster" }
			Offset 1, 1
			ZWrite On
			ZTest LEqual

			Fog { Mode Off }
			Cull Off
			Lighting Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			fixed _Cutoff;

			struct VertexOutput {
				V2F_SHADOW_CASTER;
				float4 uvAndAlpha : TEXCOORD1;
			};

			VertexOutput vert (appdata_base v, float4 vertexColor : COLOR) {
				VertexOutput o;
				o.uvAndAlpha = v.texcoord;
				o.uvAndAlpha.a = vertexColor.a;
				TRANSFER_SHADOW_CASTER(o)
				return o;
			}

			float4 frag (VertexOutput i) : SV_Target {
				fixed4 texcol = tex2D(_MainTex, i.uvAndAlpha.xy);
				clip(texcol.a * i.uvAndAlpha.a - _Cutoff);
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
	}
	CustomEditor "SpineShaderWithOutlineGUI"
}
