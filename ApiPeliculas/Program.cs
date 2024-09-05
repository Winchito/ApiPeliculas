using System.Text;
using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.MoviesMapper;
using ApiPeliculas.Repositorio;
using ApiPeliculas.Repositorio.IRepositorio;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppContextDB>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SQLConnection")));

//Soporte para autenticacion con .NET Identity
builder.Services.AddIdentity<AppUsuario, IdentityRole>().AddEntityFrameworkStores<AppContextDB>();

//Soporte para cache
builder.Services.AddResponseCaching();

//Agregamos los repositorios
builder.Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddScoped<IPeliculaRepositorio, PeliculaRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

var key = builder.Configuration.GetValue<string>("ApiSettings:Secreta");

//Soporte para versionamiento
var apiVersioningBuilder = builder.Services.AddApiVersioning(opcion =>
{
    opcion.AssumeDefaultVersionWhenUnspecified = true;
    opcion.DefaultApiVersion = new ApiVersion(1,0);
    opcion.ReportApiVersions = true;
    //opcion.ApiVersionReader = ApiVersionReader.Combine(
    //    new QueryStringApiVersionReader("api-version") //? api-version=1.0
    //    //new HeaderApiVersionReader("X-Version"),
    //    //new MediaTypeApiVersionReader("ver"));
    //    );
});

apiVersioningBuilder.AddApiExplorer(opciones =>
{
    opciones.GroupNameFormat = "'v'VVV";
    opciones.SubstituteApiVersionInUrl = true;
});

//Agregamos el AutoMapper
builder.Services.AddAutoMapper(typeof(PeliculasMapper));

//Aqui se configura la autenticacion
builder.Services.AddAuthentication(
    x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }
    ).AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddControllers(opcion =>
{
    //Cache profile. Un cache global y asi no tener que ponerlo en todas partes
    opcion.CacheProfiles.Add("PorDefecto30Segundos", new CacheProfile() { Duration = 30 });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(options =>
//options.CustomSchemaIds(type => type.ToString())
//);
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
        "Autenticacion JWT usando el esquema Bearer. \r\n\r\n" +
        "Ingresa la palabra 'Bearer' seguido de un [espacio] y después su toke en el campo de abajo. \r\n\r\n" +
        "Ejemplo: \"Bearer TokenGenerado\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            },
            Scheme = "oauth2",
            Name = "Bearer",
            In = ParameterLocation.Header
        },
        new List<string>()
    }
    });

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1.0",
        Title = "Peliculas Api V1",
        Description = "Api de peliculas",
        TermsOfService = new Uri("https://google.com.co"),
        Contact = new OpenApiContact
        {
            Name = "Winchito",
            Url = new Uri("https://google.com.co")
        },
        License = new OpenApiLicense
        {
            Name = "Licencia Personal",
            Url = new Uri("https://google.com.co")
        }
    }
);
    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2.0",
        Title = "Peliculas Api V2",
        Description = "Api de peliculas Version 2",
        TermsOfService = new Uri("https://google.com.co"),
        Contact = new OpenApiContact
        {
            Name = "Winchito",
            Url = new Uri("https://google.com.co")
        },
        License = new OpenApiLicense
        {
            Name = "Licencia Personal",
            Url = new Uri("https://google.com.co")
        }
    }
    );
});

    //Soportepara CORS
    //Se pueden habilitar: 1- Un dominio, 2-Multiples Dominios,
    //3-Cualquier dominio (Tener en cuenta cualquier seguridad)
    //Usamos de ejemplo el dominio http://localhost:3223, se debe cdambiar por el correcto
    //Se usa (*) para todos los dominios
    builder.Services.AddCors(p => p.AddPolicy("PoliticaCors", build =>
        {
            build.WithOrigins("http://localhost:3223").AllowAnyMethod().AllowAnyHeader();
        }));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(opciones =>
            {
                opciones.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiPeliculasV1");
                opciones.SwaggerEndpoint("/swagger/v2/swagger.json", "ApiPeliculasV2");
            }
            );
        }


//Soporte para archivos estaticos como imagenes
app.UseStaticFiles();

app.UseHttpsRedirection();

//Soporte para CORS
app.UseCors("PoliticaCors");

//Soporte para autenticacion
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
