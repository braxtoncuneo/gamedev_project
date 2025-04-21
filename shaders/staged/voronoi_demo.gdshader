shader_type canvas_item;

#include "../util/voronoi.gdshaderinc"

uniform bool  show_nucleus = false;
uniform float grid_scale = 0.1;
uniform float offset = 1.0;
uniform float border = 0.02;
uniform uint  breadth = 2;
uniform bool  wrap = true;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {

	float distance_to_border;
	VoronoiScanEntry2D entry = voronoi_2d(UV,grid_scale,offset,breadth,wrap,distance_to_border);
	ivec2 seed_pos = entry.cell - ivec2(floor(vec2(entry.cell)*grid_scale)/grid_scale);
	uint hash = hash_ivec2(0,seed_pos);
	vec3 color = random_vec3(hash);

	if ( distance_to_border < grid_scale*border ) {
		COLOR.rgb = vec3(0,0,0);
	} else {
		COLOR.rgb = color * 0.9;
	}

	if (show_nucleus && (entry.distance_to_nucleus < grid_scale*0.05)) {
		COLOR.rgb = vec3(0,0,0);
	}
}


