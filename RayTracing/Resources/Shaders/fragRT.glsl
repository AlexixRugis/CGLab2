#version 430 core

struct Vertex
{
    vec3 position;
    vec3 normal;
};

struct BVHNode
{
    vec3 posMin;
    int indicesCount;
    vec3 posMax;
    int childIndex;
};

struct Ray
{
    vec3 origin;
    vec3 direction;
};

struct Material
{
    vec3 color;
    float smoothness;
    vec3 emissionColor;
    float metallic;
};

struct Hit
{
    vec3 p;
    float d;
    vec3 n;
    uint matIndex;
};

struct Sphere
{
    vec3 position;
    float radius;
    uint matIndex;
};

struct MeshInfo
{
    int nodeIndex;
    uint matIndex;

    mat4 transform;
    mat4 invTransform;
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

layout(std430, binding = 1) buffer VertexBuffer
{
    Vertex _Vertices[];
};

layout(std430, binding = 2) buffer IndexBuffer
{
    uint _Indices[];
};

layout(std430, binding = 3) buffer MeshBuffer
{
    MeshInfo _Meshes[];
};

layout(std430, binding = 4) buffer BVHBuffer
{
    BVHNode _Nodes[];
};

layout(std430, binding = 5) buffer MaterialBuffer
{
    Material _Materials[];
};

uniform sampler2D prevFrame;
uniform samplerCube _Skybox;
uniform mat4 _CameraToWorld;
uniform mat4 _CameraInverseProjection;
uniform vec2 _ScreenSize;
uniform int _Bounces;
uniform int _RaysPerPixel;
uniform uint _Frame;
uniform int _SpheresCount;
uniform int _MeshesCount;

uint wang_hash(inout uint seed)
{
    seed = uint(seed ^ uint(61)) ^ uint(seed >> uint(16));
    seed *= uint(9);
    seed = seed ^ (seed >> 4);
    seed *= uint(0x27d4eb2d);
    seed = seed ^ (seed >> 15);
    return seed;
}

float randomFloat(inout uint state)
{
    return float(wang_hash(state)) / 4294967296.0f;
}

vec3 randomDirection(inout uint state)
{
    float z = randomFloat(state) * 2.0f - 1.0f;
    float a = randomFloat(state) * 6.2831853071f;
    float r = sqrt(1.0f - z * z);
    float x = r * cos(a);
    float y = r * sin(a);
    return vec3(x, y, z);
}

Ray createCameraRay(vec2 uv)
{
    vec3 origin = (_CameraToWorld * vec4(0.0f, 0.0f, 0.0f, 1.0f)).xyz;

    vec3 direction = (_CameraInverseProjection * vec4(uv, 0.0f, 1.0f)).xyz;
    direction = (_CameraToWorld * vec4(direction, 0.0f)).xyz;
    direction = normalize(direction);

    return createRay(origin, direction);
}

Hit hitSphere(Ray ray, vec3 center, float radius)
{
    Hit hit;
    hit.d = -1.0f;

    float r2 = radius * radius;
    vec3 oc = ray.origin - center;
    vec3 cr = cross(ray.direction, oc);
    if (dot(cr, cr) > r2)
    {
        return hit;
    }

    float b = dot(oc, ray.direction);
    float c = dot(oc, oc) - r2;
    float discriminant = b * b - c;

    float dst = -b - sqrt(discriminant);
        
    hit.d = dst;
    hit.p = ray.origin + ray.direction * dst;
    hit.n = normalize(hit.p - center);

    return hit;
}

Hit hitTriangle(
    Ray ray,
    uint i0, uint i1, uint i2
)
{
    Hit hit;
    hit.d = -1.0f;

    vec3 e1 = _Vertices[i1].position - _Vertices[i0].position;
    vec3 e2 = _Vertices[i2].position - _Vertices[i0].position;
    vec3 pvec = cross(ray.direction, e2);
    float det = dot(e1, pvec);

    if (abs(det) < 1e-8f) return hit;

    float invDet = 1.0f / det;
    vec3 tvec = ray.origin - _Vertices[i0].position;
    vec3 uv;
    uv.x = dot(tvec, pvec) * invDet;
    if (uv.x < 0.0f || uv.x > 1.0f) return hit;

    vec3 qvec = cross(tvec, e1);
    uv.y = dot(ray.direction, qvec) * invDet;
    if (uv.y < 0.0f || uv.x + uv.y > 1.0f) return hit;

    uv.z = 1.0f - uv.x - uv.y;

    hit.d = dot(e2, qvec) * invDet;
    hit.p = ray.origin + ray.direction * (hit.d - 0.01f);
    hit.n = normalize(_Vertices[i0].normal * uv.z + _Vertices[i1].normal * uv.x + _Vertices[i2].normal * uv.y);

    return hit;
}

bool checkAABB(Ray ray, vec3 minVec, vec3 maxVec, inout float largestTMin)
{
    vec3 invDir = 1.0 / ray.direction;
    vec3 t1 = (minVec - ray.origin) * invDir;
    vec3 t2 = (maxVec - ray.origin) * invDir;

    vec3 tMin = min(t1, t2);
    vec3 tMax = max(t1, t2);

    largestTMin = max(max(tMin.x, tMin.y), tMin.z);
    float smallestTMax = min(min(tMax.x, tMax.y), tMax.z);

    return largestTMin <= smallestTMax && smallestTMax >= 0.0;
}

Hit traverseBVH(Ray ray, int index)
{
    Hit closest;
    closest.d = 1e10f;

    int stack[32];
    int stackSize = 0;

    stack[stackSize++] = index;

    while (stackSize > 0)
    {
        stackSize--;
        BVHNode cur = _Nodes[stack[stackSize]];

        float dist;
        if (checkAABB(ray, cur.posMin, cur.posMax, dist) && dist <= closest.d)
        {
            if (cur.indicesCount == 0)
            {
                vec3 c1 = (_Nodes[cur.childIndex].posMin + _Nodes[cur.childIndex].posMax) * 0.5f;
                vec3 c2 = (_Nodes[cur.childIndex].posMin + _Nodes[cur.childIndex + 1].posMax) * 0.5f;

                if (dot(ray.origin, c1) <= dot(ray.origin, c2))
                {
                    stack[stackSize++] = cur.childIndex;
                    stack[stackSize++] = cur.childIndex + 1;
                }
                else
                {
                    stack[stackSize++] = cur.childIndex + 1;
                    stack[stackSize++] = cur.childIndex;
                }
            }
            else
            {
                for (int i = cur.childIndex; i < cur.childIndex + cur.indicesCount; i += 3)
                {
                    Hit h = hitTriangle(ray,
                        _Indices[i],
                        _Indices[i + 1],
                        _Indices[i + 2]);

                    if (h.d >= 0.0f && h.d < closest.d)
                    {
                        closest = h;
                        closest.matIndex = _Meshes[i].matIndex;
                    }
                }

            }
        }
    }

    if (closest.d == 1e10f) closest.d = -1.0f;

    return closest;
}

Hit calculateCollision(Ray ray)
{
    Hit closest;
    closest.d = 1e10f;
    
    for (int i = 0; i < _SpheresCount; i++)
    {
        Hit h = hitSphere(ray, _Spheres[i].position, _Spheres[i].radius);

        if (h.d >= 0.0f && h.d < closest.d)
        {
            closest = h;
            closest.matIndex = _Spheres[i].matIndex;
        }
    }

    for (int i = 0; i < _MeshesCount; i++)
    {
        Ray loc;
        loc.origin = (_Meshes[i].invTransform * vec4(ray.origin, 1.0)).xyz;
        loc.direction = (_Meshes[i].invTransform * vec4(ray.direction, 0.0)).xyz;

        Hit h = traverseBVH(loc, _Meshes[i].nodeIndex);
        if (h.d >= 0.0f)
        {
            h.p = (_Meshes[i].transform * vec4(h.p, 1.0)).xyz;
            h.n = transpose(mat3(_Meshes[i].invTransform)) * h.n;
            h.d = length(h.p - ray.origin);

            if (h.d < closest.d)
            {
                closest = h;
                closest.matIndex = _Meshes[i].matIndex;
            }
        }
    }

    return closest;
}

vec3 fresnelSchlick(float cosTheta, vec3 F0)
{
    return F0 + (1.0f - F0) * pow(1.0f - cosTheta, 5.0f);
}

vec3 traceRay(Ray ray, inout uint state)
{
    vec3 incomingLight = vec3(0.0f);
    vec3 color = vec3(1.0f);

    for (int i = 0; i < _Bounces; i++)
    {
        Hit hit = calculateCollision(ray);
        if (hit.d < 1e9f)
        {
            ray.origin = hit.p;

            vec3 dirDiffuse = normalize(hit.n + randomDirection(state));
            vec3 dirSpecular = reflect(ray.direction, hit.n);

            Material mat = _Materials[hit.matIndex];

            incomingLight += mat.emissionColor * color;

            float cosTheta = max(dot(hit.n, -ray.direction), 0.0f);

            vec3 F0 = mix(vec3(0.04), mat.color, mat.metallic);
            vec3 fresnel = fresnelSchlick(cosTheta, F0);
            float reflectProb = dot(fresnel, vec3(0.2126f, 0.7152f, 0.0722f));

            float isSpecular = float(randomFloat(state) < reflectProb);
            ray.direction = mix(dirDiffuse, dirSpecular, mat.smoothness * isSpecular);
            color *= mix(vec3(1.0f), mat.color, max(1.0f - isSpecular, mat.metallic));
            
        } else
        {
            incomingLight += texture(_Skybox, ray.direction).rgb * color;
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
    vec2 texCoords = (vec2(0.5f) + glPosition.xy * 0.5f);
    ivec2 pixelCoords = ivec2(texCoords * _ScreenSize);

    uint state = uint(uint(pixelCoords.x) * uint(1973) + uint(pixelCoords.y) * uint(9277) + uint(_Frame) * uint(26699)) | uint(1);

    Ray ray = createCameraRay(uv);
    
    vec3 light = vec3(0.0f);
    for (int rayIndex = 0; rayIndex < _RaysPerPixel; rayIndex++)
    {
        state += 1456;
        light += traceRay(ray, state);
    }
    light = light / int(_RaysPerPixel);

    vec4 previous = texture(prevFrame, texCoords);
    FragColor = mix(previous, vec4(light, 1.0f), 1.0f / float(_Frame + 1));
}