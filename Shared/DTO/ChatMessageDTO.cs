using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO
{
	public class ChatMessageDTO
	{
		public string Username { get; set; }
		public string Message { get; set; }

	}
}
