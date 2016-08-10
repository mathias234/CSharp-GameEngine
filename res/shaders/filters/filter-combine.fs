#version 330
#include "sampling.glh"

in vec2 texCoord0;


uniform sampler2D R_filterTexture;
uniform sampler2D R_displayTexture;

uniform float R_bloomAmount;

out vec4 fragColor;

void main()
{
	vec4 sceneColor = texture(R_displayTexture, texCoord0);
	vec4 highLightColor = texture(R_filterTexture, texCoord0);
	
	fragColor = sceneColor + highLightColor * R_bloomAmount;
}