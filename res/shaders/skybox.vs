#version 330 core
layout (location = 0) in vec3 position;
out vec3 TexCoords;

uniform mat4 T_cameraPos;

void main()
{
    gl_Position =  T_cameraPos * vec4(position, 1.0);  
    TexCoords = position;
}  