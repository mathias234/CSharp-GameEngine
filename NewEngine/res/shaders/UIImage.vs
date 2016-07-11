#version 330

layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texCoord;

out vec2 texCoord0;

uniform mat4 projectionMatrix;

void main() 
{
    // Transform from absolute texture coordinates to normalized texture coordinates
    // This works because the rectangle spans [0,1] x [0,1]
    // Depending on where the origin lies in your texture (i.e. topleft or bottom left corner), you need to replace "1. - vertex.y" with just "vertex.y"
	texCoord0 = texCoord;



    // Apply the model, view and projection transformations
    gl_Position = projectionMatrix * vec4(position, 1);
}