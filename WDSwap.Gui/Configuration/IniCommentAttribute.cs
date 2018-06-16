using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDSwap.Gui.Configuration
{
	public class IniCommentAttribute : Attribute
	{
		public List<string> Comments { get; set; }


		public IniCommentAttribute(string comment, params string[] comments)
		{
			Comments = new List<string>() { comment };

			if (comment.Any())
			{
				Comments.AddRange(comments);
			}
		}
	}
}
