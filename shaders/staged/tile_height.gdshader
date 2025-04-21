shader_type canvas_item;

uniform float grout_width = 0.2;
uniform float tile_scale = 0.2;

uniform sampler2D grime_map;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	vec2 uv = UV / tile_scale;
	vec2 uv_mod = uv - floor(uv);
	vec2 uv_from_center = (uv_mod - vec2(0.5,0.5))*2.0;
	float dist_to_edge = 1.0 - max(abs(uv_from_center.x),abs(uv_from_center.y));
	float height = 1.0;
	if(dist_to_edge <= (grout_width*0.5)) {
		dist_to_edge /= grout_width*0.5;
		height = dist_to_edge;
	}
	height = max(height,(height+texture(grime_map,UV).r/2.0));
	COLOR.rgb *= height;
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
