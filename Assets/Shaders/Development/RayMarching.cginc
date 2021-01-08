float2 ray_box_distance(const float3 min_bounds, const float3 max_bounds, const float3 ray_origin, const float3 ray_direction)
{
    const float3 t0 = (min_bounds - ray_origin) / ray_direction;
    const float3 t1 = (max_bounds - ray_origin) / ray_direction;
    const float3 t_min = min(t0, t1);
    const float3 t_max = max(t0, t1);

    const float a_dst = max(max(t_min.x, t_min.y), t_min.z);
    const float b_dst = min(t_max.x, min(t_max.y, t_max.z));

    const float dst_to_box = max(0, a_dst);
    const float dst_in_box = max(0, b_dst - dst_to_box);
    
    return float2(dst_to_box, dst_in_box);
}