

/*
BANNED VARIABLE LIST : because Unity internally uses these names?
- point


*/

static const float Custom_PI = 3.14159265359;
static const float Custom_TWO_PI = 6.28318530718;
static const float2 UVCenter = float2(0.5, 0.5);

// https://docs.unity3d.com/Packages/com.unity.shadergraph@6.9/manual/Twirl-Node.html

float2 Twirl(float2 uv, float rotationCount)
{
	float2 delta = uv - UVCenter;
	float angle = rotationCount * Custom_TWO_PI * length(delta);

	float x = cos(angle) * delta.x - sin(angle) * delta.y;
	float y = sin(angle) * delta.x + cos(angle) * delta.y;

	//// if movement is wanted
	//vec2 offset = vec2(0., 0.) * iTime;
	//uv = vec2(x + center.x + offset.x, y + center.y + offset.y);

	uv.x = x;
	uv.y = y;

	return uv;
}

