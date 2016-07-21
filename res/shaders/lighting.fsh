in vec2 texCoord0;
in vec3 worldPos0;
in vec4 shadowMapCoords0;
in mat3 tbnMatrix;

out vec4 fragColor;

uniform sampler2D diffuse;
uniform sampler2D normalMap;
uniform sampler2D dispMap;

uniform float dispMapScale;
uniform float dispMapBias;
uniform float R_shadowBias;

uniform vec3 R_shadowTexelSize;

uniform sampler2D R_shadowMap;

#include "lighting.glh"