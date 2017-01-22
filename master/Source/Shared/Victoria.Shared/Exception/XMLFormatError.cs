using System;

namespace Victoria.Shared
{
	public class XMLFormatError : Exception
	{
		public XMLFormatError(string message) : base(message)
		{
		}
	}
}

