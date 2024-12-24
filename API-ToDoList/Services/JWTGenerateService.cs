using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ToDoList.datamodels;

namespace API_ToDoList.Services
{
	public class JWTGenerateService
	{

		private readonly IConfiguration configuration;

        public JWTGenerateService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GenerateJWTToken(TblUser data)
		{
			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, data.Id.ToString()),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim("email", data.Email)
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: configuration["Jwt:Issuer"],
				audience: configuration["Jwt:Audience"],
				claims: claims,
				expires: DateTime.Now.AddDays(1),
				signingCredentials: credentials);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

	}
}
