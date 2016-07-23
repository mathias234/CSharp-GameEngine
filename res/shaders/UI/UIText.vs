#version 410

layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texCoord;

out vec2 texCoord0;

uniform mat4 T_ORTHO;

void main() 
{
	texCoord0 = texCoord;

    gl_Position = T_ORTHO * vec4(position, 1);
}