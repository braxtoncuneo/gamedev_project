shader_type spatial;

#include "../util/voronoi.gdshaderinc"

uniform float root_scale = 0.01;
uniform float scale_factor = 2.0;
uniform float offset = 1.0;
uniform float border = 0.1;
uniform int   levels = 2;
uniform bool  show_nucleus = false;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	float dist;
	float scale = root_scale;
	float wiggle = sin(TIME);
	VoronoiScanEntry2D entry = voronoi_2d(UV,scale,offset*wiggle,2,true,dist);
	for (int i=0; i<levels; i++) {
		scale *= scale_factor;
		entry = voronoi_2d(entry.nucleus_position,scale,offset*wiggle,2,true,dist);
	}

	ivec2 seed_pos = entry.cell - ivec2(floor(vec2(entry.cell)*scale)/scale);
	uint hash = hash_ivec2(0,seed_pos);
	vec3 color = random_vec3(hash)*0.5;

	if ( hash%2u == 0u ) {
		discard;
	}

	if ( dist < root_scale*border ) {
		ALBEDO.rgb = vec3(0,0,0);
	} else {
		ALBEDO.rgb = vec3(1,1,1) * color * 0.9;
	}

	float distance_to_nucleus = length(entry.nucleus_position-UV);
	if (show_nucleus && (distance_to_nucleus < scale*0.05)) {
		ALBEDO.rgb = vec3(0,0,0);
	}
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
