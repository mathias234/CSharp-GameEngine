in vec2 texCoord0;
in vec3 worldPos0;
in vec4 shadowMapCoords0;
in mat3 tbnMatrix;

out vec4 fragColor;

uniform sampler2D diffuse;
uniform sampler2D normalMap;

uniform sampler2D tex2;
uniform sampler2D tex2Nrm;

uniform sampler2D layer1;

uniform sampler2D tex3;
uniform sampler2D tex3Nrm;

uniform sampler2D layer2;
uniform sampler2D dispMap;

uniform float dispMapScale;
uniform float dispMapBias;

uniform sampler2D R_shadowMap;
uniform float R_shadowVarianceMin;
uniform float R_shadowBleedingReduction;

#include "lighting.glh"