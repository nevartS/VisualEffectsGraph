Shader "HiGames/Particles/AddCenterGlow"
{
    Properties
    {
        _MainTex("MainTexture", 2D) = "white" {}
        _NoiseTex("NoiseTexture", 2D) = "white" {}
        _FlowTex("FlowTexture", 2D) = "white" {}
        _Mask("Mask", 2D) = "white" {}
        _MainTexSpeedUV_NoiseZ("MainTexSpeedUV_NoiseZ", Vector) = (0,0,0,0)
        _DistortionSpeedXY_PowerZ("DistortionSpeedXY_PowerZ", Vector) = (0,0,0,0)
        _Emission("Emission", Float) = 2
        _Color("Color", Color) = (0.5,0.5,0.5,1)
        [Toggle]_CenterGlow("CenterGlow", Float) = 0
        [MaterialToggle]_UseDepth("UseDepth", Float) = 0
        _DepthPower("DepthPower", Float) = 1
        [Enum(Cull Off,0, Cull Front,1, Cull Back,2)]_CullingMode("CullingMode", Float) = 0
        [Enum(One,1,OneMinusSrcAlpha,6)] _BlendModeSubset("BlendModeSubste", Float) = 1
        [HideInInspector] _TexCoord("", 2D) = "white" {}
    }
    
    Category
    {
        SubShader
        {
            Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane"}
            Blend One[_BlendModeSubset]
            ColorMask RGB
            Cull[_CullingMode]
            Lighting Off
            ZWrite Off
            ZTest LEqual
            
            Pass
            {
                HLSLPROGRAM

                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0
                #pragma multi_compile_particles
                #pragma multi_compile_fog
                #include "UnityShaderVariables.cginc"
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

                #if UNITY_VERSION >= 500
                UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
                #else
                uniform sampler2D_float _CameraDepthTexture;
                #endif

                //uniform sampler2D_float _CameraDepthTexture;

                uniform sampler2D _MainTex;
                uniform float4 _MainTex_ST;
                uniform float _CenterGlow;
                uniform float4 _MainTexSpeedUV_NoiseZ;
                uniform sampler2D _FlowTex;
                uniform float4 _FlowTex_ST;
                uniform float4 _DistortionSpeedXY_PowerZ;
                uniform sampler2D _Mask;
                uniform float4 _Mask_ST;
                uniform sampler2D _NoiseTex;
                uniform float4 _NoiseTex_ST;
                uniform float4 _Color;
                uniform float _Emission;
                uniform fixed _UseDepth;
                uniform float _DepthPower;

                v2f vert (appdata_t v)
                {
                    v2f o;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                    UNITY_TRANSFER_INSTANCE_ID(v, o);

                    v.vertex.xyz += float3 (0,0,0);
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

                fixed4 frag ( v2f i ) : SV_Target
                {
                    float lp = 1;
					#ifdef SOFTPARTICLES_ON
						float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;
						float fade = saturate ((sceneZ-partZ) / _Depthpower);
						lp *= lerp(1, fade, _Usedepth);
						i.color.a *= lp;
					#endif

					float2 appendResult21 = (float2(_MainTexSpeedUV_NoiseZ.x , _MainTexSpeedUV_NoiseZ.y));
					float2 uv0_MainTex = i.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					float2 panner107 = ( 1.0 * _Time.y * appendResult21 + uv0_MainTex);
					float2 appendResult100 = (float2(_DistortionSpeedXY_PowerZ.x , _DistortionSpeedXY_PowerZ.y));
					float3 uv0_Flow = i.texcoord.xyz;
					uv0_Flow.xy = i.texcoord.xy * _FlowTex_ST.xy + _FlowTex_ST.zw;
					float2 panner110 = ( 1.0 * _Time.y * appendResult100 + (uv0_Flow).xy);
					float2 uv_Mask = i.texcoord.xy * _Mask_ST.xy + _Mask_ST.zw;
					float4 tex2DNode33 = tex2D( _Mask, uv_Mask );
					float Flowpower102 = _DistortionSpeedXY_PowerZ.z;
					float4 tex2DNode13 = tex2D( _MainTex, ( panner107 - ( (( tex2D( _FlowTex, panner110 ) * tex2DNode33 )).rg * Flowpower102 ) ) );
					float2 appendResult22 = (float2(_MainTexSpeedUV_NoiseZ.z , _MainTexSpeedUV_NoiseZ.w));
					float2 uv0_Noise = i.texcoord.xy * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
					float2 panner108 = ( 1.0 * _Time.y * appendResult22 + uv0_Noise);
					float4 tex2DNode14 = tex2D( _NoiseTex, panner108 );
					float4 temp_output_30_0 = ( tex2DNode13 * tex2DNode14 * _Color * i.color * tex2DNode13.a * tex2DNode14.a * _Color.a * i.color.a );
					float4 temp_cast_0 = ((1.0 + (uv0_Flow.z - 0.0) * (0.0 - 1.0) / (1.0 - 0.0))).xxxx;
					float4 clampResult38 = clamp( ( tex2DNode33 - temp_cast_0 ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
					float4 clampResult40 = clamp( ( tex2DNode33 * clampResult38 ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
					

					fixed4 col = ( lerp(temp_output_30_0,( temp_output_30_0 * clampResult40 ),_CenterGlow) * _Emission );
					UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,1));
					return col;
                }
                
                ENDHLSL
            }
        }
    }
}