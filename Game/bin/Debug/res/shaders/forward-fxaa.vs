#version 120
#extension GL_EXT_gpu_shader4 : enable

varying vec4 posPos;
uniform float FXAA_SUBPIX_SHIFT;


void main(void)
{
	float width = 800;
	float height = 600;
  gl_Position = ftransform();
  gl_TexCoord[0] = gl_MultiTexCoord0;
  vec2 rcpFrame = vec2(1.0/width, 1.0/height);
  posPos.xy = gl_MultiTexCoord0.xy;
  posPos.zw = gl_MultiTexCoord0.xy - 
                  (rcpFrame * (0.5 + FXAA_SUBPIX_SHIFT));
}
 