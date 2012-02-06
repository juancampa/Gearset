struct VertexToPixel
{
    float4 Position   		: POSITION;
    float4 Color			: COLOR0;
    float  LightingFactor	: TEXCOORD0;
    float2 TextureCoords	: TEXCOORD1;
	float3 ViewDirection	: TEXCOORD2;
	float3 LightDirection	: TEXCOORD3;
	float3 Normal			: TEXCOORD4;
};

struct PixelToFrame
{
    float4 Color : COLOR0;
};

//------- Constants --------
float4x4 View;
float4x4 Projection;
float4x4 World;
float3 LightPosition;
float3 CameraPosition;
float AmbientLight;
float4 Color;
bool EnableLighting;

static float4 LightSpecularColor = {1.0f,1.0f,1.0f,1.0f};

//------- Texture Samplers --	------

Texture xTexture;
sampler TextureSampler = sampler_state { texture = <xTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

//------- Technique: Pretransformed --------

VertexToPixel PretransformedVS( float4 inPos : POSITION, float4 inColor: COLOR)
{	
	VertexToPixel Output = (VertexToPixel)0;
	
	Output.Position = inPos;
	Output.Color = inColor;
    
	return Output;    
}

PixelToFrame PretransformedPS(VertexToPixel PSIn) 
{
	PixelToFrame Output = (PixelToFrame)0;		
	
	Output.Color = PSIn.Color;

	return Output;
}

technique Pretransformed
{
	pass Pass0
    {   
    	VertexShader = compile vs_1_1 PretransformedVS();
        PixelShader  = compile ps_1_1 PretransformedPS();
    }
}

//------- Technique: Colored --------

VertexToPixel ColoredVS( float4 inPos : POSITION, float4 inColor: COLOR, float3 inNormal: NORMAL)
{	
	VertexToPixel Output = (VertexToPixel)0;
	float4x4 preViewProjection = mul (View, Projection);
	float4x4 preWorldViewProjection = mul (World, preViewProjection);

    float3 ObjectPosition = mul(inPos, World);
	Output.ViewDirection = normalize(CameraPosition - inPos);
	Output.LightDirection = normalize(LightPosition - inPos);
	Output.Position = mul(inPos, preWorldViewProjection);
	Output.Color = Color;
	
	float3 Normal = normalize(mul(normalize(inNormal), World));	
	Output.Normal = Normal;
	Output.LightingFactor = 1;
	if (EnableLighting)
		Output.LightingFactor = dot(Normal, normalize(Output.LightDirection) );
    
	return Output;    
}

PixelToFrame ColoredPS(VertexToPixel PSIn) 
{
	PixelToFrame Output = (PixelToFrame)0;		
    
	if (EnableLighting)
	{
		float alpha = PSIn.Color.a;
		Output.Color.rgb = PSIn.Color.rgb * clamp(PSIn.LightingFactor + AmbientLight,0,1);
		Output.Color.a = alpha;

		float NDotL				= dot(normalize(PSIn.Normal), PSIn.LightDirection);
		float3 Reflection		= normalize(2.0f * NDotL * normalize(PSIn.Normal) - PSIn.LightDirection);
		float RDotV				= max(0.0f, dot(Reflection, PSIn.ViewDirection) );	
		float4 TotalSpecular	= Color * 2 * pow(RDotV, 5);
		Output.Color			= saturate(Output.Color + TotalSpecular);
	} else {
		Output.Color			= PSIn.Color;
	}

	/*if (PSIn.Normal.x == 0)
		Output.Color.g = 1;
	Output.Color.r			= NDotL;*/
//	Output.Color.rgb += dot(

	
	return Output;
}

technique Colored
{
	pass Pass0
    {   
    	//VertexShader = compile vs_1_1 ColoredVS();
        //PixelShader  = compile ps_1_1 ColoredPS();
        VertexShader = compile vs_3_0 ColoredVS();
        PixelShader  = compile ps_3_0 ColoredPS();

    }
}

//------- Technique: Textured --------

VertexToPixel TexturedVS( float4 inPos : POSITION, float3 inNormal: NORMAL, float2 inTexCoords: TEXCOORD0)
{	
	VertexToPixel Output = (VertexToPixel)0;
	float4x4 preViewProjection = mul (View, Projection);
	float4x4 preWorldViewProjection = mul (World, preViewProjection);
    
	Output.Position = mul(inPos, preWorldViewProjection);	
	Output.TextureCoords = inTexCoords;

	float3 LightDirection = LightPosition - Output.Position;	
	float3 Normal = normalize(mul(normalize(inNormal), World));	
	Output.LightingFactor = 1;
	if (EnableLighting)
		Output.LightingFactor = dot(Normal, -LightDirection);
    
	return Output;    
}

PixelToFrame TexturedPS(VertexToPixel PSIn) 
{
	PixelToFrame Output = (PixelToFrame)0;		
	
	Output.Color = tex2D(TextureSampler, PSIn.TextureCoords)*clamp(PSIn.LightingFactor,0,1);

	return Output;
}

technique Textured
{
	pass Pass0
    {   
    	VertexShader = compile vs_2_0 TexturedVS();
        PixelShader  = compile ps_2_0 TexturedPS();
    }
}

//------- Technique: PointSprites --------

struct SpritesVertexToPixel
{
    float4 Position   	: POSITION;
    float4 Color    	: COLOR0;
    float1 Size 		: PSIZE;
};

SpritesVertexToPixel PointSpritesVS (float4 Position : POSITION, float4 Color : COLOR0, float1 Size : PSIZE)
{
    SpritesVertexToPixel Output = (SpritesVertexToPixel)0;
     
    float4x4 preViewProjection = mul (View, Projection);
	float4x4 preWorldViewProjection = mul (World, preViewProjection); 
    Output.Position = mul(Position, preWorldViewProjection);    
    Output.Color = Color;
    Output.Size = 1/(pow(Output.Position.z,2)+1	) * Size;
    
    return Output;    
}

PixelToFrame PointSpritesPS(SpritesVertexToPixel PSIn, float2 TexCoords  : TEXCOORD0)
{ 
    PixelToFrame Output = (PixelToFrame)0;

    Output.Color = tex2D(TextureSampler, TexCoords);
    
    return Output;
}

technique PointSprites
{
	pass Pass0
    {   
    	PointSpriteEnable = true;
    	//VertexShader = compile vs_1_1 PointSpritesVS();
        //PixelShader  = compile ps_1_1 PointSpritesPS();
	VertexShader = compile vs_2_0 PointSpritesVS();
        PixelShader  = compile ps_2_0 PointSpritesPS();
	
    }
}

