﻿using System;

namespace Memorandum.Web.Framework.Errors
{
    internal class HttpErrorException : Exception
    {
        public HttpErrorException(int statusCode)
        {
            StatusCode = statusCode;
        }

        public HttpErrorException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpErrorException(int statusCode, string message, Exception inner)
            : base(message, inner)
        {
            StatusCode = statusCode;
        }

        public int StatusCode { get; private set; }
    }

    internal class Http404Exception : HttpErrorException
    {
        public Http404Exception(string message = "") : base(404, message)
        {
        }
    }

    internal class Http500Exception : HttpErrorException
    {
        public Http500Exception(string message = "")
            : base(500, message)
        {
        }

        public Http500Exception(Exception inner, string message = "")
            : base(500, message, inner)
        {
        }
    }
}