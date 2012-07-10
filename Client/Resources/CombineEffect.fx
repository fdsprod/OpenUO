float4x4 WorldViewProjection : WORLDVIEWPROJECTION;

texture WorldTexture;
texture UITexture;

sampler2D WorldTextureSampler = sampler_state {
    Texture = (WorldTexture);
    MinFilter = POINT;
    MagFilter = POINT;
    AddressU = Clamp;
    AddressV = Clamp;
};

sampler2D UITextureSampler = sampler_state {
    Texture = (UITexture);
    MinFilter = POINT;
    MagFilter = POINT;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct VS_INPUT
{
    float4 Position	: POSITION;
	float2 TexCoord	: TEXCOORD0;	
};

struct VS_OUTPUT
{
    float4 Position	: POSITION;
	float2 TexCoord	: TEXCOORD0;
};

VS_OUTPUT Combine_VertexShader(VS_INPUT IN)
{
    VS_OUTPUT OUT = (VS_OUTPUT)0;

    OUT.Position = mul(IN.Position, WorldViewProjection);
	OUT.TexCoord = IN.TexCoord;

    return OUT;
}

float4 Combine_PixelShader(VS_OUTPUT IN) : COLOR
{
	float4 worldColor = tex2D(WorldTextureSampler, IN.TexCoord);
	float4 uiColor = tex2D(UITextureSampler, IN.TexCoord);

	return float4((worldColor.rgb * (1 - uiColor.a)) + (uiColor.rgb * uiColor.a), 1);
}

technique CombineTechnique
{
	pass Pass0
	{ 
		VertexShader = compile vs_2_0 Combine_VertexShader(); 
		PixelShader = compile ps_2_0 Combine_PixelShader(); 
	}
}