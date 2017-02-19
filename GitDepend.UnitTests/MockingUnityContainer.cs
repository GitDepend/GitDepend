// This file was taken from the Telerik.JustMock.Unity project (https://github.com/tailsu/Telerik.JustMock.Unity).
// As of Feb 15, 2017 This project does not support Unity 4.0.1.
// https://github.com/tailsu/Telerik.JustMock.Unity/pull/2 has been submitted to fix this issue.
// 
// This file is licensed under the Apache 2.0 License. A copy of this license can be found at the
// root of this repository in the APACHE_LICENSE file.

using Microsoft.Practices.Unity;

namespace GitDepend.UnitTests
{
    /// <summary>
    /// Mocking container that works similarly to the built-in MockingContainer class.
    /// </summary>
    /// <typeparam name="T">The type whose dependencies will be mocked.</typeparam>
    public class MockingUnityContainer<T> : UnityContainer where T : class
    {
        private T _instance;

        /// <summary>
        /// Constructs a new mocking container.
        /// </summary>
        public MockingUnityContainer()
        {
            this.RegisterType<T>();
        }

        /// <summary>
        /// The instance with satisfied dependencies. The instance is created on first access.
        /// </summary>
        public T Instance => _instance ?? (_instance = this.Resolve<T>());
    }
}