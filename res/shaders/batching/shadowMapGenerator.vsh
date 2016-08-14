#version 330

layout (location = 0) in vec3 position;
layout (location = 4) in mat4 model;


uniform mat4 T_VP;

void main()
{
    gl_Position = T_VP * model * vec4(position, 1.0);
}