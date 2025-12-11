using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class ResponseModel
    {
        public ResponseModel(bool isSuccess = false, string description = "", int code = 2)
        {
            Code = isSuccess ? 1 : code;
            Description = description;
            IsSuccess = isSuccess;
        }

        public int Code { get; set; }
        public string Description { get; set; }
        public bool IsSuccess { get; set; }
    }

    public class ResponseModel<T>
    {
        public ResponseModel(bool isSuccess = false, T model = default, string message = "", int code = 2)
        {
            Code = isSuccess ? 1 : code;
            Message = message;
            IsSuccess = isSuccess;
            Model = model;
        }

        public int Code { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public T Model { get; set; }
    }



    public class ExternalApiResponse
    {
        public string data { get; set; }
        public bool isSuccess { get; set; }
        public string statusCode { get; set; }
        public string message { get; set; }
    }

}
