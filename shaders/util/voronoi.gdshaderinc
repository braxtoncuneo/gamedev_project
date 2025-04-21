#include "random.gdshaderinc"
#include "grid.gdshaderinc"


struct VoronoiScanEntry2D {
	ivec2 cell;
	vec2 nucleus_position;
	float distance_to_nucleus;
};

struct VoronoiScanEntry3D {
	ivec3 cell;
	vec3 nucleus_position;
	float distance_to_nucleus;
};


vec2 nucleus_pos_2d (ivec2 cell_pos,float scale,float offset_magnetude,bool loop) {
	ivec2 seed_pos = cell_pos;
	if (loop) {
		seed_pos = cell_pos - ivec2(floor(vec2(cell_pos)*scale)/scale);
	}
	uint seed = hash_ivec2(0,seed_pos);
	vec2 nucleus_offset = (random_vec2(seed)-0.5)*offset_magnetude;
	return (nucleus_offset + 0.5 + vec2(cell_pos))*scale;
}

vec3 nucleus_pos_3d (ivec3 cell_pos,float scale,float offset_magnetude,bool loop) {
	ivec3 seed_pos = cell_pos;
	if (loop) {
		seed_pos = cell_pos - ivec3(floor(vec3(cell_pos)*scale)/scale);
	}
	uint seed = hash_ivec3(0,seed_pos);
	vec3 nucleus_offset = (random_vec3(seed)-0.5)*offset_magnetude;
	return (nucleus_offset + 0.5 + vec3(cell_pos))*scale;
}


float distance_to_midline_2d(vec2 a, vec2 b, vec2 point) {
	vec2 normal = normalize(b-a);
	vec2 midpoint = (a+b)/2.0;
	return abs(dot(normal,midpoint) - dot(normal,point));
}

float distance_to_midline_3d(vec3 a, vec3 b, vec3 point) {
	vec3 normal = normalize(b-a);
	vec3 midpoint = (a+b)/2.0;
	return abs(dot(normal,midpoint) - dot(normal,point));
}


VoronoiScanEntry2D voronoi_2d(
	vec2 position,
	float scale,
	float offset_magnetude,
	uint scan_breadth,
	bool loop,
	out float distance_to_border
) {

	const uint RANKING_SIZE = 4u;
	VoronoiScanEntry2D ranking[RANKING_SIZE];
	distance_to_border = scale*1000.0;

	ivec2 home_cell = cell_coord_2d(position,vec2(scale,scale));
	vec2 home_position = nucleus_pos_2d(home_cell,scale,offset_magnetude,loop);
	for (uint i=0u; i<RANKING_SIZE; i++) {
		ranking[i].cell = home_cell;
		ranking[i].nucleus_position = home_position;
		ranking[i].distance_to_nucleus = scale * 1000.0;
	}

	int signed_breadth = int(scan_breadth);
	for (int y=-signed_breadth; y<=signed_breadth; y++) {
		for (int x=-signed_breadth; x<=signed_breadth; x++) {
			VoronoiScanEntry2D current;
			current.cell = home_cell + ivec2(x,y);
			current.nucleus_position = nucleus_pos_2d(current.cell,scale,offset_magnetude,loop);
			current.distance_to_nucleus = length(current.nucleus_position-position);

			for (uint i=0u; i<RANKING_SIZE; i++) {
				if (current.distance_to_nucleus < ranking[i].distance_to_nucleus) {
					VoronoiScanEntry2D temp = ranking[i];
					ranking[i] = current;
					current = temp;
				}
			}
		}
	}

	for (uint i=1u; i<RANKING_SIZE; i++) {
		float midline_dist = distance_to_midline_2d(
			ranking[0].nucleus_position,
			ranking[i].nucleus_position,
			position
		);
		distance_to_border = min(distance_to_border,midline_dist);
	}

	return ranking[0];
}



VoronoiScanEntry3D voronoi_3d(
	vec3 position,
	float scale,
	float offset_magnetude,
	uint scan_breadth,
	bool loop,
	out float distance_to_border
) {

	const uint RANKING_SIZE = 4u;
	VoronoiScanEntry3D ranking[RANKING_SIZE];
	distance_to_border = scale*1000.0;

	ivec3 home_cell = cell_coord_3d(position,vec3(scale,scale,scale));
	vec3 home_position = nucleus_pos_3d(home_cell,scale,offset_magnetude,loop);
	for (uint i=0u; i<RANKING_SIZE; i++) {
		ranking[i].cell = home_cell;
		ranking[i].nucleus_position = home_position;
		ranking[i].distance_to_nucleus = scale * 1000.0;
	}

	int signed_breadth = int(scan_breadth);
	for (int z=-signed_breadth; z<=signed_breadth; z++) {
		for (int y=-signed_breadth; y<=signed_breadth; y++) {
			for (int x=-signed_breadth; x<=signed_breadth; x++) {
				VoronoiScanEntry3D current;
				current.cell = home_cell + ivec3(x,y,z);
				current.nucleus_position = nucleus_pos_3d(current.cell,scale,offset_magnetude,loop);
				current.distance_to_nucleus = length(current.nucleus_position-position);

				for (uint i=0u; i<RANKING_SIZE; i++) {
					if (current.distance_to_nucleus < ranking[i].distance_to_nucleus) {
						VoronoiScanEntry3D temp = ranking[i];
						ranking[i] = current;
						current = temp;
					}
				}
			}
		}
	}

	for (uint i=1u; i<RANKING_SIZE; i++) {
		float midline_dist = distance_to_midline_3d(
			ranking[0].nucleus_position,
			ranking[i].nucleus_position,
			position
		);
		distance_to_border = min(distance_to_border,midline_dist);
	}

	return ranking[0];
}




