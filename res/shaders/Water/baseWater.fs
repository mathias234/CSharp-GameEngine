#version 330

in vec4 clipSpace;
in vec2 texCoord0;
in vec3 toCameraVector;

out vec4 fragColor;

uniform sampler2D reflectionTexture;
uniform sampler2D refractionTexture;
uniform sampler2D diffuse;
uniform sampler2D refractionTextureDepth;

uniform float moveFactor;
uniform float waveStrength;
uniform float refractivePower;

void main()
{
	vec2 ndc = (clipSpace.xy / clipSpace.w) / 2.0 + 0.5;
	vec2 reflectTexCoord = vec2(ndc.x, -ndc.y);
	vec2 refractTexCoord = vec2(ndc.x, ndc.y);
	
	float near = 0.1;
	float far = 1000;
	float depth = texture(refractionTextureDepth, refractTexCoord).r;
	float floorDistance = 2.0 * near * far / (far + near - (2.0 * depth - 1.0) * (far - near));
	
	depth = gl_FragCoord.z;
	
	float waterDistance =  2.0 * near * far / (far + near - (2.0 * depth - 1.0) * (far - near));
	
	float waterDepth = floorDistance - waterDistance;
	
	vec2 distortedTexCoords = texture(diffuse, vec2(texCoord0.x + moveFactor, texCoord0.y)).rg*0.1;
	distortedTexCoords = texCoord0 + vec2(distortedTexCoords.x, distortedTexCoords.y+moveFactor);
	vec2 totalDistortion = (texture(diffuse, distortedTexCoords).rg * 2.0 - 1.0) * waveStrength * clamp(waterDepth / 20, 0.0, 1.0);
	
	refractTexCoord += totalDistortion;
	refractTexCoord = clamp(refractTexCoord, 0.001, 0.999);
	
	reflectTexCoord += totalDistortion;
	reflectTexCoord.x = clamp(reflectTexCoord.x, 0.001, 0.999);
	reflectTexCoord.y = clamp(reflectTexCoord.y, -0.999, -0.001);

	
	vec4 reflectColor = texture(reflectionTexture, reflectTexCoord);
	vec4 refractColor = texture(refractionTexture, refractTexCoord);
	
	vec3 viewVector = normalize(toCameraVector);
	float refractiveFactor = dot(viewVector, vec3(0.0, 1.0,0.0));
	refractiveFactor = pow(refractiveFactor, refractivePower);
	refractiveFactor = clamp(refractiveFactor, 0.001, 0.999);
	
	
	fragColor = mix(reflectColor, refractColor, refractiveFactor);
	fragColor = mix(fragColor, vec4(0.0, 0.3, 0.5, 1.0), clamp(waterDepth / 5, 0.0, 1.0) / 1.5);
	
	if(clamp(waterDepth / 5, 0.0, 1.0) <= 0.09)
		fragColor = vec4(1,1,1,1);
}
	
	