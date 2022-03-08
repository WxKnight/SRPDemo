#ifndef CUSTOM_BRDF_INCLUDED
#define CUSTOM_BRDF_INCLUDED
#define MIN_REFLECTIVITY 0.04

struct BRDF
{
    float3 diffuse;
    float3 specular;
    float roughness;
};


float GetOneMinusReflectivity(float metallic)
{
    float range = 1.0 - MIN_REFLECTIVITY;
    return range * (1 - metallic);
}


BRDF GetBRDF(Surface surface,bool applyAlphaToDiffuse = false)
{
    BRDF brdf;
    brdf.diffuse = surface.color * GetOneMinusReflectivity(surface.metallic);
    if (applyAlphaToDiffuse) {
        brdf.diffuse *= surface.alpha;
    }
    brdf.specular = lerp(MIN_REFLECTIVITY,surface.color,surface.metallic);// surface.color - brdf.diffuse; 但是金属会影响镜面反射颜色
    float perceptualRoughness = PerceptualSmoothnessToRoughness(surface.smoothness);
    brdf.roughness = PerceptualRoughnessToRoughness(perceptualRoughness);

    return brdf;
}

#endif