#version 330

in vec2 texCoord0;

uniform sampler2D diffuse;
uniform vec4 color;

out vec4 fragColor;

void main() 
{
	vec4 tex = texture2D(diffuse, texCoord0.xy);
	
	fragColor =  tex * color;
}