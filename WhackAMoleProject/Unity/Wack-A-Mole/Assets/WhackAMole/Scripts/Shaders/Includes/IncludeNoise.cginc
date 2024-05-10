

/*
BANNED VARIABLE LIST : because Unity internally uses these names?
- point


*/

// Cubic Hermite Curve.  Same as SmoothStep()
//y = x * x * (3.0 - 2.0 * x);
// Quintic interpolation curve (more "flat" connection between the cells)
//y = x * x * x * (x * (x * 6. - 15.) + 10.);


float Random(float2 uv) {
	return frac(sin(dot(uv.xy,
		float2(12.9898, 78.233))) *
		43758.5453123);
}

// Value noise by Inigo Quilez - iq/2013
// https://www.shadertoy.com/view/lsf3WH
float ValueNoise(float2 st) 
{
	float2 i = floor(st);
	float2 f = frac(st);
	float2 u = f * f * (3.0 - 2.0 * f);
	return lerp(	lerp(	Random(i + float2(0.0, 0.0)),
							Random(i + float2(1.0, 0.0)), 
							u.x),
					lerp(	Random(i + float2(0.0, 1.0)),
							Random(i + float2(1.0, 1.0)), 
							u.x),	
					u.y);
}


float2 Random2(float2 st) {
	st = float2(
		dot(st, float2(127.1, 311.7)),
		dot(st, float2(269.5, 183.3)));
	return -1.0 + 2.0 * frac(sin(st) * 43758.5453123);
}

// Gradient Noise by Inigo Quilez - iq/2013
// https://www.shadertoy.com/view/XdXGW8
float GradientNoise(float2 st) {
	float2 i = floor(st);
	float2 f = frac(st);

	float2 u = f * f * (3.0 - 2.0 * f);

	return lerp(
				lerp(	dot(Random2(i + float2(0.0, 0.0)), f - float2(0.0, 0.0)),
						dot(Random2(i + float2(1.0, 0.0)), f - float2(1.0, 0.0)),
						u.x),
				lerp(	dot(Random2(i + float2(0.0, 1.0)), f - float2(0.0, 1.0)),
						dot(Random2(i + float2(1.0, 1.0)), f - float2(1.0, 1.0)),
						u.x), 
				u.y);
}


//Simplex Noise
// Some useful functions
float3 mod289(float3 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
float2 mod289(float2 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
float3 permute(float3 x) { return mod289(((x * 34.0) + 1.0) * x); }

//
// Description : GLSL 2D simplex noise function
//      Author : Ian McEwan, Ashima Arts
//  Maintainer : ijm
//     Lastmod : 20110822 (ijm)
//     License :
//  Copyright (C) 2011 Ashima Arts. All rights reserved.
//  Distributed under the MIT License. See LICENSE file.
//  https://github.com/ashima/webgl-noise
//
float SimplexNoise(float2 v) {

	// Precompute values for skewed triangular grid
	const float4 C = float4(0.211324865405187,
		// (3.0-sqrt(3.0))/6.0
		0.366025403784439,
		// 0.5*(sqrt(3.0)-1.0)
		-0.577350269189626,
		// -1.0 + 2.0 * C.x
		0.024390243902439);
	// 1.0 / 41.0

// First corner (x0)
	float2 i = floor(v + dot(v, C.yy));
	float2 x0 = v - i + dot(i, C.xx);

	// Other two corners (x1, x2)
	float2 i1 = float2(0.0, 0.0);
	i1 = (x0.x > x0.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
	float2 x1 = x0.xy + C.xx - i1;
	float2 x2 = x0.xy + C.zz;

	// Do some permutations to avoid
	// truncation effects in permutation
	i = mod289(i);
	float3 p = permute(
		permute(i.y + float3(0.0, i1.y, 1.0))
		+ i.x + float3(0.0, i1.x, 1.0));

	float3 m = max(0.5 - float3(
		dot(x0, x0),
		dot(x1, x1),
		dot(x2, x2)
	), 0.0);

	m = m * m;
	m = m * m;

	// Gradients:
	//  41 pts uniformly over a line, mapped onto a diamond
	//  The ring size 17*17 = 289 is close to a multiple
	//      of 41 (41*7 = 287)

	float3 x = 2.0 * frac(p * C.www) - 1.0;
	float3 h = abs(x) - 0.5;
	float3 ox = floor(x + 0.5);
	float3 a0 = x - ox;

	// Normalise gradients implicitly by scaling m
	// Approximation of: m *= inversesqrt(a0*a0 + h*h);
	m *= 1.79284291400159 - 0.85373472095314 * (a0 * a0 + h * h);

	// Compute final noise value at P
	float3 g = float3(0.0, 0.0, 0.0);
	g.x = a0.x * x0.x + h.x * x0.y;
	g.yz = a0.yz * float2(x1.x, x2.x) + h.yz * float2(x1.y, x2.y);
	return 130.0 * dot(m, g);
}


//float Hash(float3 p)  // replace this by something better / OR NOT
//{
//	p  = frac( p*0.3183099+.1 );
//	p *= 17.0;
//	return frac( p.x*p.y*p.z*(p.x+p.y+p.z) );
//}
//
//float Noise(float3 x )
//{
//	float3 i = floor(x);
//	float3 f = frac(x);
//	f = f*f*(3.0-2.0*f);
//	
//	return lerp(lerp(lerp( Hash(i+float3(0,0,0)), 
//						Hash(i+float3(1,0,0)),f.x),
//					lerp( Hash(i+float3(0,1,0)), 
//						Hash(i+float3(1,1,0)),f.x),f.y),
//				lerp(lerp( Hash(i+float3(0,0,1)), 
//						Hash(i+float3(1,0,1)),f.x),
//					lerp( Hash(i+float3(0,1,1)), 
//						Hash(i+float3(1,1,1)),f.x),f.y),f.z);
//}

//float GetLayered3DNoise(float3 pos)
//{		
//	float3x3 m = float3x3( 0.00,  0.80,  0.60,
//        -0.80,  0.36, -0.48,
//        -0.60, -0.48,  0.64 );
//			
//	float3 q = pos;
//	float f = 0.0;
//
//	f  = 0.5000 * Noise( q );
//	q = mul(m, q * 2.01);
//				
//	f += 0.2500*Noise( q );
//	q = mul(m, q*2.02);
//				
//	f += 0.1250*Noise( q );
//	q = mul(m, q*2.03);
//				
//	f += 0.0625*Noise( q );
//	q = mul(m, q*2.01);
//
//	return f;
//}


//// https://www.shadertoy.com/view/4dS3Wd
#define NOISE fbm
#define NUM_NOISE_OCTAVES 5

float hash(float p) 
{ 
	p = frac(p * 0.011); 
	p *= p + 7.5; p *= p + p; 
	return frac(p); 
}

float hash(float2 p) 
{ 
	float3 p3 = frac(float3(p.xyx) * 0.13);
	p3 += dot(p3, p3.yzx + 3.333);
	return frac((p3.x + p3.y) * p3.z); 
}

float Noise(float x) {
	float i = floor(x);
	float f = frac(x);
	float u = f * f * (3.0 - 2.0 * f);
	return lerp(hash(i), hash(i + 1.0), u);
}


float Noise(float2 x) {
	float2 i = floor(x);
	float2 f = frac(x);

	// Four corners in 2D of a tile
	float a = hash(i);
	float b = hash(i + float2(1.0, 0.0));
	float c = hash(i + float2(0.0, 1.0));
	float d = hash(i + float2(1.0, 1.0));

	// Simple 2D lerp using smoothstep envelope between the values.
	// return float3(mix(mix(a, b, smoothstep(0.0, 1.0, f.x)),
	//			mix(c, d, smoothstep(0.0, 1.0, f.x)),
	//			smoothstep(0.0, 1.0, f.y)));

	// Same code, with the clamps in smoothstep and common subexpressions
	// optimized away.
	float2 u = f * f * (3.0 - 2.0 * f);
	return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
}


float Noise(float3 x) {
	const float3 step = float3(110, 241, 171);

	float3 i = floor(x);
	float3 f = frac(x);

	// For performance, compute the base input to a 1D hash from the integer part of the argument and the 
	// incremental change to the 1D based on the 3D -> 1D wrapping
	float n = dot(i, step);

	float3 u = f * f * (3.0 - 2.0 * f);
	return lerp(lerp(lerp(hash(n + dot(step, float3(0, 0, 0))), hash(n + dot(step, float3(1, 0, 0))), u.x),
		lerp(hash(n + dot(step, float3(0, 1, 0))), hash(n + dot(step, float3(1, 1, 0))), u.x), u.y),
		lerp(lerp(hash(n + dot(step, float3(0, 0, 1))), hash(n + dot(step, float3(1, 0, 1))), u.x),
		lerp(hash(n + dot(step, float3(0, 1, 1))), hash(n + dot(step, float3(1, 1, 1))), u.x), u.y), u.z);
}


//Fractal Brownian Motion
float FBM(float x) {
	float v = 0.0;
	float a = 0.5;
	float shift = float(100);
	for (int i = 0; i < NUM_NOISE_OCTAVES; ++i) {
		v += a * noise(x);
		x = x * 2.0 + shift;
		a *= 0.5;
	}
	return v;
}


float FBM(float2 x) {
	float v = 0.0;
	float a = 0.5;
	float2 shift = float2(100, 100);
	// Rotate to reduce axial bias
	float2x2 rot = float2x2(cos(0.5), sin(0.5), -sin(0.5), cos(0.50));
	for (int i = 0; i < NUM_NOISE_OCTAVES; ++i) 
	{
		v += a * noise(x);
		x = (mul(rot, x) * 2.0) + shift;
		a *= 0.5;
	}
	return v;
}


float FBM(float3 x) {
	float v = 0.0;
	float a = 0.5;
	float3 shift = float3(100, 100, 100);
	for (int i = 0; i < NUM_NOISE_OCTAVES; ++i) {
		v += a * noise(x);
		x = x * 2.0 + shift;
		a *= 0.5;
	}
	return v;
}


//https://docs.unity3d.com/Packages/com.unity.shadergraph@6.9/manual/Voronoi-Node.html
inline float2 unity_voronoi_noise_randomVector(float2 UV, float offset)
{
	float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
	UV = frac(sin(mul(UV, m)) * 46839.32);
	return float2(sin(UV.y * offset) * 0.5 + 0.5, cos(UV.x * offset) * 0.5 + 0.5);
}

void Unity_Voronoi_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells)
{
	float2 g = floor(UV * CellDensity);
	float2 f = frac(UV * CellDensity);
	float t = 8.0;
	float3 res = float3(8.0, 0.0, 0.0);

	for (int y = -1; y <= 1; y++)
	{
		for (int x = -1; x <= 1; x++)
		{
			float2 lattice = float2(x, y);
			float2 offset = unity_voronoi_noise_randomVector(lattice + g, AngleOffset);
			float d = distance(lattice + offset, f);
			if (d < res.x)
			{
				res = float3(d, offset.x, offset.y);
				Out = res.x;
				Cells = res.y;
			}
		}
	}
}


