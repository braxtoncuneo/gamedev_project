
#ifndef UTIL_GRID
#define UTIL_// Called for every pixel the material is visible on.GRID


int cell_coord_1d(float coord, float scale) {
	return int(floor(coord/scale));
}

vec2 inner_coord_1d(vec2 coord, vec2 scale) {
	return (coord/scale)-floor(coord/scale);
}

vec2 inner_coord_2d(vec2 coord, vec2 scale) {
	return (coord/scale)-floor(coord/scale);
}

ivec2 cell_coord_2d(vec2 coord, vec2 scale) {
	return ivec2(floor(coord/scale));
}


vec3 inner_coord_3d(vec3 coord, vec3 scale) {
	return (coord/scale)-floor(coord/scale);
}

ivec3 cell_coord_3d(vec3 coord, vec3 scale) {
	return ivec3(floor(coord/scale));
}



vec4 inner_coord_4d(vec4 coord, vec4 scale) {
	return (coord/scale)-floor(coord/scale);
}

ivec4 cell_coord_4d(vec4 coord, vec4 scale) {
	return ivec4(floor(coord/scale));
}


#endif // UTIL_GRID
