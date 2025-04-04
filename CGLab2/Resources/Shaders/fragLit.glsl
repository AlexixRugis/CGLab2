#version 410 core

uniform sampler2D texture0;

uniform vec3 AmbientColor;
uniform vec3 LightColor;
uniform vec3 LightPos;
uniform vec3 ViewPos;

uniform vec4 Color;

out vec4 FragColor;
in vec3 normal;
in vec3 fragPos;
in vec2 texCoord;

void main()
{
    vec3 norm = normalize(normal);
    vec3 lightDir = normalize(LightPos - fragPos);


    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * LightColor;

    float specularStrength = 0.5;
    vec3 viewDir = normalize(ViewPos - fragPos);
    vec3 reflectDir = reflect(-lightDir, norm);

    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
    vec3 specular = specularStrength * spec * LightColor;

    vec4 c = vec4(diffuse + specular + AmbientColor, 1.0) * Color * texture(texture0, texCoord);

    if (c.a < 1.0f)
    {
        discard;
    }

    FragColor = c;
}