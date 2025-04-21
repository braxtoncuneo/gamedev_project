shader_type spatial;

uniform sampler2D ground_noise;

varying vec3 world_normal;

void vertex() {
	world_normal = MODEL_NORMAL_MATRIX * NORMAL;
}

void fragment() {
	vec3 green = vec3(0.2,0.3,0.0);
	vec3 brown = vec3(0.2,0.2,0.0);
	float verticality = 1.0 - dot(world_normal,vec3(0,1,0));
	float rugged_factor  = (texture(ground_noise,UV*2.0).r+1.0) * pow(verticality,0.5);
	ALBEDO = mix(green,brown,rugged_factor);
	METALLIC = mix(0.8,0.1,rugged_factor);
	SSS_STRENGTH = mix(0.5,0.1,rugged_factor);
}
