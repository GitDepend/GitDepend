using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace GitDepend
{
	/// <summary>
	/// A utility class for DependencyInjection
	/// </summary>
	public static class DependencyInjection
	{
		/// <summary>
		/// The <see cref="UnityContainer"/>
		/// </summary>
		public static UnityContainer Container { get; set; }

		/// <summary>
		/// Resolves a type using Dependency Injection.
		/// </summary>
		/// <typeparam name="T">The type of the object to resolve.</typeparam>
		/// <returns></returns>
		public static T Resolve<T>()
		{
			return Container.Resolve<T>();
		}
	}
}
