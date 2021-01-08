Shader "Hidden/VIVisualiser"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Samples ("Samples", Int) = 10
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "VolumetricInteraction.cginc"
            #include "RayMarching.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 view_vector : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
                float3 view_vector = mul(unity_CameraInvProjection, float4(v.uv * 2 - 1, 0, -1));
                o.view_vector = mul(unity_CameraToWorld, float4(view_vector, 0));
                
                return o;
            }

            sampler2D _MainTex;
            int _Samples;

            sampler2D _CameraDepthTexture;

            fixed4 frag (v2f i) : SV_Target
            {
                const float3 max_bounds = float3(0.5, 0.5, 0.5); 
                const float3 min_bounds = float3(-0.5, -0.5, -0.5);
                
                // Find volume intersection points
                const float3 ray_origin = mul(volume_world_to_local, float4(_WorldSpaceCameraPos, 1));
                const float3 ray_direction = normalize(mul(volume_world_to_local, float4(normalize(i.view_vector), 0)));
                
                const float2 box_info = ray_box_distance(min_bounds, max_bounds, ray_origin, ray_direction);
                const float dst_to_box = box_info.x;
                const float dst_in_box = box_info.y;

                const float non_linear_depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                const float depth = LinearEyeDepth(non_linear_depth) * length(mul(volume_world_to_local, float4(i.view_vector, 0)));
                
                // Begin marching
                const float step_size = dst_in_box / _Samples;
                const float dst_limit = min(depth - dst_to_box, dst_in_box);
                
                float dst_travelled = 0;
                float4 sample = float4(0, 0, 0, 0);
                
                while (dst_travelled < dst_limit)
                {
                    const float3 ray_position = ray_origin + ray_direction * (dst_to_box + dst_travelled);
                    const float4 samp = sample_interaction_world(mul(volume_local_to_world, float4(ray_position, 1)));
                    
                    sample = samp.a > sample.a ? samp : sample;

                    dst_travelled += step_size;
                    // sample = float4(1, 1, 1, 1);
                }
                
                fixed4 col = tex2D(_MainTex, i.uv);
                
                return float4(lerp(col.rgb, sample.rgb, sample.a), col.a);
            }
            ENDCG
        }
    }
}
