

/*
BANNED VARIABLE LIST : because Unity internally uses these names?
- point


*/

// 3D https://www.iquilezles.org/www/articles/distfunctions/distfunctions.htm
// 2D https://www.iquilezles.org/www/articles/distfunctions2d/distfunctions2d.htm

static const float Custom_PI = 3.14159265359;
static const float Custom_TWO_PI = 6.28318530718;

// 2D

float SDFCircle2D(float2 pos, float radius)
{
	return length(pos) - radius;
}

float SDFBox2D(float2 pos, float2 size )
{
	float2 q = abs(pos) - size;
	return length( max(q, 0.0) ) + min(max(q.x, q.y),0.0);
}



float SDFMultiPolyShape(float2 uv, int sides) // https://thebookofshaders.com/07/
{
	// Remap the space to -1. to 1.
	uv = uv * 2. - 1.;

	// Number of sides of your shape
	int N = max(sides, 3);

	// Angle and radius from the current pixel
	float a = atan2(uv.x, uv.y) + Custom_PI;
	float r = Custom_TWO_PI / float(N);

	// Shaping function that modulate the distance
	//Original  d = cos(floor(.5 + a / r) * r - a) * length(st);
	// saw blade somwhow //d = abs(cos(mod(a + r/2.0, r/2.0))) * length(st);

	// more understandable
	float d = cos(fmod(a + r / 2.0, r) - r / 2.0) * length(uv);

	return d;
	//color = vec3(1.0 - smoothstep(.4, .41, d));
}

//




// 3D
float SDFSphere3D(float3 pos, float radius )
{
	return length(pos) - radius;
}

float SDFBox3D(float3 pos, float3 size )
{
	float3 q = abs(pos) - size;
	return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0);
}
//


// Operations

float SDF_OP_Round(in float sdfValue, in float r)
{
	return sdfValue - r;
}

float SDF_OP_Onion(in float sdfValue, in float r)
{
	return abs(sdfValue) - r;
}

float SDF_OP_Union(float d1, float d2)
{
	return min(d1, d2);
}
float SDF_OP_Subtraction(float d1, float d2)
{
	return max(-d1, d2);
}
float SDF_OP_Intersection(float d1, float d2)
{
	return max(d1, d2);
}
float SDF_OP_Xor(float d1, float d2)
{
	return max(min(d1, d2), -max(d1, d2));
}


float SDF_OP_SmoothUnion(float d1, float d2, float k)
{
	float h = clamp(0.5 + 0.5 * (d2 - d1) / k, 0.0, 1.0);
	return lerp(d2, d1, h) - k * h * (1.0 - h);
}

float SDF_OP_SmoothSubtraction(float d1, float d2, float k)
{
	float h = clamp(0.5 - 0.5 * (d2 + d1) / k, 0.0, 1.0);
	return lerp(d2, -d1, h) + k * h * (1.0 - h);
}

float SDF_OP_SmoothIntersection(float d1, float d2, float k)
{
	float h = clamp(0.5 - 0.5 * (d2 - d1) / k, 0.0, 1.0);
	return lerp(d2, d1, h) + k * h * (1.0 - h);
}

