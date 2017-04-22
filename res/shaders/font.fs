#version 330

in vec2 pass_textureCoords;

uniform vec3 color;
uniform sampler2D diffuse;

out vec4 fragColor;

void main() {
	fragColor = vec4(color, texture(diffuse, pass_textureCoords).a);
}