// This file was taken from the Telerik.JustMock.Unity project (https://github.com/tailsu/Telerik.JustMock.Unity).
// As of Feb 15, 2017 This project does not support Unity 4.0.1.
// https://github.com/tailsu/Telerik.JustMock.Unity/pull/2 has been submitted to fix this issue.
// 
// This file is licensed under the Apache 2.0 License. A copy of this license can be found at the
// root of this repository in the APACHE_LICENSE file.

using System;
using System.Linq.Expressions;
using Microsoft.Practices.Unity;
using Telerik.JustMock;
using Telerik.JustMock.Expectations;
using Telerik.JustMock.Helpers;

namespace GitDepend.UnitTests
{
	/// <summary>
	/// Extension methods for IUnityContainer that add mocking, arranges and asserts.
	/// </summary>
	public static class MockingUnityContainerExtensions
	{
		/// <summary>
		/// Registers a mocked type.
		/// </summary>
		/// <typeparam name="T">The type of mock to register.</typeparam>
		/// <param name="container">The mocking container.</param>
		/// <param name="name">Optional contract name identifying the mock.</param>
		/// <returns>The mock instance.</returns>
		public static T RegisterMock<T>(this IUnityContainer container, string name = null)
		{
			return container.GetMockingExtension().RegisterMock<T>(name);
		}

		/// <summary>
		/// Registers a mocked type.
		/// </summary>
		/// <param name="type">The type of mock to register.</param>
		/// <param name="container">The mocking container.</param>
		/// <param name="name">Optional contract name identifying the mock.</param>
		/// <returns>The mock instance.</returns>
		public static object RegisterMock(this IUnityContainer container, Type type, string name = null)
		{
			return container.GetMockingExtension().RegisterMock(type, name);
		}

		/// <summary>
		/// Enables implicit mocking when resolving unregistered dependencies. RegisterMock, Arrange and Assert also enable this behavior,
		/// so normally you don't have to call this method.
		/// </summary>
		/// <param name="container">The mocking container.</param>
		/// <returns>The same mocking container for fluently chaining calls.</returns>
		public static IUnityContainer EnableMocking(this IUnityContainer container)
		{
			container.GetMockingExtension();
			return container;
		}

		/// <summary>
		/// Register a mocked type and arrange a method on it.
		/// </summary>
		/// <typeparam name="TDependency">The type of mock to register.</typeparam>
		/// <param name="container">The mocking container.</param>
		/// <param name="expression">The method to arrange.</param>
		/// <returns>Fluent interface to further configure the behavior of this arrangement.</returns>
		public static FuncExpectation<object> Arrange<TDependency>(this IUnityContainer container, Expression<Func<TDependency, object>> expression)
		{
			return container.Arrange(null, expression);
		}

		/// <summary>
		/// Register a mocked type and arrange a method on it.
		/// </summary>
		/// <typeparam name="TDependency">The type of mock to register.</typeparam>
		/// <param name="container">The mocking container.</param>
		/// <param name="name">The contract name of the mock register.</param>
		/// <param name="expression">The method to arrange.</param>
		/// <returns>Fluent interface to further configure the behavior of this arrangement.</returns>
		public static FuncExpectation<object> Arrange<TDependency>(this IUnityContainer container, string name, Expression<Func<TDependency, object>> expression)
		{
			return container.RegisterMock<TDependency>(name).Arrange(expression);
		}

		/// <summary>
		/// Register a mocked type and arrange a method on it.
		/// </summary>
		/// <typeparam name="TDependency">The type of mock to register.</typeparam>
		/// <param name="container">The mocking container.</param>
		/// <param name="expression">The method to arrange.</param>
		/// <returns>Fluent interface to further configure the behavior of this arrangement.</returns>
		public static ActionExpectation Arrange<TDependency>(this IUnityContainer container, Expression<Action<TDependency>> expression)
		{
			return container.Arrange(null, expression);
		}

		/// <summary>
		/// Register a mocked type and arrange a method on it.
		/// </summary>
		/// <typeparam name="TDependency">The type of mock to register.</typeparam>
		/// <param name="container">The mocking container.</param>
		/// <param name="name">The contract name of the mock register.</param>
		/// <param name="expression">The method to arrange.</param>
		/// <returns>Fluent interface to further configure the behavior of this arrangement.</returns>
		public static ActionExpectation Arrange<TDependency>(this IUnityContainer container, string name, Expression<Action<TDependency>> expression)
		{
			return container.RegisterMock<TDependency>(name).Arrange(expression);
		}

		/// <summary>
		/// Register a mocked type and arrange a setter or event on it.
		/// </summary>
		/// <typeparam name="TDependency">The type of mock to register.</typeparam>
		/// <param name="container">The mocking container.</param>
		/// <param name="action">The setter or event to arrange.</param>
		/// <returns>Fluent interface to further configure the behavior of this arrangement.</returns>
		public static ActionExpectation ArrangeSet<TDependency>(this IUnityContainer container, Action<TDependency> action)
		{
			return container.RegisterMock<TDependency>().ArrangeSet(action);
		}

		/// <summary>
		/// Register a mocked type and arrange a setter or event on it.
		/// </summary>
		/// <typeparam name="TDependency">The type of mock to register.</typeparam>
		/// <param name="container">The mocking container.</param>
		/// <param name="name">The contract name of the mock register.</param>
		/// <param name="action">The setter or event to arrange.</param>
		/// <returns>Fluent interface to further configure the behavior of this arrangement.</returns>
		public static ActionExpectation ArrangeSet<TDependency>(this IUnityContainer container, string name, Action<TDependency> action)
		{
			return container.RegisterMock<TDependency>(name).ArrangeSet(action);
		}

		/// <summary>
		/// Register a mocked type and apply a functional specification to it.
		/// </summary>
		/// <typeparam name="TDependency">The type of mock to register.</typeparam>
		/// <param name="container">The mocking container.</param>
		/// <param name="functionalSpecification">The method to arrange.</param>
		public static void ArrangeLike<TDependency>(this IUnityContainer container, Expression<Func<TDependency, bool>> functionalSpecification)
		{
			container.RegisterMock<TDependency>().ArrangeLike(functionalSpecification);
		}

		/// <summary>
		/// Register a mocked type and apply a functional specification to it.
		/// </summary>
		/// <typeparam name="TDependency">The type of mock to register.</typeparam>
		/// <param name="container">The mocking container.</param>
		/// <param name="name">The contract name of the mock register.</param>
		/// <param name="functionalSpecification">The method to arrange.</param>
		public static void ArrangeLike<TDependency>(this IUnityContainer container, string name, Expression<Func<TDependency, bool>> functionalSpecification)
		{
			container.RegisterMock<TDependency>(name).ArrangeLike(functionalSpecification);
		}

		/// <summary>
		/// Assert a method on an registered mock.
		/// </summary>
		/// <typeparam name="TDependency">The type of the registered mock.</typeparam>
		/// <param name="container">The mocking container.</param>
		/// <param name="expression">The method to assert.</param>
		public static void Assert<TDependency>(this IUnityContainer container, Expression<Action<TDependency>> expression)
		{
			container.RegisterMock<TDependency>().Assert(expression);
		}

		/// <summary>
		/// Assert a method on an registered mock.
		/// </summary>
		/// <typeparam name="TDependency">The type of the registered mock.</typeparam>
		/// <param name="container">The mocking container.</param>
		/// <param name="expression">The method to assert.</param>
		public static void Assert<TDependency>(this IUnityContainer container, Expression<Func<TDependency, object>> expression)
		{
			container.RegisterMock<TDependency>().Assert(expression);
		}

		/// <summary>
		/// Assert an registered mock.
		/// </summary>
		/// <typeparam name="TDependency">The type of the registered mock.</typeparam>
		/// <param name="container">The mocking container.</param>
		public static void Assert<TDependency>(this IUnityContainer container)
		{
			container.RegisterMock<TDependency>().Assert();
		}

		/// <summary>
		/// Assert a method on an registered mock.
		/// </summary>
		/// <typeparam name="TDependency">The type of the registered mock.</typeparam>
		/// <param name="container">The mocking container.</param>
		/// <param name="expression">The method to assert.</param>
		/// <param name="occurs">Occurrence expectation.</param>
		public static void Assert<TDependency>(this IUnityContainer container, Expression<Func<TDependency, object>> expression, Occurs occurs)
		{
			container.RegisterMock<TDependency>().Assert(expression, occurs);
		}

		/// <summary>
		/// Assert a method on an registered mock.
		/// </summary>
		/// <typeparam name="TDependency">The type of the registered mock.</typeparam>
		/// <param name="container">The mocking container.</param>
		/// <param name="expression">The method to assert.</param>
		/// <param name="occurs">Occurrence expectation.</param>
		public static void Assert<TDependency>(this IUnityContainer container, Expression<Action<TDependency>> expression, Occurs occurs)
		{
			container.RegisterMock<TDependency>().Assert(expression, occurs);
		}

		/// <summary>
		/// Assert an registered mock.
		/// </summary>
		/// <typeparam name="TDependency">The type of the registered mock.</typeparam>
		/// <param name="container">The mocking container.</param>
		/// <param name="name">The contract name identifying the registered mock.</param>
		public static void Assert<TDependency>(this IUnityContainer container, string name)
		{
			container.RegisterMock<TDependency>(name).Assert();
		}

		/// <summary>
		/// Assert a method on an registered mock.
		/// </summary>
		/// <typeparam name="TDependency">The type of the registered mock.</typeparam>
		/// <param name="container">The mocking container.</param>
		/// <param name="name">The contract name identifying the registered mock.</param>
		/// <param name="expression">The method to assert.</param>
		public static void Assert<TDependency>(this IUnityContainer container, string name, Expression<Func<TDependency, object>> expression)
		{
			container.RegisterMock<TDependency>(name).Assert(expression);
		}

		/// <summary>
		/// Assert a method on an registered mock.
		/// </summary>
		/// <typeparam name="TDependency">The type of the registered mock.</typeparam>
		/// <param name="container">The mocking container.</param>
		/// <param name="name">The contract name identifying the registered mock.</param>
		/// <param name="expression">The method to assert.</param>
		public static void Assert<TDependency>(this IUnityContainer container, string name, Expression<Action<TDependency>> expression)
		{
			container.RegisterMock<TDependency>(name).Assert(expression);
		}

		/// <summary>
		/// Assert a method on an registered mock.
		/// </summary>
		/// <typeparam name="TDependency">The type of the registered mock.</typeparam>
		/// <param name="container">The mocking container.</param>
		/// <param name="name">The contract name identifying the registered mock.</param>
		/// <param name="expression">The method to assert.</param>
		/// <param name="occurs">Occurrence expectation.</param>
		public static void Assert<TDependency>(this IUnityContainer container, string name, Expression<Func<TDependency, object>> expression, Occurs occurs)
		{
			container.RegisterMock<TDependency>(name).Assert(expression, occurs);
		}

		/// <summary>
		/// Assert a method on an registered mock.
		/// </summary>
		/// <typeparam name="TDependency">The type of the registered mock.</typeparam>
		/// <param name="container">The mocking container.</param>
		/// <param name="name">The contract name identifying the registered mock.</param>
		/// <param name="expression">The method to assert.</param>
		/// <param name="occurs">Occurrence expectation.</param>
		public static void Assert<TDependency>(this IUnityContainer container, string name, Expression<Action<TDependency>> expression, Occurs occurs)
		{
			container.RegisterMock<TDependency>(name).Assert(expression, occurs);
		}

		/// <summary>
		/// Asserts all explicit expectations on all registered mocks.
		/// </summary>
		public static void Assert(this IUnityContainer container)
		{
			container.GetMockingExtension().Assert();
		}

		/// <summary>
		/// Asserts all explicit and implicit expectations on all registered mocks.
		/// </summary>
		public static void AssertAll(this IUnityContainer container)
		{
			container.GetMockingExtension().AssertAll();
		}

		private static MockingUnityContainerExtension GetMockingExtension(this IUnityContainer container)
		{
			var extension = container.Configure<MockingUnityContainerExtension>();
			if (extension == null)
			{
				extension = new MockingUnityContainerExtension();
				container.AddExtension(extension);
			}
			return extension;
		}
	}
}
