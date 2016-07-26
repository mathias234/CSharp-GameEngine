#version 120

void main()
{
	// this shouldnt be used in basic shadow map generation
	gl_FragColor = vec4(gl_FragCoord.z);
}