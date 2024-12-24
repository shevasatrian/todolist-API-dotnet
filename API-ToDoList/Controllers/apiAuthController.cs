using API_ToDoList.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using ToDoList.datamodels;
using ToDoList.viewmodels;

namespace API_ToDoList.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class apiAuthController : ControllerBase
	{

		private readonly ToDoListContext db;
		private VMResponse response = new VMResponse();
		private readonly JWTGenerateService jWTGenerate;
		private readonly PasswordService passwordService;

        public apiAuthController(ToDoListContext db, JWTGenerateService jWTGenerate, PasswordService passwordService)
        {
            this.db = db;
			this.jWTGenerate = jWTGenerate;
			this.passwordService = passwordService;
        }

		[HttpPost("Register")]
		public VMResponse Register(VMTblUser data)
		{

			string hashedPassword = passwordService.HashPassword(data.Password);

			var newUser = new TblUser
			{
				FirstName = data.FirstName,
				LastName = data.LastName,
				Email = data.Email,
				Password = hashedPassword,
				IsDelete = false,
			};

			if (newUser.FirstName == null || newUser.Email == null || newUser.Password == null)
			{
				response.Success = false;
				response.Message = "Registration failed, please fill FirstName, Email and Password";
				return response;
			}

			db.TblUsers.Add(newUser);
			db.SaveChanges();

			response.Success = true;
			response.Message = "Registration Successful.";

			return response;

		}

		[HttpPost("CheckEmailIfExist")]
		public IActionResult CheckEmail(VMCheckEmail email)
		{
			bool isExist = db.TblUsers.Any(x => x.Email == email.Email && x.IsDelete == false);
			return Ok(new { exists = isExist });
		}

		[HttpPost("Login")]
		public IActionResult Login(VMLoginRequest request)
		{

			var user = db.TblUsers.FirstOrDefault(u => u.Email == request.Email && u.IsDelete == false);

			bool isPasswordSame = passwordService.VerifyPassword(request.Password, user.Password);

			if (user != null && isPasswordSame)
			{
				var token = jWTGenerate.GenerateJWTToken(user);

				return Ok(new { Token = token });

			}

			return Unauthorized();

		}

		[HttpGet("User/me")]
		//[Authorize]
		public IActionResult GetUserProfile()
		{
			var authUser = HttpContext.Request.Headers["Authorization"].ToString();
			if (string.IsNullOrEmpty(authUser) || !authUser.StartsWith("Bearer "))
			{

				response.Success = false;
				response.Message = "Authorization token is missing or invalid.";

				return Unauthorized(response);
			} 

			// ekstrak token
			var token = authUser.Substring("Bearer ".Length).Trim();

			// validasi token dan dapatkan klaim
			var handler = new JwtSecurityTokenHandler();
			try
			{
				var jwtToken = handler.ReadJwtToken(token);
				var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
					response.Success = false;
					response.Message = "Invalid token: User ID is missing.";

					return Unauthorized(response);
                }

				// Konversi userId ke long dan cari user di database
				var userId = long.Parse(userIdClaim);
				var user = db.TblUsers.FirstOrDefault(u => u.Id == userId && u.IsDelete == false);

				if (user == null)
				{
					response.Success = false;
					response.Message = "User not found";

					return Unauthorized(response);
				}

				string profilePictureBase64 = user.ProfilePicture != null ? Convert.ToBase64String(user.ProfilePicture) : null!;

				return Ok(new {	
					user.Id,
					user.FirstName,
					user.LastName,
					user.Email,
					ProfilePicture = profilePictureBase64,
				});
			}
			catch (Exception ex)
			{
				response.Success = false;
				response.Message = ex.Message;

				return Unauthorized(response);
			}

		}

		[HttpPut("Edit")]
		public async Task<IActionResult> Edit(VMEditProfile editProfile)
		{
			var authUser = HttpContext.Request.Headers["Authorization"].ToString();
			if (string.IsNullOrEmpty(authUser) || !authUser.StartsWith("Bearer "))
			{

				response.Success = false;
				response.Message = "Authorization token is missing or invalid.";

				return Unauthorized(response);
			}

			// ekstrak token
			var token = authUser.Substring("Bearer ".Length).Trim();

			// validasi token dan dapatkan klaim
			var handler = new JwtSecurityTokenHandler();

			try
			{
				var jwtToken = handler.ReadJwtToken(token);
				var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub)?.Value;

				if (string.IsNullOrEmpty(userIdClaim))
				{
					response.Success = false;
					response.Message = "Invalid token: User ID is missing.";

					return Unauthorized(response);
				}

				// Konversi userId ke long dan cari user di database
				var userId = long.Parse(userIdClaim);
				var user = db.TblUsers.FirstOrDefault(u => u.Id == userId && u.IsDelete == false);

				if (user == null)
				{
					response.Success = false;
					response.Message = "user not found";
					return NotFound(response);
				}

				var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
				var maxFileSize = 2 * 1024 * 1024; // 2 MB

				// update first name and last name
				user.FirstName = editProfile.FirstName ?? user.FirstName;
				user.LastName = editProfile.LastName ?? "";

				// handle upload untuk foto profile
				if (editProfile.ProfilePicture != null)
				{
					var fileExtension = Path.GetExtension(editProfile.ProfilePicture.FileName).ToLower();

					// validate file format
					if (!allowedExtensions.Contains(fileExtension))
					{
						response.Success = false;
						response.Message = "Only JPG, JPEG, and PNG formats are allowed";
						return BadRequest(response);
					}

					// validate file size
					if (editProfile.ProfilePicture.Length > maxFileSize)
					{
						response.Success = false;
						response.Message = "File size should be exceed 2 mb";
						return BadRequest(response);
					}

					using (var memoryStream =  new MemoryStream())
					{
						await editProfile.ProfilePicture.CopyToAsync(memoryStream);
						user.ProfilePicture = memoryStream.ToArray();
					}
				}

				db.SaveChanges();

				response.Success = true;
				response.Message = "Profile updated successfully.";
				return Ok(response);

			}
			catch (Exception ex)
			{
				response.Success = false;
				response.Message = ex.Message;

				return Unauthorized(response);
			}

		}

    }
}
