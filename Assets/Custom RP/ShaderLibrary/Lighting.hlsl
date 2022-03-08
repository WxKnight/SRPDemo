#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

float Square(float v)
{
    return v * v;
}

float SpecularStrength(Surface surface, BRDF brdf, Light light)
{
    float3 h = SafeNormalize(light.direction + surface.ViewDir);
    float nh2 = Square(saturate(dot(surface.normal, h)));
    float lh2 = Square(saturate(dot(light.direction, h)));
    float r2 = Square(brdf.roughness);
    float d2 = Square(nh2 * (r2 - 1.0) + 1.00001);
    float normalization = brdf.roughness * 4.0 + 2.0;
    return r2 / (d2 * max(0.1, lh2) * normalization);
}

float3 DirectBRDF(Surface surface, BRDF brdf, Light light)
{
    return SpecularStrength(surface,brdf,light) * brdf.specular + brdf.diffuse;
}


float3 IncomingLighting(Surface surface,Light light)
{
    return saturate(dot(surface.normal,light.direction)) * light.color;
}

float3 GetLighting(Surface surface,Light light,BRDF brdf)
{
    // return surface.normal.y * surface.color;
    return IncomingLighting(surface,light) * DirectBRDF(surface,brdf,light);
}

float3 GetLighting(Surface surface,BRDF brdf)
{
    float3 color = 0.0;
    for (int i = 0; i < GetDirectionalLightCount(); i++)
    {
        color += GetLighting(surface,GetDirectionalLight(i),brdf);
    }

    return color;
}

#endif