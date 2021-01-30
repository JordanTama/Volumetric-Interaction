uniform sampler3D interaction_texture;

uniform float4x4 volume_local_to_world;
uniform float4x4 volume_world_to_local;


float4 raw_to_force(float4 raw)
{
    raw.xyz = raw.xyz * 2 - 1;
    return raw;
}


float4 get_raw_local(float3 local_position)
{
    float4 samp = tex3Dlod(interaction_texture, float4(local_position, 0));
    return samp;
}

float4 get_raw_world(float3 world_position)
{
    world_position -= mul(volume_local_to_world, float4(0, 0, 0, 1));
    
    float3 local_position = mul(volume_world_to_local, world_position);
    local_position += 0.5;

    return get_raw_local(local_position);
}


float4 get_force_local(float3 local_position)
{
    float4 samp = get_raw_local(local_position);
    
    return raw_to_force(samp);
}

float4 get_force_world(float3 world_position)
{
    float4 samp = get_raw_world(world_position);
    return raw_to_force(samp);
}