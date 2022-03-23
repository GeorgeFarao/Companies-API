using System;
using System.Globalization;
using System.Runtime;

namespace TodoApi2.ExceptionHandle
{
    public class MyException : Exception
    {
        public MyException(): base(){}

        public MyException(string message): base(message){}

        public MyException(string message, params object[] args): 
            base(string.Format(CultureInfo.CurrentCulture, message, args)){}
    }
}
