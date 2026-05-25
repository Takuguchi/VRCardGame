// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "RRFreelance_Hand/AS_HandSkinAnimUI"
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
		_Tattoo("Tattoo", 2D) = "black" {}
		_TattooTintFadeA("TattooTint-Fade(A)", Color) = (0.07850347,0.07920894,0.08088237,0.909)
		_BloodTexture("BloodTexture", 2D) = "white" {}
		_AddBloodGloss("AddBloodGloss", Range( 0 , 1)) = 0
		_BloodAmount("BloodAmount", Range( 0 , 1)) = 0
		[NoScaleOffset]_UIAnimSheet("UIAnimSheet", 2D) = "white" {}
		_AnimationMaskArea("AnimationMaskArea", 2D) = "white" {}
		_UI_Angle("UI_Angle", Range( 0 , 360)) = 0
		_UIEmissive("UIEmissive", Range( 0 , 2)) = 0
		_AnimMaskPower("AnimMaskPower", Range( 0 , 1)) = 1
		_Anim_Rows("Anim_Rows", Float) = 8
		_Anim_Columns("Anim_Columns", Float) = 8
		_Frame("Frame", Float) = 10
		_Anim_Speed("Anim_Speed", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#pragma target 3.0
		#pragma surface surf StandardCustom keepalpha addshadow fullforwardshadows exclude_path:deferred 
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
		uniform sampler2D _UIAnimSheet;
		uniform float _Anim_Columns;
		uniform float _Anim_Rows;
		uniform float _Anim_Speed;
		uniform float _Frame;
		uniform float _UI_Angle;
		uniform sampler2D _AnimationMaskArea;
		uniform float4 _AnimationMaskArea_ST;
		uniform float _AnimMaskPower;
		uniform float _UIEmissive;
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
			float4 BaseAlbedo83 = lerpResult45;
			// *** BEGIN Flipbook UV Animation vars ***
			// Total tiles of Flipbook Texture
			float fbtotaltiles54 = _Anim_Columns * _Anim_Rows;
			// Offsets for cols and rows of Flipbook Texture
			float fbcolsoffset54 = 1.0f / _Anim_Columns;
			float fbrowsoffset54 = 1.0f / _Anim_Rows;
			// Speed of animation
			float fbspeed54 = _Time[ 1 ] * _Anim_Speed;
			// UV Tiling (col and row offset)
			float2 fbtiling54 = float2(fbcolsoffset54, fbrowsoffset54);
			// UV Offset - calculate current tile linear index, and convert it to (X * coloffset, Y * rowoffset)
			// Calculate current tile linear index
			float fbcurrenttileindex54 = round( fmod( fbspeed54 + _Frame, fbtotaltiles54) );
			fbcurrenttileindex54 += ( fbcurrenttileindex54 < 0) ? fbtotaltiles54 : 0;
			// Obtain Offset X coordinate from current tile linear index
			float fblinearindextox54 = round ( fmod ( fbcurrenttileindex54, _Anim_Columns ) );
			// Multiply Offset X by coloffset
			float fboffsetx54 = fblinearindextox54 * fbcolsoffset54;
			// Obtain Offset Y coordinate from current tile linear index
			float fblinearindextoy54 = round( fmod( ( fbcurrenttileindex54 - fblinearindextox54 ) / _Anim_Columns, _Anim_Rows ) );
			// Reverse Y to get tiles from Top to Bottom
			fblinearindextoy54 = (int)(_Anim_Rows-1) - fblinearindextoy54;
			// Multiply Offset Y by rowoffset
			float fboffsety54 = fblinearindextoy54 * fbrowsoffset54;
			// UV Offset
			float2 fboffset54 = float2(fboffsetx54, fboffsety54);
			// Flipbook UV
			half2 fbuv54 = i.uv_texcoord * fbtiling54 + fboffset54;
			// *** END Flipbook UV Animation vars ***
			float cos58 = cos( _UI_Angle );
			float sin58 = sin( _UI_Angle );
			float2 rotator58 = mul( fbuv54 - float2( 0,0 ) , float2x2( cos58 , -sin58 , sin58 , cos58 )) + float2( 0,0 );
			float4 tex2DNode68 = tex2D( _UIAnimSheet, rotator58 );
			float2 uv_AnimationMaskArea = i.uv_texcoord * _AnimationMaskArea_ST.xy + _AnimationMaskArea_ST.zw;
			float3 temp_cast_1 = (tex2D( _AnimationMaskArea, uv_AnimationMaskArea ).r).xxx;
			float temp_output_2_0_g2 = _AnimMaskPower;
			float temp_output_3_0_g2 = ( 1.0 - temp_output_2_0_g2 );
			float3 appendResult7_g2 = (float3(temp_output_3_0_g2 , temp_output_3_0_g2 , temp_output_3_0_g2));
			float3 temp_output_76_0 = ( ( ( temp_cast_1 * temp_output_2_0_g2 ) + appendResult7_g2 ) * tex2DNode68.a );
			float4 lerpResult85 = lerp( BaseAlbedo83 , tex2DNode68 , float4( temp_output_76_0 , 0.0 ));
			o.Albedo = lerpResult85.rgb;
			float4 lerpResult86 = lerp( float4( 0,0,0,0 ) , ( tex2DNode68 * tex2DNode68.a ) , _UIEmissive);
			o.Emission = ( float4( temp_output_76_0 , 0.0 ) * lerpResult86 ).rgb;
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
1920;24;1920;1004;-2585.105;1467.767;1;True;True
Node;AmplifyShaderEditor.SamplerNode;1;-664.7747,-336.2899;Inherit;True;Property;_Base_Albedo;Base_Albedo;7;0;Create;True;0;0;False;0;-1;None;42d7d2dc72a17674d8c7465fe85f3ea1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;15;-493.4711,611.4734;Float;False;Property;_PalmToneSmoothMult;PalmTone-SmoothMult;16;0;Create;True;0;0;False;0;0,0,0,0;0.05109212,0.05122001,0.05147058,0.247;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;7;-559.9107,403.2025;Inherit;True;Property;_OptionalMask;OptionalMask;12;0;Create;True;0;0;False;0;-1;None;768ccbcf836c44446a567a06a753319d;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;34;-2.75795,960.9005;Float;False;Constant;_Float0;Float 0;10;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;93.06075,339.6524;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;37;384.8344,-588.5651;Inherit;True;Property;_Tattoo;Tattoo;18;0;Create;True;0;0;False;0;-1;None;0fa61d2432f6a874ab41877694cda37c;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;2;-468.2296,-104.9784;Float;False;Property;_Base_Tint;Base_Tint;8;0;Create;True;0;0;False;0;1,1,1,0;1,0.9077079,0.8088235,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;33;172.8594,868.3171;Inherit;False;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;28;320.7151,238.4878;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;10;-493.712,986.5991;Float;False;Property;_NailToneSmoothAdd;NailTone-SmoothAdd;15;0;Create;True;0;0;False;0;0,0,0,0;0.9411765,0.9065744,0.9065744,0.303;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-56.37962,-212.5566;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;52;827.3218,-438.8076;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;41;459.954,162.0128;Float;False;Property;_replaceNailTone;replaceNailTone;14;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;77;1724.578,-658.5042;Inherit;False;Property;_Frame;Frame;31;0;Create;True;0;0;False;0;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;397.7692,661.102;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;71;1659.951,-894.8022;Inherit;False;Property;_Anim_Columns;Anim_Columns;30;0;Create;True;0;0;False;0;8;8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;38;456.0923,-300.4926;Float;False;Property;_TattooTintFadeA;TattooTint-Fade(A);19;0;Create;True;0;0;False;0;0.07850347,0.07920894,0.08088237,0.909;0.01427337,0.0146701,0.0147059,0.716;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;707.15,89.86047;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;90;1629.743,-1045.139;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;19;403.0866,-68.24194;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;61;1935.867,-798.623;Inherit;False;994.4001;527.6;Angle and position // scale?;3;60;54;58;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;70;1680.251,-817.0019;Inherit;False;Property;_Anim_Rows;Anim_Rows;29;0;Create;True;0;0;False;0;8;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;72;1696.052,-724.0024;Inherit;False;Property;_Anim_Speed;Anim_Speed;32;0;Create;True;0;0;False;0;1;8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;50;881.0707,-576.7609;Float;False;Property;_Invert_TattooColors;Invert_TattooColors;17;0;Create;True;0;0;False;0;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCFlipBookUVAnimation;54;1964.164,-891.1579;Inherit;False;0;0;6;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;60;1921.002,-415.8456;Inherit;False;Property;_UI_Angle;UI_Angle;25;0;Create;True;0;0;False;0;0;0;0;360;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;1273.873,-377.6393;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;43;1509.569,-81.39529;Inherit;True;Property;_BloodTexture;BloodTexture;20;0;Create;True;0;0;False;0;-1;None;c346fbba5f4b05742b647b03a6968d6c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;46;1547.239,131.3274;Float;False;Property;_BloodAmount;BloodAmount;22;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;1270.687,-486.65;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;21;842.415,-88.24806;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-533.8904,280.0416;Float;False;Property;_SmoothnessAdd;SmoothnessAdd;10;0;Create;True;0;0;False;0;0;-0.05;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;2207.34,96.19354;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;36;1605.792,-366.6876;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;4;-554.7065,70.14758;Inherit;True;Property;_MetalSmooth;Metal-Smooth;9;0;Create;True;0;0;False;0;-1;None;b05552131450365489c69db56720fb27;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RotatorNode;58;2315.862,-565.6729;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;53;2968.459,-512.7802;Inherit;False;1010.927;527.8618;UI effect;1;68;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;75;3446.675,-711.0413;Inherit;True;Property;_AnimationMaskArea;AnimationMaskArea;24;0;Create;True;0;0;False;0;-1;5855108d35ed3654eb38d10e716b66ce;5855108d35ed3654eb38d10e716b66ce;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;45;2580.844,-130.3521;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;23;-109.0783,201.5849;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;68;3452.473,-481.3406;Inherit;True;Property;_UIAnimSheet;UIAnimSheet;23;1;[NoScaleOffset];Create;True;0;0;False;0;-1;a419be70b58de6c4383157a02c94188c;a419be70b58de6c4383157a02c94188c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;450.8839,442.6782;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;80;3461.978,-817.0549;Inherit;False;Property;_AnimMaskPower;AnimMaskPower;28;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;44;1540.368,233.4861;Float;False;Property;_AddBloodGloss;AddBloodGloss;21;0;Create;True;0;0;False;0;0;0.715;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;181.1305,1057.461;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;83;2818.24,-80.27429;Inherit;False;BaseAlbedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;27;841.7479,408.3146;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;89;3919.284,-160.5748;Inherit;False;Property;_UIEmissive;UIEmissive;26;0;Create;True;0;0;False;0;0;1.182;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;3890.928,-272.1063;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;1866.14,269.6754;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;81;3790.363,-839.0693;Inherit;False;Lerp White To;-1;;2;047d7c189c36a62438973bad9d37b1c2;0;2;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;49;2090.994,382.0895;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;3913.47,-642.116;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;84;4101.662,-715.1108;Inherit;False;83;BaseAlbedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;8;-487.671,794.7775;Float;False;Property;_sssTint;sssTint;13;0;Create;True;0;0;False;0;0,0,0,0;0.5367647,0.2723292,0.2723292,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;86;4241.359,-144.2964;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;85;4359.048,-493.6387;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;18;893.6025,138.5529;Inherit;True;Property;_NormalMap;NormalMap;11;0;Create;True;0;0;False;0;-1;None;3af75e3549193544b86b12a379f2a490;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;93;1392.784,-956.2793;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;94;4495.431,-159.4898;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;123.8485,523.403;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;92;1395.385,-1051.179;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ClampOpNode;24;2311.307,406.9689;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;91;1075.585,-1075.879;Inherit;False;Property;_TilingAndOffset;TilingAndOffset;27;0;Create;True;0;0;False;0;1,1,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;4755.653,-69.11131;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;RRFreelance_Hand/AS_HandSkinAnimUI;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;0;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;20;0;7;3
WireConnection;20;1;15;0
WireConnection;20;2;1;0
WireConnection;33;0;1;0
WireConnection;33;1;34;0
WireConnection;28;0;1;0
WireConnection;28;1;20;0
WireConnection;28;2;20;0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;52;0;37;0
WireConnection;22;0;7;2
WireConnection;22;1;10;0
WireConnection;22;2;33;0
WireConnection;42;0;7;2
WireConnection;42;1;41;0
WireConnection;19;0;3;0
WireConnection;19;1;28;0
WireConnection;19;2;7;3
WireConnection;50;0;37;0
WireConnection;50;1;52;0
WireConnection;54;0;90;0
WireConnection;54;1;71;0
WireConnection;54;2;70;0
WireConnection;54;3;72;0
WireConnection;54;4;77;0
WireConnection;39;0;50;0
WireConnection;39;1;38;0
WireConnection;40;0;50;0
WireConnection;40;1;38;4
WireConnection;21;0;19;0
WireConnection;21;1;22;0
WireConnection;21;2;42;0
WireConnection;47;0;43;4
WireConnection;47;1;46;0
WireConnection;36;0;21;0
WireConnection;36;1;39;0
WireConnection;36;2;40;0
WireConnection;58;0;54;0
WireConnection;58;2;60;0
WireConnection;45;0;36;0
WireConnection;45;1;43;0
WireConnection;45;2;47;0
WireConnection;23;0;4;4
WireConnection;23;1;5;0
WireConnection;68;1;58;0
WireConnection;25;0;7;3
WireConnection;25;1;15;4
WireConnection;25;2;15;4
WireConnection;26;0;7;2
WireConnection;26;1;10;4
WireConnection;83;0;45;0
WireConnection;27;0;25;0
WireConnection;27;1;26;0
WireConnection;27;2;23;0
WireConnection;87;0;68;0
WireConnection;87;1;68;4
WireConnection;48;0;43;4
WireConnection;48;1;44;0
WireConnection;48;2;46;0
WireConnection;81;1;75;1
WireConnection;81;2;80;0
WireConnection;49;0;48;0
WireConnection;49;1;27;0
WireConnection;76;0;81;0
WireConnection;76;1;68;4
WireConnection;86;1;87;0
WireConnection;86;2;89;0
WireConnection;85;0;84;0
WireConnection;85;1;68;0
WireConnection;85;2;76;0
WireConnection;93;0;91;3
WireConnection;93;1;91;4
WireConnection;94;0;76;0
WireConnection;94;1;86;0
WireConnection;11;0;7;1
WireConnection;11;1;8;0
WireConnection;92;0;91;1
WireConnection;92;1;91;2
WireConnection;24;0;49;0
WireConnection;0;0;85;0
WireConnection;0;1;18;0
WireConnection;0;2;94;0
WireConnection;0;3;4;0
WireConnection;0;4;24;0
WireConnection;0;7;11;0
ASEEND*/
//CHKSM=F8A1C1F039586EFFE0083E7E6CCAFF5C6B85B1A1