shader_type canvas_item;
render_mode unshaded;

const vec2 OFFSETS[8] = {
    vec2(-0.71, -0.71), vec2(-1, 0), vec2(-0.71, 0.71), vec2(0, -1), vec2(0, 1), 
    vec2(0.71, -0.71), vec2(1, 0), vec2(0.71, 0.71)
};

uniform float line_thickness : hint_range(0.0, 10.0);
uniform vec4 line_color : source_color = vec4(1.0);
uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;
uniform bool enabled;

void fragment() {
    if (enabled) {
        // Get the size of the pixels on screen, and create a variable for out outline
        vec2 size = line_thickness * SCREEN_PIXEL_SIZE;
        float outline = 0.0;

        // Project in each direction, and add up the alpha of each projection.
        // This is similar to picking a point, hit testing in 8 directions, and
        // returning true if the hit test works
        for (int i = 0; i < OFFSETS.length(); i++) {
            vec2 coordinate_offset = SCREEN_UV + size * OFFSETS[i];
            outline += texture(screen_texture, coordinate_offset).a;
        }
        
        // Force outline to 1 or 0
        outline = sign(outline);

        // Get the texture from the screen
        vec4 c = texture(screen_texture, SCREEN_UV);

        // If the alpha exists at least a little bit, amplify the colors.
        // This ensures alpha and colors remain precise even when reading from screen.
        if (c.a > 0.0001) {
            c.rgb /= c.a;
        }
        
        COLOR.rgb = mix(c, line_color, outline - c.a).rgb;
        
        if (COLOR.rgb == vec3(0.0)) {
            COLOR.a = 0.0;
        }
    }
    else {
        COLOR.rgb = texture(screen_texture, SCREEN_UV).rgb;
    }
}