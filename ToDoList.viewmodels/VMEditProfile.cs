using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.viewmodels
{
	public class VMEditProfile
	{
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public IFormFile? ProfilePicture { get; set; }
	}
}
