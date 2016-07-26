vec4 calcTerrain(vec2 texCoords, sampler2D ArgTex1, sampler2D ArgTex2, sampler2D ArgLayer1, sampler2D ArgTex3, sampler2D ArgLayer2) 
{

	vec4 texture1 = texture2D(ArgTex1, texCoords.xy * 20);
	vec4 texture2 = texture2D(ArgTex2, texCoords.xy * 20);
	vec4 textureLayer1 = texture2D(ArgLayer1, texCoords.xy);
	vec4 texture3 = texture2D(ArgTex3, texCoords.xy * 50);
	vec4 textureLayer2 = texture2D(ArgLayer2, texCoords.xy);

	vec4 mix1 = mix(texture1, texture2, textureLayer1);
	vec4 mix2 = mix(mix1, texture3, textureLayer2);
	return mix2;
}
