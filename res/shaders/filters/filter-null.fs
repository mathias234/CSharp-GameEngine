#version 330
#include "sampling.glh"

in vec2 texCoord0;


uniform sampler2D R_filterTexture;

out vec4 fragColor;

void main()
{
	vec4 texture = texture2D(R_filterTexture, texCoord0);
	
	fragColor = texture;
}