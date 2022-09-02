// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "CustomFade"
{
	Properties
	{
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		_VoronoiTile("VoronoiTile", Vector) = (5,5,0,0)
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		[Toggle(_REVERSE_ON)] _Reverse("Reverse", Float) = 0
		_Life_Speed_Multiplier("Life_Speed_Multiplier", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}


	Category 
	{
		SubShader
		{
		LOD 0

			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
			Blend One One
			ColorMask RGB
			Cull Off
			Lighting Off 
			ZWrite Off
			ZTest LEqual
			
			Pass {
			
				CGPROGRAM
				
				#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
				#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
				#endif
				
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_instancing
				#pragma multi_compile_particles
				#pragma multi_compile_fog
				#define ASE_NEEDS_FRAG_COLOR
				#pragma shader_feature_local _REVERSE_ON


				#include "UnityCG.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					#ifdef SOFTPARTICLES_ON
					float4 projPos : TEXCOORD2;
					#endif
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
					
				};
				
				
				#if UNITY_VERSION >= 560
				UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
				#else
				uniform sampler2D_float _CameraDepthTexture;
				#endif

				//Don't delete this comment
				// uniform sampler2D_float _CameraDepthTexture;

				uniform sampler2D _MainTex;
				uniform fixed4 _TintColor;
				uniform float4 _MainTex_ST;
				uniform float _InvFade;
				uniform sampler2D _TextureSample0;
				uniform float4 _TextureSample0_ST;
				uniform float2 _VoronoiTile;
				uniform float _Life_Speed_Multiplier;


				v2f vert ( appdata_t v  )
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					

					v.vertex.xyz +=  float3( 0, 0, 0 ) ;
					o.vertex = UnityObjectToClipPos(v.vertex);
					#ifdef SOFTPARTICLES_ON
						o.projPos = ComputeScreenPos (o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);
					#endif
					o.color = v.color;
					o.texcoord = v.texcoord;
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag ( v2f i  ) : SV_Target
				{
					UNITY_SETUP_INSTANCE_ID( i );
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( i );

					#ifdef SOFTPARTICLES_ON
						float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;
						float fade = saturate (_InvFade * (sceneZ-partZ));
						i.color.a *= fade;
					#endif

					float2 uv_TextureSample0 = i.texcoord.xy * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
					float4 tex2DNode23 = tex2D( _TextureSample0, uv_TextureSample0 );
					float3 texCoord11 = i.texcoord.xyz;
					texCoord11.xy = i.texcoord.xyz.xy * _VoronoiTile + float2( 0,0 );
					float clampResult44 = clamp( ( 1.0 - ( texCoord11.z * _Life_Speed_Multiplier ) ) , 0.0 , 1.0 );
					#ifdef _REVERSE_ON
					float staticSwitch41 = clampResult44;
					#else
					float staticSwitch41 = texCoord11.z;
					#endif
					float4 temp_cast_0 = (staticSwitch41).xxxx;
					float4 temp_cast_1 = (( 1.0 - i.color.a )).xxxx;
					float4 temp_output_21_0 = saturate( ( ( (float4( 0,0,0,0 ) + (tex2DNode23 - temp_cast_0) * (float4( 1,1,1,1 ) - float4( 0,0,0,0 )) / (float4( 1,1,1,1 ) - temp_cast_0)) - temp_cast_1 ) * i.color ) );
					

					fixed4 col = temp_output_21_0;
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG 
			}
		}	
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18935
1015.333;314;826;725;-815.4554;268.0057;1.741108;True;False
Node;AmplifyShaderEditor.Vector2Node;9;-1025.374,252.9269;Inherit;False;Property;_VoronoiTile;VoronoiTile;0;0;Create;True;0;0;0;False;0;False;5,5;5,5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;-766.8394,247.1283;Inherit;False;0;-1;3;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;46;-716.5482,475.6057;Inherit;False;Property;_Life_Speed_Multiplier;Life_Speed_Multiplier;3;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-490.4498,386.7167;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;43;-320.6721,363.3908;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;44;-176.3723,416.6908;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;26;733.6317,318.3564;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;23;536.5081,-377.3632;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;0;False;0;False;-1;84242ceb9f8db9d4091714dc65f157c0;84242ceb9f8db9d4091714dc65f157c0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;41;-135.0203,167.3146;Inherit;False;Property;_Reverse;Reverse;2;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;28;968.4404,351.8848;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;13;301.5415,20.24396;Inherit;True;5;0;COLOR;0,0,0,0;False;1;COLOR;0.94,0,0,0;False;2;COLOR;1,1,1,1;False;3;COLOR;0,0,0,0;False;4;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;27;1054.817,138.3193;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;1386.539,202.0933;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;1095.909,-178.7038;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;1099.545,-65.98283;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;1090.202,-279.689;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;16;1434.359,-123.8576;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SaturateNode;21;1548.328,33.77878;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;53;2041.268,-18.67559;Float;False;True;-1;2;ASEMaterialInspector;0;9;CustomFade;0b6a9f8b4f707c74ca64c0be8e590de0;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;True;True;4;1;False;-1;1;False;-1;0;5;False;-1;10;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;True;True;True;True;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;2;False;-1;True;3;False;-1;False;True;4;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;11;0;9;0
WireConnection;45;0;11;3
WireConnection;45;1;46;0
WireConnection;43;1;45;0
WireConnection;44;0;43;0
WireConnection;41;1;11;3
WireConnection;41;0;44;0
WireConnection;28;0;26;4
WireConnection;13;0;23;0
WireConnection;13;1;41;0
WireConnection;27;0;13;0
WireConnection;27;1;28;0
WireConnection;54;0;27;0
WireConnection;54;1;26;0
WireConnection;33;0;23;2
WireConnection;33;1;26;2
WireConnection;34;0;23;3
WireConnection;34;1;26;3
WireConnection;30;0;23;1
WireConnection;30;1;26;1
WireConnection;16;0;30;0
WireConnection;16;1;33;0
WireConnection;16;2;34;0
WireConnection;16;3;21;0
WireConnection;21;0;54;0
WireConnection;53;0;21;0
ASEEND*/
//CHKSM=0041329B8C1776FBCE48F0399ADCA54D12911E09