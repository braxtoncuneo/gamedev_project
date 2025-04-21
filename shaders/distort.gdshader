shader_type spatial;

uniform sampler2D noise;

void vertex() {
	VERTEX.xyz *= 1.0 + texture(noise,UV).rgb*0.1;
}

void fragment() {
	// Called for every pixel the material is visible on.
	ALBEDO.rgb *= 0.1 + texture(noise,UV).rgb*0.1;
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
