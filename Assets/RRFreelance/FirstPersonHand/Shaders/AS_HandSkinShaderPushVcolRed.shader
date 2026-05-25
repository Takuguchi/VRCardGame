// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "RRFreelance_Hand/AS_HandSkinShaderPushVColRed"
{
	Properties
	{
		[Header(Translucency)]
		_Translucency("Strength", Range( 0 , 50)) = 1
		_TransNormalDistortion("Normal Distortion", Range( 0 , 1)) = 0.1
		_TransScattering("Scaterring Falloff", Range( 1 , 50)) = 2
		_TransDirect("Direct", Range( 0 , 1)) = 1
		_TransAmbient("Ambient", Range( 0 , 1)) = 0.2
		_TransShadow("Shadow", Range( 0 , 1)) = 0.9
		_Base_Albedo("Base_Albedo", 2D) = "white" {}
		_Base_Tint("Base_Tint", Color) = (1,1,1,0)
		_MetalSmooth("Metal-Smooth", 2D) = "white" {}
		_SmoothnessAdd("SmoothnessAdd", Range( -1 , 1)) = 0
		_NormalMap("NormalMap", 2D) = "bump" {}
		_OptionalMask("OptionalMask", 2D) = "black" {}
		_sssTint("sssTint", Color) = (0,0,0,0)
		_replaceNailTone("replaceNailTone", Range( 0 , 1)) = 0
		_NailToneSmoothAdd("NailTone-SmoothAdd", Color) = (0,0,0,0)
		_PalmToneSmoothMult("PalmTone-SmoothMult", Color) = (0,0,0,0)
		[Toggle]_Invert_TattooColors("Invert_TattooColors", Float) = 0
		_PushVColRedVerts("PushVColRedVerts", Range( -1 , 3)) = 0
		_Tattoo("Tattoo", 2D) = "black" {}
		_TattooTintFadeA("TattooTint-Fade(A)", Color) = (0.07850347,0.07920894,0.08088237,0.909)
		_BloodTexture("BloodTexture", 2D) = "white" {}
		_BloodAmount("BloodAmount", Range( 0 , 1)) = 0
		_AddBloodGloss("AddBloodGloss", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#pragma target 3.0
		#pragma surface surf StandardCustom keepalpha addshadow fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		struct SurfaceOutputStandardCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			half3 Translucency;
		};

		uniform float _PushVColRedVerts;
		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform sampler2D _Base_Albedo;
		uniform float4 _Base_Albedo_ST;
		uniform float4 _Base_Tint;
		uniform sampler2D _OptionalMask;
		uniform float4 _OptionalMask_ST;
		uniform float4 _PalmToneSmoothMult;
		uniform float4 _NailToneSmoothAdd;
		uniform float _replaceNailTone;
		uniform float _Invert_TattooColors;
		uniform sampler2D _Tattoo;
		uniform float4 _Tattoo_ST;
		uniform float4 _TattooTintFadeA;
		uniform sampler2D _BloodTexture;
		uniform float4 _BloodTexture_ST;
		uniform float _BloodAmount;
		uniform sampler2D _MetalSmooth;
		uniform float4 _MetalSmooth_ST;
		uniform float _AddBloodGloss;
		uniform float _SmoothnessAdd;
		uniform half _Translucency;
		uniform half _TransNormalDistortion;
		uniform half _TransScattering;
		uniform half _TransDirect;
		uniform half _TransAmbient;
		uniform half _TransShadow;
		uniform float4 _sssTint;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertexNormal = v.normal.xyz;
			v.vertex.xyz += ( ( ( v.color.r * _PushVColRedVerts ) / 500.0 ) * ase_vertexNormal );
		}

		inline half4 LightingStandardCustom(SurfaceOutputStandardCustom s, half3 viewDir, UnityGI gi )
		{
			#if !DIRECTIONAL
			float3 lightAtten = gi.light.color;
			#else
			float3 lightAtten = lerp( _LightColor0.rgb, gi.light.color, _TransShadow );
			#endif
			half3 lightDir = gi.light.dir + s.Normal * _TransNormalDistortion;
			half transVdotL = pow( saturate( dot( viewDir, -lightDir ) ), _TransScattering );
			half3 translucency = lightAtten * (transVdotL * _TransDirect + gi.indirect.diffuse * _TransAmbient) * s.Translucency;
			half4 c = half4( s.Albedo * translucency * _Translucency, 0 );

			SurfaceOutputStandard r;
			r.Albedo = s.Albedo;
			r.Normal = s.Normal;
			r.Emission = s.Emission;
			r.Metallic = s.Metallic;
			r.Smoothness = s.Smoothness;
			r.Occlusion = s.Occlusion;
			r.Alpha = s.Alpha;
			return LightingStandard (r, viewDir, gi) + c;
		}

		inline void LightingStandardCustom_GI(SurfaceOutputStandardCustom s, UnityGIInput data, inout UnityGI gi )
		{
			#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
				gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal);
			#else
				UNITY_GLOSSY_ENV_FROM_SURFACE( g, s, data );
				gi = UnityGlobalIllumination( data, s.Occlusion, s.Normal, g );
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandardCustom o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			o.Normal = UnpackNormal( tex2D( _NormalMap, uv_NormalMap ) );
			float2 uv_Base_Albedo = i.uv_texcoord * _Base_Albedo_ST.xy + _Base_Albedo_ST.zw;
			float4 tex2DNode1 = tex2D( _Base_Albedo, uv_Base_Albedo );
			float2 uv_OptionalMask = i.uv_texcoord * _OptionalMask_ST.xy + _OptionalMask_ST.zw;
			float4 tex2DNode7 = tex2D( _OptionalMask, uv_OptionalMask );
			float4 temp_output_20_0 = ( tex2DNode7.b * _PalmToneSmoothMult * tex2DNode1 );
			float4 lerpResult19 = lerp( ( tex2DNode1 * _Base_Tint ) , ( tex2DNode1 + temp_output_20_0 + temp_output_20_0 ) , tex2DNode7.b);
			float4 temp_cast_0 = (0.5).xxxx;
			float4 lerpResult21 = lerp( lerpResult19 , ( tex2DNode7.g * _NailToneSmoothAdd * pow( tex2DNode1 , temp_cast_0 ) ) , ( tex2DNode7.g * _replaceNailTone ));
			float2 uv_Tattoo = i.uv_texcoord * _Tattoo_ST.xy + _Tattoo_ST.zw;
			float4 tex2DNode37 = tex2D( _Tattoo, uv_Tattoo );
			float4 lerpResult36 = lerp( lerpResult21 , ( (( _Invert_TattooColors )?( ( 1.0 - tex2DNode37 ) ):( tex2DNode37 )) * _TattooTintFadeA ) , ( (( _Invert_TattooColors )?( ( 1.0 - tex2DNode37 ) ):( tex2DNode37 )) * _TattooTintFadeA.a ));
			float2 uv_BloodTexture = i.uv_texcoord * _BloodTexture_ST.xy + _BloodTexture_ST.zw;
			float4 tex2DNode43 = tex2D( _BloodTexture, uv_BloodTexture );
			float4 lerpResult45 = lerp( lerpResult36 , tex2DNode43 , ( tex2DNode43.a * _BloodAmount ));
			o.Albedo = lerpResult45.rgb;
			float2 uv_MetalSmooth = i.uv_texcoord * _MetalSmooth_ST.xy + _MetalSmooth_ST.zw;
			float4 tex2DNode4 = tex2D( _MetalSmooth, uv_MetalSmooth );
			o.Metallic = tex2DNode4.r;
			float clampResult24 = clamp( ( ( tex2DNode43.a * _AddBloodGloss * _BloodAmount ) + ( ( tex2DNode7.b * _PalmToneSmoothMult.a * _PalmToneSmoothMult.a ) + ( tex2DNode7.g * _NailToneSmoothAdd.a ) + ( tex2DNode4.a + _SmoothnessAdd ) ) ) , 0.0 , 1.0 );
			o.Smoothness = clampResult24;
			o.Translucency = ( tex2DNode7.r * _sssTint ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17800
1920;24;1920;1004;-5.633504;-117.7541;1.796731;True;True
Node;AmplifyShaderEditor.ColorNode;15;-493.4711,611.4734;Float;False;Property;_PalmToneSmoothMult;PalmTone-SmoothMult;16;0;Create;True;0;0;False;0;0,0,0,0;0.3235294,0.2723422,0.266436,0.096;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;7;-559.9107,403.2025;Inherit;True;Property;_OptionalMask;OptionalMask;12;0;Create;True;0;0;False;0;-1;None;768ccbcf836c44446a567a06a753319d;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-664.7747,-336.2899;Inherit;True;Property;_Base_Albedo;Base_Albedo;7;0;Create;True;0;0;False;0;-1;None;42d7d2dc72a17674d8c7465fe85f3ea1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;93.06075,339.6524;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;37;384.8344,-588.5651;Inherit;True;Property;_Tattoo;Tattoo;19;0;Create;True;0;0;False;0;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;2;-468.2296,-104.9784;Float;False;Property;_Base_Tint;Base_Tint;8;0;Create;True;0;0;False;0;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;34;-2.75795,960.9005;Float;False;Constant;_Float0;Float 0;10;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;33;172.8594,868.3171;Inherit;False;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;41;459.954,162.0128;Float;False;Property;_replaceNailTone;replaceNailTone;14;0;Create;True;0;0;False;0;0;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;52;827.3218,-438.8076;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;28;320.7151,238.4878;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;10;-493.712,986.5991;Float;False;Property;_NailToneSmoothAdd;NailTone-SmoothAdd;15;0;Create;True;0;0;False;0;0,0,0,0;1,0.02068997,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-56.37962,-212.5566;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;4;-554.7065,70.14758;Inherit;True;Property;_MetalSmooth;Metal-Smooth;9;0;Create;True;0;0;False;0;-1;None;b05552131450365489c69db56720fb27;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;5;-533.8904,280.0416;Float;False;Property;_SmoothnessAdd;SmoothnessAdd;10;0;Create;True;0;0;False;0;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;57;1780.447,744.3937;Inherit;False;Property;_PushVColRedVerts;PushVColRedVerts;18;0;Create;True;0;0;False;0;0;0;-1;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;46;1547.239,131.3274;Float;False;Property;_BloodAmount;BloodAmount;22;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;23;-109.0783,201.5849;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;707.15,89.86047;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;44;1540.368,233.4861;Float;False;Property;_AddBloodGloss;AddBloodGloss;23;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;53;1857.138,585.5475;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ToggleSwitchNode;50;881.0707,-576.7609;Float;False;Property;_Invert_TattooColors;Invert_TattooColors;17;0;Create;True;0;0;False;0;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;38;456.0923,-300.4926;Float;False;Property;_TattooTintFadeA;TattooTint-Fade(A);20;0;Create;True;0;0;False;0;0.07850347,0.07920894,0.08088237,0.909;0.2138841,0.2442191,0.3161765,0.872;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;43;1509.569,-81.39529;Inherit;True;Property;_BloodTexture;BloodTexture;21;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;181.1305,1057.461;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;19;403.0866,-68.24194;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;450.8839,442.6782;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;397.7692,661.102;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;2085.35,638.3405;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;1866.14,269.6754;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;27;841.7479,408.3146;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;1270.687,-486.65;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;1273.873,-377.6393;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;21;842.415,-88.24806;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;2207.34,96.19354;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;36;1605.792,-366.6876;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalVertexDataNode;54;2204.085,794.549;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;49;2090.994,382.0895;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;8;-487.671,794.7775;Float;False;Property;_sssTint;sssTint;13;0;Create;True;0;0;False;0;0,0,0,0;1,0.4558824,0.4558824,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;58;2251.791,679.5834;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;500;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;18;893.6025,138.5529;Inherit;True;Property;_NormalMap;NormalMap;11;0;Create;True;0;0;False;0;-1;None;3af75e3549193544b86b12a379f2a490;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;24;2311.307,406.9689;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;45;2612.929,-34.09534;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;2452.117,720.8264;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;123.8485,523.403;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2800.837,327.2232;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;RRFreelance_Hand/AS_HandSkinShaderPushVColRed;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;0;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;20;0;7;3
WireConnection;20;1;15;0
WireConnection;20;2;1;0
WireConnection;33;0;1;0
WireConnection;33;1;34;0
WireConnection;52;0;37;0
WireConnection;28;0;1;0
WireConnection;28;1;20;0
WireConnection;28;2;20;0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;23;0;4;4
WireConnection;23;1;5;0
WireConnection;42;0;7;2
WireConnection;42;1;41;0
WireConnection;50;0;37;0
WireConnection;50;1;52;0
WireConnection;26;0;7;2
WireConnection;26;1;10;4
WireConnection;19;0;3;0
WireConnection;19;1;28;0
WireConnection;19;2;7;3
WireConnection;25;0;7;3
WireConnection;25;1;15;4
WireConnection;25;2;15;4
WireConnection;22;0;7;2
WireConnection;22;1;10;0
WireConnection;22;2;33;0
WireConnection;56;0;53;1
WireConnection;56;1;57;0
WireConnection;48;0;43;4
WireConnection;48;1;44;0
WireConnection;48;2;46;0
WireConnection;27;0;25;0
WireConnection;27;1;26;0
WireConnection;27;2;23;0
WireConnection;40;0;50;0
WireConnection;40;1;38;4
WireConnection;39;0;50;0
WireConnection;39;1;38;0
WireConnection;21;0;19;0
WireConnection;21;1;22;0
WireConnection;21;2;42;0
WireConnection;47;0;43;4
WireConnection;47;1;46;0
WireConnection;36;0;21;0
WireConnection;36;1;39;0
WireConnection;36;2;40;0
WireConnection;49;0;48;0
WireConnection;49;1;27;0
WireConnection;58;0;56;0
WireConnection;24;0;49;0
WireConnection;45;0;36;0
WireConnection;45;1;43;0
WireConnection;45;2;47;0
WireConnection;55;0;58;0
WireConnection;55;1;54;0
WireConnection;11;0;7;1
WireConnection;11;1;8;0
WireConnection;0;0;45;0
WireConnection;0;1;18;0
WireConnection;0;3;4;0
WireConnection;0;4;24;0
WireConnection;0;7;11;0
WireConnection;0;11;55;0
ASEEND*/
//CHKSM=D28C8EAF58213EB18B2B80E0043FDA5C143D416F