float4x4 WorldViewProjection : WORLDVIEWPROJECTION;

texture Texture : TEXTURE;

sampler2D TextureSampler = sampler_state {
    Texture = (Texture);
    MinFilter = POINT;
    MagFilter = POINT;
    AddressU = Wrap;
    AddressV = Wrap;
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

VS_OUTPUT Diffuse_VertexShader(VS_INPUT IN)
{
    VS_OUTPUT OUT = (VS_OUTPUT)0;

    OUT.Position = mul(IN.Position, WorldViewProjection);
	OUT.TexCoord = IN.TexCoord;

    return OUT;
}

float4 Diffuse_PixelShader(VS_OUTPUT IN) : COLOR
{
	float4 diffuse = tex2D(TextureSampler, IN.TexCoord);
	return diffuse;
}

technique DiffuseTechnique
{
	pass Pass0
	{ 
		VertexShader = compile vs_2_0 Diffuse_VertexShader(); 
		PixelShader = compile ps_2_0 Diffuse_PixelShader(); 
	}
}