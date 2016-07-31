#version 330
#include "terrain/lighting.fsh"

uniform PointLight R_pointLight;

vec4 CalcLightingEffect(vec3 normal, vec3 worldPos)
{    
    return CalcPointLight(R_pointLight,  normal, worldPos);
}

#include "terrain/lightingMain.fsh"
