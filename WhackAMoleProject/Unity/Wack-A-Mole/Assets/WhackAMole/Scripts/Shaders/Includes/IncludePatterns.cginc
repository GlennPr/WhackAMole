

/*
BANNED VARIABLE LIST : because Unity internally uses these names?
- point


*/

#include "Includes/IncludeNoise.cginc"


static const float Custom_PI = 3.14159265359;
static const float Custom_TWO_PI = 6.28318530718;

float2 TruchetPatternRaw(in float2 uv) 
{
	float2 ipos = floor(uv);  // integer
	float2 fpos = frac(uv);  // fraction

	uv = fpos;

	float index = Random(ipos);
	index = frac(((index - 0.5) * 2.0));
	if (index > 0.75) {
		uv = float2(1.0, 1.0) - uv; // flip both axis
	}
	else if (index > 0.5) {
		uv = float2(1.0 - uv.x, uv.y); //flip X
	}
	else if (index > 0.25) {
		uv = float2(uv.x, 1.0 - uv.y); // flip Y 
	}

	return uv;
}


float TruchetPatternTriangle(in float2 uv)
{
	float2 tile = TruchetPatternRaw(uv);

	return step(tile.x, tile.y);
}

float TruchetPatternLines(in float2 uv, float thickness)
{
	float2 tile = TruchetPatternRaw(uv);

	return smoothstep(tile.x - thickness, tile.x, tile.y) - smoothstep(tile.x, tile.x + thickness, tile.y);
}

float TruchetPatternCircle(in float2 uv, float thickness)
{
	float2 tile = TruchetPatternRaw(uv);

	float max = 0.5 + thickness;
	float min = 0.5 - thickness;

	return	(
				step(length(tile), max) - step(length(tile), min)
			) 
			+
			(
				step(length(tile - float2(1.0, 1.0)), max) - step(length(tile - float2(1.0, 1.0)), min)
			);
}


