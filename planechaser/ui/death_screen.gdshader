shader_type canvas_item; // Use for 2D UI elements

uniform float glitch_intensity : hint_range(0.0, 1.0) = 0.0;
uniform float displacement_amount : hint_range(0.0, 0.1) = 0.0;
uniform float line_frequency : hint_range(1.0, 100.0) = 50.0;

void fragment() {
    // Sample the original texture
    vec4 color = texture(TEXTURE, UV);

    // Simple horizontal displacement based on a sine wave for glitch effect
    float displacement = sin(UV.y * line_frequency + TIME * 50.0) * displacement_amount * glitch_intensity;
    vec2 displaced_uv = UV;
    displaced_uv.x += displacement;

    // Sample again with the displaced UV
    vec4 displaced_color = texture(TEXTURE, displaced_uv);

    // Mix original and displaced color
    COLOR = mix(color, displaced_color, glitch_intensity);
}
