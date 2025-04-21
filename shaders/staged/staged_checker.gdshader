shader_type canvas_item;

uniform float scale = 0.1;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	ivec2 position = ivec2(UV/scale);
	if ((position.x + position.y) % 2 == 0) {
		COLOR = vec4(0,0,0,1);
	} else {
		COLOR = vec4(1,1,1,1);
	}
}
