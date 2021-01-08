Shader "Volumetric Interaction/VIDisplacement"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
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
        };

        float _Displacement;

        void vert(inout appdata_full v)
        {
            float4 world_position = mul(unity_ObjectToWorld, v.vertex);
            float3 world_normal = UnityObjectToWorldNormal(v.normal);

            float3 direction = get_displacement_world(world_position);

            world_position.xyz -= direction * _Displacement;

            v.vertex.xyz = mul(unity_WorldToObject, world_position);
        }

        sampler2D _MainTex;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
