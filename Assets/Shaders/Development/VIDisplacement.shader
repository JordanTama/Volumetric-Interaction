Shader "Volumetric Interaction/VIDisplacement"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _DisplacementColor ("Displacement Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Displacement ("Displacement Amount", Float) = 1
    }
    SubShader
    {
        
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM        
        #pragma surface surf Standard fullforwardshadows vertex:vert addshadow
        #pragma target 3.0

        #include "VolumetricInteraction.cginc"
        
        struct Input
        {
            float2 uv_MainTex;
            float displacement_amount;
        };

        float _Displacement;

        void vert(inout appdata_full v, out Input o)
        {
            
            float4 world_position = mul(unity_ObjectToWorld, v.vertex);
            float3 world_normal = UnityObjectToWorldNormal(v.normal);

            float3 direction = get_displacement_world(world_position);
            float3 projection =  world_normal * (dot(direction, world_normal) / pow(length(world_normal), 2)); // project direction onto world_normal...
            // projection = normalize(projection);
            
            world_position.xyz -= projection * _Displacement;

            v.vertex.xyz = mul(unity_WorldToObject, world_position);

            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.displacement_amount = length(direction);
        }

        sampler2D _MainTex;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _DisplacementColor;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * lerp(_Color, _DisplacementColor, IN.displacement_amount);
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
