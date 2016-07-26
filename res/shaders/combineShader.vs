#version 330
layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texCoord;
layout (location = 2) in vec3 normal;
layout (location = 3) in vec3 tangent;

out vec2 texCoord0;

uniform mat4 T_MVP;

void main()
{
    gl_Position = T_MVP * vec4(position, 1.0);
    texCoord0 = texCoord;
}