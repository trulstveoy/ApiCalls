using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Castle.DynamicProxy;
using Newtonsoft.Json;

namespace ApiCalls.Contracts
{
    public class CallInterceptor : IInterceptor
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string _baseUrl;

        public CallInterceptor(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public void Intercept(IInvocation invocation)
        {
            string methodName = invocation.Method.Name;
            string controllerInterfaceName = invocation.Method.DeclaringType.Name;
            var controller = controllerInterfaceName.TrimStart('I').Replace("Controller", "");
            var argumentNames = invocation.Method.GetParameters().Select(p => p.Name).ToArray();
            var arguments = invocation.Arguments;
            var returnType = invocation.Method.ReturnType;

            if (methodName.StartsWith("Get"))
            {
                object resultInstance = Get(methodName, controller, argumentNames, arguments, returnType);
                invocation.ReturnValue = resultInstance;
            }
            else if(methodName.StartsWith("Post"))
            {
                string url = $"{_baseUrl}/api/{controller}/{methodName}";

                var json = JsonConvert.SerializeObject(arguments.First());
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var resultAsString = _httpClient.PostAsync(url, content).Result.Content.ReadAsStringAsync().Result;
                if (returnType != typeof(void))
                {
                    var resultInstance = JsonConvert.DeserializeObject(resultAsString, returnType);
                    invocation.ReturnValue = resultInstance;
                }
            }
            else
            {
                throw new NotSupportedException($"Unsupported methodName {methodName}");
            }
        }

        private object Get(string methodName, string controller, string[] argumentNames, object[] arguments, Type returnType)
        {
            string queryString = string.Join("&", Enumerable.Range(0, argumentNames.Length)
                .Select(i => new Tuple<string, string>(argumentNames[i], ParseArgument(arguments[i])))
                .Select(t => $"{t.Item1}={t.Item2.ToString()}"));

            string url = $"{_baseUrl}/api/{controller}/{methodName}";
            if (!string.IsNullOrWhiteSpace(queryString))
                url += "?" + queryString;

            var reply = _httpClient.GetAsync(url).Result;
            string resultAsString = reply.Content.ReadAsStringAsync().Result;
            if (!reply.IsSuccessStatusCode)
            {
                throw new ControllerProxyException(resultAsString, reply.StatusCode);
            }

            var resultInstance = JsonConvert.DeserializeObject(resultAsString, returnType);
            return resultInstance;
        }

        private string ParseArgument(object o)
        {
            if (o is DateTime)
            {   
                var dateTime = ((DateTime) o).ToString("MM/dd/yy HH:mm:ss");
                return dateTime;
            }
            return o.ToString();
        }
    }
}



