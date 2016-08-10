#include "sampling.glh"
#include "shadow.glh"

#include "Terrain/terrainCalc.fsh"

void main()
{
    vec3 directionToEye = normalize(C_eyePos - worldPos0);
	vec2 texCoords = CalcParallaxTexCoords(dispMap, tbnMatrix, directionToEye, texCoord0, dispMapScale, dispMapBias);
	vec3 normal = normalize(tbnMatrix * (255.0/128.0 * calcTerrain(texCoords, normalMap, tex2Nrm, layer1, tex3Nrm, layer2).xyz - 1));
	
	vec4 lightingAmt = CalcLightingEffect(normal, worldPos0) * CalcShadowAmount(R_shadowMap, shadowMapCoords0);
	
	
	fragColor = calcTerrain(texCoords, diffuse, tex2, layer1, tex3, layer2) * lightingAmt;
}