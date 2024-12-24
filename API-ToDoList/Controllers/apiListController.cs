using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ToDoList.datamodels;
using ToDoList.viewmodels;

namespace API_ToDoList.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class apiListController : ControllerBase
	{

		private readonly ToDoListContext db;
		private VMResponse response = new VMResponse();

		public apiListController(ToDoListContext _db)
        {
            db = _db;
        }

		[HttpGet("GetAllList")]
		[Authorize]
		public List<TblList> GetAllList()
		{
			var authUser = HttpContext.Request.Headers["Authorization"].ToString();

			// ekstrak token
			var token = authUser.Substring("Bearer ".Length).Trim();

			// validasi token dan dapatkan klaim
			var handler = new JwtSecurityTokenHandler();

			var jwtToken = handler.ReadJwtToken(token);
			var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub)?.Value;

			var userId = long.Parse(userIdClaim);

			List<TblList> data = db.TblLists
				.Where(x => x.UserId == userId && x.IsDelete == false)
				.OrderByDescending(x => x.CreatedOn)
				.ToList();

			return data;
		}

		[HttpPost("CreateList")]
		[Authorize]
		public IActionResult CreateList(VMTblList list)
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

			var jwtToken = handler.ReadJwtToken(token);
			var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub)?.Value;

			var userId = long.Parse(userIdClaim);

			var data = new TblList
			{
				UserId = userId,
				Note = list.Note,
				Color = list.Color,
				IsDelete = false,
				CreatedBy = userId,
				CreatedOn = DateTime.Now
			};
			
			db.TblLists.Add(data);
			db.SaveChanges();
			return Ok(data);
		}

		[HttpPut("UpdateList/{id}")]
		public IActionResult UpdateList(int id, VMTblList list)
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

			var jwtToken = handler.ReadJwtToken(token);
			var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub)?.Value;

			var userId = long.Parse(userIdClaim);
			TblList dt = db.TblLists.FirstOrDefault(a => a.Id == id && a.UserId == userId)!;

			if (dt == null)
			{
				response.Success = false;
				response.Message = "List not found or access denied.";

				return NotFound(response);
			}

			dt.Note = list.Note;
			//dt.Color = list.Color;
			dt.ModifiedBy = userId;
			dt.ModifiedOn = DateTime.Now;
			db.Update(dt);
			db.SaveChanges();

			response.Success = true;
			response.Message = "Update Successful.";

			return Ok(new { Message = response, Data = dt });

		}

		[HttpDelete("DeleteList/{id}")]
		public VMResponse DeleteList(int id)
		{
			var authUser = HttpContext.Request.Headers["Authorization"].ToString();

			// ekstrak token
			var token = authUser.Substring("Bearer ".Length).Trim();

			// validasi token dan dapatkan klaim
			var handler = new JwtSecurityTokenHandler();

			var jwtToken = handler.ReadJwtToken(token);
			var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub)?.Value;

			var userId = long.Parse(userIdClaim);
			TblList dt = db.TblLists.FirstOrDefault(a => a.Id == id && a.UserId == userId)!;

			if (dt != null)
			{
				dt.IsDelete = true;
				dt.DeletedBy = userId;
				dt.DeletedOn = DateTime.Now;

				try
				{
					db.Update(dt);
					db.SaveChanges();

					response.Message = "Data success delete";
				}
				catch (Exception ex)
				{
					response.Success = false;
					response.Message = "Failed deleted : " + ex.Message;
				}
			} else
			{
				response.Success = false;
				response.Message = "Data not found";
			}

			return response;
		}

	}
}
