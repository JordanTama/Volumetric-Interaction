uniform sampler3D interaction_texture;
uniform matrix volume_local_to_world;
uniform matrix volume_world_to_local;

float4 sample_interaction(float3 world_position)
{
    world_position = world_position - mul(volume_local_to_world, float4(0, 0, 0, 1));
    float3 local_position = mul(volume_world_to_local, world_position);
    local_position = local_position + .5;
    float4 sample = tex3Dlod(interaction_texture, float4(local_position, 0));
    return sample;
}
