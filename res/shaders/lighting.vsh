layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texCoord;
layout (location = 2) in vec3 normal;
layout (location = 3) in vec3 tangent;

out vec2 texCoord0;
out vec3 worldPos0;
out vec4 shadowMapCoords0;
out mat3 tbnMatrix;


uniform mat4 T_model;

uniform mat4 T_VP;

uniform mat4 R_lightMatrix;

void main()
{
    gl_Position = T_VP * T_model * vec4(position, 1.0);

    texCoord0 = texCoord;
	shadowMapCoords0 = R_lightMatrix * vec4(position, 1.0);
    worldPos0 = (T_model * vec4(position, 1.0)).xyz;
    
	vec3 n = normalize((T_model * vec4(normal, 0.0)).xyz);
	vec3 t = normalize((T_model * vec4(tangent, 0.0)).xyz);
	
	t = normalize(t - dot(t, n) * n);
	
	vec3 biTangent = cross(t, n);
	
	tbnMatrix = mat3(t, biTangent, n);
}