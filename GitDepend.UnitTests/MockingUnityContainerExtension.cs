// This file was taken from the Telerik.JustMock.Unity project (https://github.com/tailsu/Telerik.JustMock.Unity).
// As of Feb 15, 2017 This project does not support Unity 4.0.1.
// https://github.com/tailsu/Telerik.JustMock.Unity/pull/2 has been submitted to fix this issue.
// 
// This file is licensed under the Apache 2.0 License. A copy of this license can be found at the
// root of this repository in the APACHE_LICENSE file.

using System;
using System.Collections.Generic;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;
using Telerik.JustMock;

namespace GitDepend.UnitTests
{
	/// <summary>
	/// Extension for unity containers that enables implicit and explicit type mocking and asserting.
	/// </summary>
	public sealed class MockingUnityContainerExtension : UnityContainerExtension
	{
		private readonly Dictionary<KeyValuePair<Type, string>, object> _mocks = new Dictionary<KeyValuePair<Type, string>, object>();

		/// <summary>
		/// Registers a mocked type.
		/// </summary>
		/// <typeparam name="T">The mocked type to register</typeparam>
		/// <param name="name">Optional contract name.</param>
		/// <returns>The mock instance.</returns>
		public T RegisterMock<T>(string name = null)
		{
			return (T)RegisterMock(typeof(T), name);
		}

		/// <summary>
		/// Registers a mocked type.
		/// </summary>
		/// <param name="type">The mocked type to register</param>
		/// <param name="name">Optional contract name.</param>
		/// <returns>The mock instance.</returns>
		public object RegisterMock(Type type, string name = null)
		{
			object mock;
			var key = new KeyValuePair<Type, string>(type, name);
			if (!_mocks.TryGetValue(key, out mock))
			{
				mock = Mock.Create(type);
				_mocks.Add(key, mock);
				this.Container.RegisterInstance(type, name, mock);
			}
			return mock;
		}

		/// <summary>
		/// Asserts all explicit expectations on all registered mocks.
		/// </summary>
		public void Assert()
		{
			foreach (var mock in _mocks.Values)
			{
				Mock.Assert(mock);
			}
		}

		/// <summary>
		/// Asserts all explicit and implicit expectations on all registered mocks.
		/// </summary>
		public void AssertAll()
		{
			foreach (var mock in _mocks.Values)
			{
				Mock.AssertAll(mock);
			}
		}

		/// <summary>
		/// Initializes mocking.
		/// </summary>
		protected override void Initialize()
		{
			this.Context.Strategies.Add(new MockingStrategy(this), UnityBuildStage.PreCreation);
		}

		internal sealed class MockingStrategy : BuilderStrategy
		{
			private readonly MockingUnityContainerExtension extension;

			internal MockingStrategy(MockingUnityContainerExtension extension)
			{
				this.extension = extension;
			}

			public override void PreBuildUp(IBuilderContext context)
			{
				var key = context.OriginalBuildKey;
				if (!this.extension.Container.IsRegistered(key.Type, key.Name))
				{
					context.Existing = this.extension.RegisterMock(key.Type, key.Name);
				}
			}
		}
	}
}
