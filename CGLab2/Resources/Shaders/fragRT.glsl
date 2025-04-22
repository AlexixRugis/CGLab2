#version 430 core

struct Ray
{
    vec3 origin;
    vec3 direction;
};

struct Material
{
    vec3 color;
    vec3 emissionColor;
    float emissionStrength;
};

struct Hit
{
    float d;
    vec3 p;
    vec3 n;
    Material mat;
};

struct Sphere
{
    vec3 position;
    float radius;
    Material mat;
};

Ray createRay(vec3 origin, vec3 direction)
{
    Ray r;
    r.origin = origin;
    r.direction = direction;
    return r;
}

layout(std430, binding = 0) buffer SphereBuffer
{
    Sphere _Spheres[];
};

uniform sampler2D prevFrame;
uniform float _AccumFactor;
uniform mat4 _CameraToWorld;
uniform mat4 _CameraInverseProjection;
uniform vec2 _ScreenSize;
uniform uint _Frame;

float randomValue(inout uint state)
{
    state *= (state + 195439) * (state + 124395) * (state + 845921);
    return state / 4294967295.0;
}

float randomNormalDistribution(inout uint state)
{
    float theta = 2.0 * 3.1215926 * randomValue(state);
    float rho = sqrt(-2.0 * log(randomValue(state)));
    return rho * cos(theta);
}

vec3 randomDirection(inout uint state)
{
    float x = randomNormalDistribution(state);
    float y = randomNormalDistribution(state);
    float z = randomNormalDistribution(state);
    return normalize(vec3(x, y, z));
}

vec3 randomHemisphere(vec3 normal, inout uint state)
{
    vec3 dir = randomDirection(state);
    float d = dot(dir, normal);
    return dir * sign(d);
}

Ray createCameraRay(vec2 uv)
{
    vec3 origin = (_CameraToWorld * vec4(0.0, 0.0, 0.0, 1.0)).xyz;

    vec3 direction = (_CameraInverseProjection * vec4(uv, 0.0, 1.0)).xyz;
    direction = (_CameraToWorld * vec4(direction, 0.0)).xyz;
    direction = normalize(direction);

    return createRay(origin, direction);
}

Hit hitSphere(Ray ray, vec3 center, float radius)
{
    Hit hit;
    hit.d = -1.0;

    vec3 oc = ray.origin - center;
    float a = dot(ray.direction, ray.direction);
    float b = 2.0 * dot(oc, ray.direction);
    float c = dot(oc, oc) - radius * radius;
    float discriminant = b * b - 4.0 * a * c;

    if (discriminant >= 0.0)
    {
        float dst = (-b - sqrt(discriminant)) / (2.0 * a);
        
        if (dst >= 0.0)
        {
            hit.d = dst;
            hit.p = ray.origin + ray.direction * dst;
            hit.n = normalize(hit.p - center);
        }
    }

    return hit;
}

Hit calculateCollision(Ray ray)
{
    Hit closest;
    closest.d = 1e10;
    
    for (int i = 0; i < 4; i++)
    {
        Sphere s = _Spheres[i];
        Hit h = hitSphere(ray, s.position, s.radius);

        if (h.d >= 0.0 && h.d < closest.d)
        {
            closest = h;
            closest.mat = s.mat;
        }
    }

    return closest;
}

vec3 traceRay(Ray ray, inout uint state)
{
    vec3 incomingLight = vec3(0.0);
    vec3 color = vec3(1.0);

    for (int i = 0; i <= 30; i++)
    {
        Hit hit = calculateCollision(ray);
        if (hit.d < 1e9)
        {
            ray.origin = hit.p;
            ray.direction = randomHemisphere(hit.n, state);

            Material mat = hit.mat;
            vec3 emittedLight = mat.emissionColor * mat.emissionStrength;
            incomingLight += emittedLight * color;
            color = color * hit.mat.color;
        }
        else
        {
            break;
        }
    }

    return incomingLight;
}

out vec4 FragColor;
in vec3 glPosition;

void main()
{
    vec2 uv = glPosition.xy;
    vec2 texCoords = (vec2(0.5) + glPosition.xy * 0.5);
    ivec2 pixelCoords = ivec2(texCoords * _ScreenSize);
    uint state = pixelCoords.y * int(_ScreenSize.x) + pixelCoords.x + _Frame * 1234;

    Ray ray = createCameraRay(uv);
    
    vec3 light = vec3(0.0f);
    for (int rayIndex = 0; rayIndex < 100; rayIndex++)
    {
        state += 1456;
        light += traceRay(ray, state);
    }
    light = light / 100.0;

    vec4 previous = texture(prevFrame, texCoords);

    FragColor = mix(vec4(light, 1.0), previous, _AccumFactor);
}