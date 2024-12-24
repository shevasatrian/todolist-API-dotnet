using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.viewmodels
{
	public class VMResponse
	{
		public VMResponse()
		{
			Success = true;
		}

		public bool Success { get; set; }
		public string Message { get; set; }
		public object Entity { get; set; }

	}
}
