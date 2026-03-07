using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.DTO
{
    public class ApiResponseDto<T>
    {
        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
       

        public ApiResponseDto(int statusCode, bool success, string message, T data)
        {
            StatusCode = statusCode;
            Success = success;
            Message = message;
            Data = data;
        }
    }
}
