#version 330
#include "sampling.glh"

in vec2 texCoord0;
in vec3 worldPos0;
in mat3 tbnMatrix;

uniform vec3 R_ambient;
uniform vec3 C_eyePos;
uniform sampler2D diffuse;
uniform sampler2D dispMap;
uniform sampler2D cutoutMask;

uniform float dispMapScale;
uniform float dispMapBias;

out vec4 fragColor;

void main()
{
	vec3 directionToEye = normalize(C_eyePos - worldPos0);
	vec2 texCoords = CalcParallaxTexCoords(dispMap, tbnMatrix, directionToEye, texCoord0, dispMapScale, dispMapBias);
	
	vec4 texture = texture2D(diffuse, texCoords.xy);
	vec4 cutoutMaskTex = texture2D(cutoutMask, texCoords.xy);
	
	if(cutoutMaskTex.r < 0.5)
		discard;

	fragColor = texture * vec4(R_ambient, 1);
}