using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserWebApp.Integration.Models
{
	public class PagedResponse<T>
	{
		public int Page { get; set; }
		public int Total { get; set; }
		public List<T> Data { get; set; }
	}
}
