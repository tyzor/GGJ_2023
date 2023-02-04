#ifndef VORONOI_INCLUDED
#define VORONOI_INCLUDED

inline float2 random2(float2 st){
  st = float2( dot(st,float2(127.1,311.7)),
             dot(st,float2(269.5,183.3)) );
  return frac(sin(st)*43758.5453123);
}

inline float taxicab_dist(float2 a, float2 b)
{
    a = abs(a-b);
    return a.x + a.y;
}

// d1 is closest point, d2 is second closest
void Voronoi_float(float2 input, out float d1, out float d2)
{
	float2 test;
    float2 c = float2(floor(input));
	float d;

	d1 = 2.0;
	d2 = 2.0;

	for (int i=-3; i<=3; i++) {
		for (int j=-3; j<=3; j++) {
			test = c + float2(i, j);
			d = taxicab_dist(float2(test) + random2(test), input);
			if (d < d1) {
				d2 = d1;
				d1 = d;
			} else if (d < d2) {
				d2 = d;
			}
		}
	}
/*
    float2 g = floor(input);
    float2 f = frac(input);
    float t = 8.0;
    float3 res = float3(1.0, 0.0, 0.0);

    for(int y=-1; y<=1; y++)
    {
        for(int x=-1; x<=1; x++)
        {
            float2 lattice = float2(x,y);
            float2 offset = random2(lattice + g);
            //float d = distance(lattice + offset, f);
            float2 loc = lattice + offset - f;
            float d = loc.x+loc.y;
            if(d < res.x)
            {
                res = float3(d, offset.x, offset.y);
                output = res.x;
            }
        }
    }
    */
}

#endif // VORONOI_INCLUDED