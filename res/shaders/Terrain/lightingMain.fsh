#include "sampling.glh"

float CalcShadowAmount(sampler2D shadowMap, vec4 initialShadowMapCoords) 
{
	vec3 shadowMapCoords = (initialShadowMapCoords.xyz/initialShadowMapCoords.w);
		
	return SampleVarianceShadowMap(shadowMap, shadowMapCoords.xy, shadowMapCoords.z, R_shadowVarianceMin, R_shadowBleedingReduction);
}

#include "Terrain/terrainCalc.fsh"

void main()
{
    vec3 directionToEye = normalize(C_eyePos - worldPos0);
	vec2 texCoords = CalcParallaxTexCoords(dispMap, tbnMatrix, directionToEye, texCoord0, dispMapScale, dispMapBias);
	vec3 normal = normalize(tbnMatrix * (255.0/128.0 * calcTerrain(texCoords, normalMap, tex2Nrm, layer1, tex3Nrm, layer2).xyz - 1));
	
	vec4 lightingAmt = CalcLightingEffect(normal, worldPos0) * CalcShadowAmount(R_shadowMap, shadowMapCoords0);
	
	fragColor = calcTerrain(texCoords, diffuse, tex2, layer1, tex3, layer2) * lightingAmt;
}