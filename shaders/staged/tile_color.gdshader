shader_type canvas_item;
render_mode unshaded;

uniform vec3 grime_color = vec3(0.15,0.05,0.0);
uniform vec3 tile_color  = vec3(0.5,0.8,0.7);
uniform sampler2D height_map;
uniform sampler2D grime_map;

void vertex() {
}

void fragment() {
	float height = texture(height_map,UV).r;
	float grime_level = texture(grime_map,UV).r;
	float grime_boost = max(0,grime_level-height);
	grime_level += grime_boost;
	COLOR.rgb = mix(tile_color,grime_color,grime_level) * height;
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
