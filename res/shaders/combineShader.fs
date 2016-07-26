#version 330

in vec2 texCoord0;

uniform sampler2D R_basePass;
uniform sampler2D R_lightPass;

out vec4 fragColor;

void main()
{
	vec4 basePassResultTex = texture(R_basePass, texCoord0.xy);
	vec4 lightPassResultTex = texture(R_lightPass, texCoord0.xy);

	
	fragColor =(basePassResultTex);
}