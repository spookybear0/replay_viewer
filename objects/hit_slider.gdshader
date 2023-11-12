shader_type canvas_item;
render_mode blend_disabled;

uniform sampler2D SCREEN_TEXTURE: hint_screen_texture;

void vertex() {
    
}

void fragment() {
    //if (UV.y > 0.45 && UV.y < 0.55 && UV.x > 0.05 && UV.x < 0.95) {
    //    COLOR.rgb = vec3(0.5);
    //}
    if (UV.y > 0.95 || UV.y < 0.05) {
        COLOR.rgb = vec3(0.5);
    }
    else {
        COLOR.rgb = vec3(0.09803921568);
    }
    
}