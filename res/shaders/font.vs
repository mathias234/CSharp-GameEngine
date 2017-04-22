#version 330

layout (location = 0) in vec2 position;
layout (location = 1) in vec2 textureCoords;

uniform vec2 translation;

out vec2 pass_textureCoords;

void main(void){
	pass_textureCoords = textureCoords;

    gl_Position = vec4((position + translation * vec2(2.0, -2.0)), 0.0, 1.0);

}