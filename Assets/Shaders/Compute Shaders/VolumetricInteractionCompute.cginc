
struct seed
{
    float3 position;
    float3 prev_position;
    float radius;
};

RWTexture3D<float4> result;
RWStructuredBuffer<seed> buffer;

float4x4 volume_local_to_world;
int3 resolution;
float delta;

float move_towards(float current, float target, float max_delta)
{
    if (abs(target - current) <= max_delta)
        return target;

    return current + sign(target - current) * max_delta;
}