shader_type spatial;

uniform float fuzz_magnetude     = 0.1;
uniform float fuzz_period        = 0.1;
uniform float fuzz_rate          = 0.1;

uniform float inter_eye_angle    = 0.3;
uniform float pupil_rise_angle   = 0.4;
uniform float pupil_outter_angle = 0.1;
uniform float pupil_inner_angle  = 0.05;
uniform float eye_fuzz           = 0.1;

uniform float mouth_rise_angle   = -0.1;
uniform float mouth_width_angle  = 0.5;
uniform float mouth_line_width_angle = 0.01;


uniform mat3 facing;
uniform float facing_angular_velocity = 0;
uniform float turn_fuzz = 1;
varying vec3 world_vertex;


void vertex() {
	world_vertex = inverse(facing) * mat3(MODEL_MATRIX) * VERTEX.xyz * 10.f;
}

bool on_eye_line() {
	// Called for every pixel the material is visible on.
	float rise_horizontal = cos(pupil_rise_angle);
	float rise_vertical   = sin(pupil_rise_angle);
	vec3 eye_dir = vec3(
		sin(inter_eye_angle)*rise_horizontal,
		rise_vertical,
		cos(inter_eye_angle)*rise_horizontal
	);
	vec3 frag_dir = normalize(world_vertex);
	float pitch = sin((asin(frag_dir.y)+TIME*fuzz_rate)/fuzz_period)*fuzz_magnetude;
	float fuzz_offset = pitch*fuzz_magnetude*(eye_fuzz+facing_angular_velocity*turn_fuzz);
	frag_dir.xz = mat2(
		vec2(cos(fuzz_offset),-sin(fuzz_offset)),
		vec2(sin(fuzz_offset), cos(fuzz_offset))
	) * frag_dir.xz;
	frag_dir.x = abs(frag_dir.x);
	frag_dir.z *= -1.f;

	bool is_black = false;
	float eye_angle = acos(dot(eye_dir,frag_dir));
	if ( (eye_angle < pupil_outter_angle) && (eye_angle > pupil_inner_angle) ) {
		return true;
	}
	return false;
}

bool on_mouth_line() {
	vec3 frag_dir = normalize(world_vertex);
	float rise_horizontal = cos(mouth_rise_angle);
	float rise_vertical   = sin(mouth_rise_angle);
	vec3 mouth_dir = vec3(
		rise_horizontal,
		rise_vertical,
		rise_horizontal
	);
	return false;
}

void fragment() {
	CLEARCOAT = 0.2;
	CLEARCOAT_ROUGHNESS = 1.0;
	RIM = 1.0;
	if (!on_eye_line()) {
		ALBEDO = vec3(0.2,0.4,0.8);
	} else {
		EMISSION = vec3(0.1,0.1,0.1);
	}
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
