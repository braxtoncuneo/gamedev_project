shader_type spatial;
render_mode unshaded;

void vertex() {
}

void fragment() {
	ALBEDO.rgb = vec3(UV,0);
}


