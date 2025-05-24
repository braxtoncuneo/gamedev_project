shader_type particles;
render_mode keep_data;

uniform vec3 character_position;



void start() {
	COLOR = vec4(1,1,1,1);
	VELOCITY += vec3(0,-1,0);
}

void process() {
	COLOR = vec4(1,1,1,1);
}

void vertex() {

}

