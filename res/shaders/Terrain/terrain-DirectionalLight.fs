#version 330
#include "terrain/lighting.fsh"

uniform DirectionalLight R_directionalLight;

vec4 CalcLightingEffect(vec3 normal, vec3 worldPos)
{    
    return CalcDirectionalLight(R_directionalLight, normal, worldPos);
}

#include "terrain/lightingMain.fsh"
