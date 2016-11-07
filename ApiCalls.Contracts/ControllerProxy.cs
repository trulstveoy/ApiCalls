using Castle.DynamicProxy;

namespace ApiCalls.Contracts
{
    public class ControllerProxy
    {
        private readonly string _baseUrl;

        public ControllerProxy(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public T Create<T>() where T : class
        {
            var generator = new ProxyGenerator();
            var proxy = generator.CreateInterfaceProxyWithoutTarget<T>(new CallInterceptor(_baseUrl));
            return proxy;
        }
    }
}
