using System;
using System.Net;
using ApiCalls.Contracts;
using ApiCalls.Contracts.Controllers;
using ApiCalls.Contracts.Hello;
using Microsoft.Owin.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiCalls.Tests
{
    [TestClass]
    public class HelloControllerTests
    {
        private const string Url = "http://localhost:9000";
        private static IDisposable _webApp;
        private static IHelloController _controller;
        
        [ClassInitialize]
        public static void Init(TestContext tc)
        {
            _webApp = WebApp.Start<Startup>(Url);
            _controller = new ControllerProxy(Url).Create<IHelloController>();
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            _webApp.Dispose();
        }

        [TestMethod]
        public void GetFoo()
        {
            var foos = _controller.GetFoo(5);
            Assert.IsNotNull(foos);
        }

        [TestMethod]
        public void GetWithoutArguments()
        {
            var foos = _controller.GetWithoutArguments();
            Assert.IsNotNull(foos);
        }

        [TestMethod]
        public void GetWithMultiple()
        {
            var foos = _controller.GetWithMultiple(5, "test");
            Assert.IsNotNull(foos);
        }

        [TestMethod]
        public void GetWithDateTime()
        {
            var date = DateTime.Now;
            var result = _controller.GetWithDateTime(date);
            Assert.AreEqual(date.ToString("G"), result.ToString("G"));
        }

        [TestMethod]
        public void PostFoo()
        {
            _controller.PostFoo(new Foo {Bar = "test"});
        }

        [TestMethod]
        public void GetThrowsException()
        {
            try
            {
                _controller.GetThrowsException();
            }
            catch (ControllerProxyException e)
            {
                Assert.AreEqual(HttpStatusCode.InternalServerError, e.StatusCode);
                return;
            }

            Assert.Fail("Didn't throw exception");
        }

        [TestMethod]
        public void PostWithReturnType()
        {
            var foo = new Foo {Bar = "bar"};
            var result = _controller.PostWithReturnType(foo);
            Assert.AreEqual(foo.Bar, result.Bar);
        }
    }
}
