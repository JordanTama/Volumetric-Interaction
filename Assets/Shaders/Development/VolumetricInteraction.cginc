uniform sampler3D interaction_texture;

uniform float4x4 volume_local_to_world;
uniform float4x4 volume_world_to_local;

float4 sample_interaction_local(float3 local_position)
{
    float4 samp = tex3Dlod(interaction_texture, float4(local_position, 0));
    return samp;
}

float4 sample_interaction_world(float3 world_position)
{
    // Make world_position relative to the volume.
    world_position = world_position - mul(volume_local_to_world, float4(0, 0, 0, 1));

    // Convert to local position.
    float3 local_position = mul(volume_world_to_local, world_position);

    // Move from -0.5, +0.5 range to 0, 1 range (UV).
    local_position = local_position + .5;

    // Sample the interaction_texture.
    return sample_interaction_local(local_position);
}

float3 get_displacement_world(float3 world_position)
{
    float4 samp = sample_interaction_world(world_position);
    samp.xyz = samp.xyz * 2 - 1;
    samp.xyz *= samp.a;
    return samp.xyz;
}