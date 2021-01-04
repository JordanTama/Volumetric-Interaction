uniform sampler3D interaction_texture;

uniform matrix volume_local_to_world;
uniform matrix volume_world_to_local;

float4 sample_interaction(float3 world_position)
{
    // Make world_position relative to the volume.
    world_position = world_position - mul(volume_local_to_world, float4(0, 0, 0, 1));

    // Convert to local position.
    float3 local_position = mul(volume_world_to_local, world_position);

    // Move from -0.5, +0.5 range to 0, 1 range (UV).
    local_position = local_position + .5;

    // Sample the interaction_texture.
    float4 sample = tex3Dlod(interaction_texture, float4(local_position, 0));
    return sample;
}
