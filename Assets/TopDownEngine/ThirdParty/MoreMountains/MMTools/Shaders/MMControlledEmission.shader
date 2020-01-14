// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MoreMountains/MMControlledEmission"
{
	Properties
	{
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_DiffuseColor("DiffuseColor", Color) = (1,1,1,1)
		_Opacity("Opacity", Range( 0 , 1)) = 1
		[HDR]_EmissionColor("EmissionColor", Color) = (1,1,1,1)
		_EmissionForce("EmissionForce", Float) = 0
		[Toggle(_USEEMISSIONFRESNEL_ON)] _UseEmissionFresnel("UseEmissionFresnel", Float) = 0
		_EmissionFresnelBias("EmissionFresnelBias", Float) = 1
		_EmissionFresnelScale("EmissionFresnelScale", Float) = 1
		_EmissionFresnelPower("EmissionFresnelPower", Float) = 1
		[Toggle(_USEOPACITYFRESNEL_ON)] _UseOpacityFresnel("UseOpacityFresnel", Float) = 0
		[Toggle(_INVERTOPACITYFRESNEL_ON)] _InvertOpacityFresnel("InvertOpacityFresnel", Float) = 0
		_OpacityFresnelBias("OpacityFresnelBias", Float) = 1
		_OpacityFresnelScale("OpacityFresnelScale", Float) = 1
		_OpacityFresnelPower("OpacityFresnelPower", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature _USEEMISSIONFRESNEL_ON
		#pragma shader_feature _USEOPACITYFRESNEL_ON
		#pragma shader_feature _INVERTOPACITYFRESNEL_ON
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
		};

		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
		uniform float4 _DiffuseColor;
		uniform float _EmissionForce;
		uniform float4 _EmissionColor;
		uniform float _EmissionFresnelBias;
		uniform float _EmissionFresnelScale;
		uniform float _EmissionFresnelPower;
		uniform float _OpacityFresnelBias;
		uniform float _OpacityFresnelScale;
		uniform float _OpacityFresnelPower;
		uniform float _Opacity;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			o.Albedo = ( tex2D( _TextureSample0, uv_TextureSample0 ) * _DiffuseColor ).rgb;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV8 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode8 = ( _EmissionFresnelBias + _EmissionFresnelScale * pow( 1.0 - fresnelNdotV8, _EmissionFresnelPower ) );
			#ifdef _USEEMISSIONFRESNEL_ON
				float staticSwitch22 = fresnelNode8;
			#else
				float staticSwitch22 = 1.0;
			#endif
			o.Emission = ( _EmissionForce * _EmissionColor * staticSwitch22 ).rgb;
			float fresnelNdotV26 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode26 = ( _OpacityFresnelBias + _OpacityFresnelScale * pow( 1.0 - fresnelNdotV26, _OpacityFresnelPower ) );
			#ifdef _INVERTOPACITYFRESNEL_ON
				float staticSwitch31 = ( 1.0 - fresnelNode26 );
			#else
				float staticSwitch31 = fresnelNode26;
			#endif
			#ifdef _USEOPACITYFRESNEL_ON
				float staticSwitch27 = staticSwitch31;
			#else
				float staticSwitch27 = 1.0;
			#endif
			o.Alpha = ( staticSwitch27 * _Opacity * _DiffuseColor.a );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	//CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16800
-1852;173;1776;1039;2802.161;1434.069;2.460439;True;True
Node;AmplifyShaderEditor.RangedFloatNode;24;-2470.223,930.1887;Float;False;Property;_OpacityFresnelScale;OpacityFresnelScale;12;0;Create;True;0;0;False;0;1;0.65;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-2470.223,1014.619;Float;False;Property;_OpacityFresnelPower;OpacityFresnelPower;13;0;Create;True;0;0;False;0;1;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-2464.192,839.7272;Float;False;Property;_OpacityFresnelBias;OpacityFresnelBias;11;0;Create;True;0;0;False;0;1;-0.27;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;26;-2158.3,895.4371;Float;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;12.06;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;32;-1698.893,1175.237;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-2719.817,333.9998;Float;False;Property;_EmissionFresnelBias;EmissionFresnelBias;6;0;Create;True;0;0;False;0;1;-0.47;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-2725.848,424.4613;Float;False;Property;_EmissionFresnelScale;EmissionFresnelScale;7;0;Create;True;0;0;False;0;1;0.65;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-2725.848,508.8918;Float;False;Property;_EmissionFresnelPower;EmissionFresnelPower;8;0;Create;True;0;0;False;0;1;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;31;-1478.648,1000.029;Float;False;Property;_InvertOpacityFresnel;InvertOpacityFresnel;10;0;Create;True;0;0;False;0;0;0;1;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-1375.554,727.4433;Float;False;Constant;_NoFresnel;NoFresnel;13;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;8;-2413.925,389.7098;Float;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;12.06;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-2336.864,300.8306;Float;False;Constant;_NoEmissionFresnel;NoEmissionFresnel;13;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;27;-1043.862,835.997;Float;False;Property;_UseOpacityFresnel;UseOpacityFresnel;9;0;Create;True;0;0;False;0;0;0;1;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;22;-1860.434,338.449;Float;False;Property;_UseEmissionFresnel;UseEmissionFresnel;5;0;Create;True;0;0;False;0;0;0;1;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-1239.096,-478.1724;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;4;-874.2047,983.3147;Float;False;Property;_Opacity;Opacity;2;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;2;-1161.683,-247.8651;Float;False;Property;_DiffuseColor;DiffuseColor;1;0;Create;True;0;0;False;0;1,1,1,1;0,0.6309547,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;5;-2068.066,109.8726;Float;False;Property;_EmissionColor;EmissionColor;3;1;[HDR];Create;True;0;0;True;0;1,1,1,1;0,5.008955,7.906699,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;6;-1776.588,-46.1496;Float;False;Property;_EmissionForce;EmissionForce;4;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-789.0964,-308.1723;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-1251.327,196.7999;Float;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-414.7378,320.1205;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;MoreMountains/MMControlledEmission;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;5;True;True;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;5;True;2;5;False;-1;10;False;-1;2;5;False;-1;10;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;26;1;23;0
WireConnection;26;2;24;0
WireConnection;26;3;25;0
WireConnection;32;0;26;0
WireConnection;31;1;26;0
WireConnection;31;0;32;0
WireConnection;8;1;14;0
WireConnection;8;2;12;0
WireConnection;8;3;11;0
WireConnection;27;1;29;0
WireConnection;27;0;31;0
WireConnection;22;1;30;0
WireConnection;22;0;8;0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;10;0;6;0
WireConnection;10;1;5;0
WireConnection;10;2;22;0
WireConnection;13;0;27;0
WireConnection;13;1;4;0
WireConnection;13;2;2;4
WireConnection;0;0;3;0
WireConnection;0;2;10;0
WireConnection;0;9;13;0
ASEEND*/
//CHKSM=F6CAE4DEFCAE0AD286D5257702CCB0DE088EC73D