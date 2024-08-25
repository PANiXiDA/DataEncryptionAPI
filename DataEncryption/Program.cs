using Dal.SQL;
using BL.Standard;
using Tools.SymmetricEncryption;
using Microsoft.OpenApi.Models;
using Tools.HashEncryption;
using Common.Configuration;
using Tools.JsonWebTokenEncryption;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Common.Configuration.Models;
using Tools.AsymmetricEncryption;
using Tools.Certificates;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Net.Security;
using System.Security.Authentication;

var builder = WebApplication.CreateBuilder(args);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("Auth", new OpenApiInfo { Title = "Auth API", Version = "v1" });
    c.SwaggerDoc("Users", new OpenApiInfo { Title = "Users API", Version = "v1" });
    c.SwaggerDoc("SymmetricEncryption", new OpenApiInfo { Title = "Symmetric Encryption API", Version = "v1" });
    c.SwaggerDoc("AsymmetricEncryption", new OpenApiInfo { Title = "Asymmetric Encryption API", Version = "v1" });
    c.SwaggerDoc("HashEncryption", new OpenApiInfo { Title = "Hash Encryption API", Version = "v1" });
});

RsaEncryption.GeneratePrivateAndPublicKeys();

builder.Services.AddDataAccessLayer(configuration);
builder.Services.AddBusinessLogicLayer();

builder.Services.Configure<SharedConfiguration>(builder.Configuration.GetSection("SharedConfiguration"));

builder.Services.AddSingleton<AesEncryption>();
builder.Services.AddSingleton<HashEncryption>();
builder.Services.AddSingleton<JsonWebTokenEncryption>();
builder.Services.AddSingleton<RsaEncryption>();
builder.Services.AddSingleton<EcdsaEncryption>();

var sharedConfiguration = builder.Configuration.GetSection("SharedConfiguration").Get<SharedConfiguration>() ?? throw new ArgumentNullException("Shared configuration is not found");
var certificate = new Certificate(sharedConfiguration).Create();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
        httpsOptions.ClientCertificateValidation = (certificate, chain, errors) =>
        {
            if (errors != SslPolicyErrors.None)
            {
                return false;
            }

            if (chain == null)
            {
                return false;
            }

            foreach (var status in chain.ChainStatus)
            {
                if (status.Status == X509ChainStatusFlags.RevocationStatusUnknown
                || status.Status == X509ChainStatusFlags.OfflineRevocation)
                {
                    return false;
                }
            }

            return true;
        };
    });
});

builder.Services.AddHttpClient("Client")
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        var handler = new HttpClientHandler
        {
            ClientCertificates = { certificate },
            SslProtocols = SslProtocols.Tls12
        };
        return handler;
    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = configuration.GetSection("SharedConfiguration:JsonWebTokenConfiguration").Get<JsonWebTokenConfiguration>();

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings!.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите токен JWT для авторизации"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.InjectJavascript("/js/custom-swagger.js");
        c.SwaggerEndpoint("/swagger/Auth/swagger.json", "Auth API");
        c.SwaggerEndpoint("/swagger/Users/swagger.json", "Users API");
        c.SwaggerEndpoint("/swagger/SymmetricEncryption/swagger.json", "SymmetricEncryption API");
        c.SwaggerEndpoint("/swagger/AsymmetricEncryption/swagger.json", "AsymmetricEncryption API");
        c.SwaggerEndpoint("/swagger/HashEncryption/swagger.json", "HashEncryption API");
    });
}
else
{
    app.UseExceptionHandler("/api/error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
