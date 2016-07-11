#version 330

in vec2 texCoord0;

uniform sampler2D sampler;

out vec4 fragColor;

void main() 
{
	vec4 tex = texture2D(sampler, texCoord0.xy);
	fragColor =  tex;
}