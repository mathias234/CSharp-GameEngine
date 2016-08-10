#version 330
#include "sampling.glh"

in vec2 texCoord0;


uniform sampler2D R_filterTexture;

out vec4 fragColor;

void main()
{
vec4 tex = texture(R_filterTexture, texCoord0);
	
	float brightness = (tex.r * 0.2126) + (tex.g * 0.7152) + (tex.b * 0.0722);
	
	fragColor = tex * brightness;
}