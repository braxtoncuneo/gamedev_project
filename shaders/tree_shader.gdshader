shader_type spatial;

uniform vec2 target;

void vertex() {
	// Called for every vertex the material is visible on.
	vec4 world_pos = vec4(VERTEX,1) * MODEL_MATRIX;
	vec3 offset = vec3(target.x,0,target.y);
	VERTEX += normalize(offset) * (sin(TIME*PI)+1.0) * (VERTEX.y+1.0) * 0.05;
}

void fragment() {
	ALBEDO = vec3(0,1,0);
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
