using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ToDoDbContext>();

builder.Services.AddCors(options => options.AddPolicy("Policy", builder =>
 builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
        };
    });


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Policy");

app.UseAuthentication();

app.UseAuthorization();

//שליפת כל המשימות
app.MapGet("/items", (ToDoDbContext context) => Results.Ok(context.Items)).RequireAuthorization();

//שליפת משימה ע"פ מזהה
app.MapGet("/items/{id}", async (int id, ToDoDbContext context) =>
{
    var item = await context.Items.FindAsync(id);
    await context.SaveChangesAsync();
    return Results.Ok();
}).RequireAuthorization();

// הוספת משימה חדשה
app.MapPost("/items", async (ToDoDbContext context, Item item) =>
{
    await context.Items.AddAsync(item);
    await context.SaveChangesAsync();
    return Results.Created("/items/" + item.Id, item);
}).RequireAuthorization();

// עדכון משימה
app.MapPut("/items/{id}", async (int id, ToDoDbContext context, Item updatedItem) =>
{
    var existingItem = await context.Items.FindAsync(id);
    if (existingItem == null)
        return Results.NotFound();

    if (updatedItem.Name is not null)
        existingItem.Name = updatedItem.Name;
    existingItem.IsComplete = updatedItem.IsComplete;

    await context.SaveChangesAsync();

    return Results.NoContent();
}).RequireAuthorization();

// מחיקת משימה
app.MapDelete("/items/{id}", async (int id, ToDoDbContext context) =>
{
    var item = await context.Items.FindAsync(id);
    if (item == null)
        return Results.NotFound();

    context.Items.Remove(item);
    await context.SaveChangesAsync();

    return Results.NoContent();
}).RequireAuthorization();

app.MapPost("/login", (IConfiguration _configuration, ToDoDbContext context, User user) =>
{
    var userFromDb = context.Users.FirstOrDefault(u => u.Name == user.Name && u.Password == user.Password);
    if (userFromDb is null)
        return Results.Unauthorized();

    var jwt = CreateJWT(_configuration, userFromDb);
    return Results.Ok(jwt);
});

app.MapPost("/signin", async (IConfiguration _configuration, ToDoDbContext context, User user) =>
{
    await context.Users.AddAsync(user);
    await context.SaveChangesAsync();

    var jwt = CreateJWT(_configuration, user);
    return Results.Ok(jwt);
});

app.Run();

static object CreateJWT(IConfiguration _configuration, User user)
{
    var claims = new List<Claim>()
    {
        new("userName", user.Name),
        new("role", user.Password)
    };

    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWT:Key")));
    var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
    var tokeOptions = new JwtSecurityToken(
        issuer: _configuration.GetValue<string>("JWT:Issuer"),
        audience: _configuration.GetValue<string>("JWT:Audience"),
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: signinCredentials
    );
    var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
    return new { Token = tokenString };
}