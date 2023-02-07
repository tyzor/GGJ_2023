#ifndef VORONOI_INCLUDED
#define VORONOI_INCLUDED

/*
inline float2 random2(float2 st){
  st = float2( dot(st,float2(127.1,311.7)),
             dot(st,float2(269.5,183.3)) );
  return frac(sin(st)*43758.5453123);
}
*/

// Random noise functions taken from https://www.ronja-tutorials.com/post/024-white-noise/
float rand2dTo1d(float2 value, float2 dotDir = float2(12.9898, 78.233)){
    float2 smallValue = sin(value);
    float random = dot(smallValue, dotDir);
    random = frac(sin(random) * 143758.5453);
    return random;
}

float2 rand2dTo2d(float2 value){
    return float2(
        rand2dTo1d(value, float2(12.989, 78.233)),
        rand2dTo1d(value, float2(39.346, 11.135))
    );
}

inline float taxicab_dist(float2 a, float2 b)
{
    a = abs(a-b);
    return a.x + a.y;
}

// d1 is closest point, d2 is second closest, cellHash is unique identifier for cell
void Voronoi_float(float2 uv, out float d1, out float d2, out float cellHash)
{
	float2 cell;
    float2 c = float2(floor(uv));
	float d;

	d1 = 8.0;
	d2 = 8.0;

    [unroll]
	for (int i=-3; i<=3; i++) {
        [unroll]
		for (int j=-3; j<=3; j++) {
			cell = c + float2(i, j);
			d = taxicab_dist(float2(cell) + rand2dTo2d(cell), uv);
			if (d < d1) {
				d2 = d1;
				d1 = d;
                cellHash = rand2dTo1d(cell);
			} else if (d < d2) {
				d2 = d;
			}
		}
	}

}

#endif // VORONOI_INCLUDED