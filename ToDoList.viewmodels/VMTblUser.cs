using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.viewmodels
{
	public class VMTblUser
	{
		public string Email { get; set; } = null!;

		public string FirstName { get; set; } = null!;

		public string? LastName { get; set; }

		public string Password { get; set; } = null!;
	}
}
