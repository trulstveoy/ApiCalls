using System;
using System.Collections.Generic;
using ApiCalls.Contracts.Hello;

namespace ApiCalls.Contracts.Controllers
{
    public interface IHelloController
    {
        List<Foo> GetFoo(int number);

        List<Foo> GetWithoutArguments();

        List<Foo> GetWithMultiple(int number, string bar);

        DateTime GetWithDateTime(DateTime dateTime);
        
        void PostFoo(Foo foo);

        Foo PostWithReturnType(Foo foo);
       
        List<Foo> GetThrowsException();
    }
}