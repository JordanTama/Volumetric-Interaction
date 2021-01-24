struct seed
{
    float3 position;
    float3 prev_position;
    float radius;
};

RWTexture3D<float4> result;
RWStructuredBuffer<seed> buffer;

float4x4 volume_local_to_world;
float4x4 volume_world_to_local;
int3 resolution;
float delta;
float decay_speed;
float radius_multiplier;
int3 step_size;

float move_towards(float current, float target, float max_delta)
{
    if (abs(target - current) <= max_delta)
        return target;

    return current + sign(target - current) * max_delta;
}

float3 closest_point_on_line_segment(float3 p, float3 a, float3 b)
{
    float3 v = b - a;
    float3 u = a - p;
    
    float t = -(dot(v, u) / dot(v, v));
    t = saturate(t);
    return (1 - t) * a + t * b;
}

float3 pixel_to_world(float3 id)
{
    const float3 uv = id.xyz / float3(resolution.x - 1, resolution.y - 1, resolution.z - 1);
    const float3 local = uv.xyz - 0.5;
    return mul(volume_local_to_world, float4(local, 1));
}

float3 world_to_pixel(float3 world)
{
    const float3 local = mul(volume_world_to_local, float4(world, 1));
    const float3 uv = local + 0.5;
    return floor(uv * float3(resolution - 1));
}

float3 uv_to_world(float3 uv)
{
    const float3 local = uv - 0.5;
    return mul(volume_local_to_world, float4(local, 1)).xyz;
}

float4 blend_interaction(float4 from, float4 to)
{
    float4 final_value = float4(to.xyz, max(from.a, to.a));

    const float t_a = to.a + from.a;
    if (t_a > 0)
        final_value.xyz = (to.xyz * (to.a / t_a)) + (from.xyz * (from.a / t_a));
    
    return final_value;
}

float4 calculate_interaction_point(float3 world)
{
    float4 new_value = float4(0.5, 0.5, 0.5, 0);
    
    uint length, stride;
    buffer.GetDimensions(length, stride);
    
    for (uint i = 0; i < length; i++)
    {
        const float dist = distance(world, buffer[i].position);
        const float strength = 1 - dist / buffer[i].radius;
        if (strength >= new_value.a)
        {
            float3 to_seed = (normalize(float3(buffer[i].position - world)) + 1) * 0.5;
            new_value = float4(to_seed, strength);
        }
    }

    return new_value;
}

float4 calculate_interaction_line(float3 world)
{
    float4 new_value = float4(0.5, 0.5, 0.5, 0);
    
    uint length, stride;
    buffer.GetDimensions(length, stride);
    
    for (uint i = 0; i < length; i++)
    {
        const float3 point_on_line = closest_point_on_line_segment(world, buffer[i].prev_position, buffer[i].position);
        const float dist = distance(world, point_on_line);
        const float strength = 1 - dist / buffer[i].radius;
        if (strength >= new_value.a)
        {
            float3 to_seed = (normalize(float3(point_on_line - world)) + 1) * 0.5;
            new_value = float4(to_seed, strength);
        }
    }

    return new_value;
}