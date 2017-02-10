using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDepend
{
	public static class ReturnCodes
	{
		public const int Success = 0;
		public const int GitDependFileNotFound = 1;
		public const int GitRepositoryNotFound = 2;
	}
}
