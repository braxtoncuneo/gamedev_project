shader_type spatial;
render_mode unshaded;

varying vec3 world_vertex;

uniform float scale = 1.0;

bool trues_even(bvec3 val) {
	return (int(val.x)+int(val.y)+int(val.z)) % 2 == 0;
}

void vertex() {
	world_vertex = (MODEL_MATRIX * vec4(VERTEX,1)).xyz / scale;
}

void fragment() {
	// Called for every pixel the material is visible on.
	bvec3 is_positive = greaterThanEqual(world_vertex,vec3(0,0,0));
	vec3 rounded_coordinate = floor(world_vertex+0.0001);
	ivec3 inner_coordinate = ivec3(world_vertex - rounded_coordinate);
	ivec3 tile_coordinate = ivec3(rounded_coordinate);
	bvec3 is_even = equal(tile_coordinate % 2,ivec3(0,0,0));
	bool is_black = trues_even(is_even);
	if (is_black) {
		ALBEDO = vec3(0,0,0);
	}
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
