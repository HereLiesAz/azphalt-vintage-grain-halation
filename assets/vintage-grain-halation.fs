/*{
  "DESCRIPTION": "Fine animated film grain plus warm halation bloom around bright highlights — the vintage, nostalgic look smart-effect tools reach for automatically.",
  "CATEGORIES": ["Guillotine", "Stylize"],
  "INPUTS": [
    { "NAME": "inputImage", "TYPE": "image" },
    { "NAME": "grain", "TYPE": "float", "DEFAULT": 0.12, "MIN": 0.0, "MAX": 0.4 },
    { "NAME": "halation", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 }
  ]
}*/

float hash(vec2 p) {
  return fract(sin(dot(p, vec2(12.9898, 78.233))) * 43758.5453);
}

void main() {
  vec2 uv = isf_FragNormCoord;
  vec2 texel = 1.0 / RENDERSIZE;
  vec4 c = IMG_THIS_PIXEL(inputImage);

  // A cheap halation source: bright neighbors bleed warm color outward.
  vec3 bright = max(c.rgb - 0.7, 0.0) * 3.0;
  bright += max(IMG_NORM_PIXEL(inputImage, uv + vec2(texel.x * 3.0, 0.0)).rgb - 0.7, 0.0) * 3.0;
  bright += max(IMG_NORM_PIXEL(inputImage, uv - vec2(texel.x * 3.0, 0.0)).rgb - 0.7, 0.0) * 3.0;
  bright += max(IMG_NORM_PIXEL(inputImage, uv + vec2(0.0, texel.y * 3.0)).rgb - 0.7, 0.0) * 3.0;
  bright += max(IMG_NORM_PIXEL(inputImage, uv - vec2(0.0, texel.y * 3.0)).rgb - 0.7, 0.0) * 3.0;
  bright *= 0.2;

  vec3 warmHalation = bright * vec3(1.0, 0.55, 0.25) * halation;

  // Animated grain: reseeded every frame via TIME, so it flickers like real film instead of a static overlay.
  float n = hash(uv * RENDERSIZE + fract(TIME) * 97.0) - 0.5;

  vec3 result = c.rgb + warmHalation + n * grain;
  gl_FragColor = vec4(result, c.a);
}
