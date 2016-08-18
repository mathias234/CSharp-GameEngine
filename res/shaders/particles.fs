#version 330 core

// Interpolated values from the vertex shaders
in vec2 UV;
in vec4 particlecolor;

// Ouput data
out vec4 color;

uniform sampler2D diffuse;
uniform sampler2D cutoutMask;
void main(){
	// Output color = color of the texture at the specified UV
	
	vec2 uv = UV;//vec2(((UV.x + 5) / 8), ((UV.y + 7) / 8));
	
	 vec4 cutout = texture(cutoutMask, uv);
	
	 if(cutout.r < 0.9) 
	 discard;
	
	color = texture( diffuse, uv ) * particlecolor;

}