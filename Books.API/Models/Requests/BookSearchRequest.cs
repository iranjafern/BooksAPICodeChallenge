﻿using Books.API.Models.DTOs;

namespace Books.API.Models.Requests
{
    /// <summary>
    /// The book search request
    /// </summary>
    public class BookSearchRequest
    {
        public string Query { get; set; }
        public int StartIndex { get; set; }
        public int PageSize { get; set; }
        public UserDto UserInfo { get; set; }
    }
}
