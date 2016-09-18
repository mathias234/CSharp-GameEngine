#version 330

in vec2 pass_textureCoords;

out vec4 fragColor;

uniform vec3 color;
uniform sampler2D fontAtlas;

void main() {
	fragColor = vec4(color, texture(fontAtlas, pass_textureCoords).a);
}