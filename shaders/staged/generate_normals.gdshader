shader_type canvas_item;

uniform sampler2D height_map;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	vec2  lods = textureQueryLod(height_map,UV);
	ivec2 dims = textureSize(height_map,int(lods.y));
	vec2  delta = vec2(1,1) / vec2(dims);
	vec2  dx = vec2(delta.x,0.0);
	vec2  dy = vec2(0.0,delta.y);
	float height_00 = texture(height_map,UV).r;
	float height_10 = texture(height_map,UV+dx).r;
	float height_01 = texture(height_map,UV+dy).r;
	float dhdx = height_10-height_00;
	float dhdy = height_01-height_00;
	vec3 x_tangent = normalize(vec3(delta.x,0.0,dhdx));
	vec3 y_tangent = normalize(vec3(0.0,delta.y,dhdy));
	vec3 norm = normalize(cross(x_tangent,y_tangent));
	norm.y *= -1.0;
	COLOR.rgb = norm;
}