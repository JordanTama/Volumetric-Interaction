Shader "Unlit/VISampler"
{
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "VolumetricInteraction.cginc"

            struct vert_input
            {
                float4 vertex : POSITION;
            };

            struct frag_input
            {
                float4 vertex : SV_POSITION;
                float3 world_position : TEXCOORD0;
            };

            frag_input vert (vert_input v)
            {
                frag_input o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.world_position = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (frag_input i) : SV_Target
            {
                return get_raw_world(i.world_position);
            }
            
            ENDCG
        }
    }
}
