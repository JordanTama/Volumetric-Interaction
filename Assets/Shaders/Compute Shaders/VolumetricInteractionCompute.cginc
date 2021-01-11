struct seed
{
    float3 position;
    float3 prev_position;
    float radius;
};

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