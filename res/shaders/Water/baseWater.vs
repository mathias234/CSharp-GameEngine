#version 330
layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texCoord;
layout (location = 2) in vec3 normal;
layout (location = 3) in vec3 tangent;

out vec4 clipSpace;
out vec2 texCoord0;
out vec3 toCameraVector;

uniform mat4 T_model;
uniform mat4 T_MVP;

uniform vec4 R_clipPlane;
uniform vec3 C_eyePos;

uniform float tiling;

void main()
{
	vec4 WorldPosition = (T_model * vec4(position.x, 0.0, position.y, 1.0));
	toCameraVector = C_eyePos - WorldPosition.xyz;

	clipSpace = T_MVP * vec4(position, 1.0);
	
	texCoord0 = texCoord * tiling;
    gl_Position = clipSpace;
	
	
    gl_ClipDistance[0] = dot(WorldPosition, R_clipPlane);
}