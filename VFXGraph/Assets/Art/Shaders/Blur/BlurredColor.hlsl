void BlurredColor_float(float4 Seed, float Min, float Max, float BlurX, float BlurY, out float4 Out)
{
    // We take a random value from the Random Range node function
    float randomno = frac(sin(dot(Seed.xy, float2(12.9898, 78.233))) * 43758.5453);

    // Then we create noise from it
    float noise = lerp(Min, Max, randomno);

    // We create two variables with sine and cosine of noise to create an area of blur
    float uvx = float(sin(noise)) * BlurX;
    float uvy = float(cos(noise)) * BlurY;

    // We finally add the noise to the UV's to create the blurred image
    float4 uvpos = float4(Seed.x + uvx, Seed.y + uvy, Seed.zw);

    // We pass the UV's and screen position as output
    Out = uvpos;
}