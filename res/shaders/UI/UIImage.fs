#version 330

in vec2 texCoord0;

uniform sampler2D diffuse;

out vec4 fragColor;

void main() 
{
	vec4 tex = texture2D(diffuse, texCoord0.xy);
	if(tex.a == 0)
		discard;
	fragColor =  tex;
}