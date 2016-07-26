#version 330
#include "sampling.glh"

// combine three textures using layers


in vec2 texCoord0;
in vec3 worldPos0;
in mat3 tbnMatrix;

uniform vec3 R_ambient;
uniform vec3 C_eyePos;
uniform sampler2D diffuse; //tex1
uniform sampler2D tex2;
uniform sampler2D layer1;
uniform sampler2D tex3;
uniform sampler2D layer2;
uniform sampler2D dispMap;

uniform float dispMapScale;
uniform float dispMapBias;

out vec4 fragColor;

#include "terrain/terrainCalc.fsh"

void main()
{
    vec3 directionToEye = normalize(C_eyePos - worldPos0);
	vec2 texCoords = CalcParallaxTexCoords(dispMap, tbnMatrix, directionToEye, texCoord0, dispMapScale, dispMapBias);

	fragColor = calcTerrain(texCoords, diffuse, tex2, layer1, tex3, layer2) * vec4(R_ambient, 1);
}