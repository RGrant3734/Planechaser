shader_type canvas_item;

uniform float glitch_intensity : hint_range(0.0, 1.0) = 0.0;
uniform float displacement_amount : hint_range(0.0, 0.1) = 0.0;
uniform float line_frequency : hint_range(1.0, 100.0) = 50.0;

void fragment() {
    // Keep modulate alpha only — not color
    float fade = COLOR.a;

    // Sample the texture normally (no color multiplication)
    vec4 color = texture(TEXTURE, UV);

    float displacement = sin(UV.y * line_frequency + TIME * 50.0)
                         * displacement_amount * glitch_intensity;

    vec2 displaced_uv = UV;
    displaced_uv.x += displacement;

    vec4 displaced_color = texture(TEXTURE, displaced_uv);

    // Glitch blend
    vec4 final_color = mix(color, displaced_color, glitch_intensity);

    // Apply fade a *single time only*
    final_color.a *= fade;

    COLOR = final_color;
}