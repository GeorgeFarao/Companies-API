using System;
using System.Collections;
using System.Collections.Generic;
using TodoApi2.Data;
using System.ComponentModel.DataAnnotations;

namespace TodoApi2.Models.Lookups
{
    public class BaseLookup
    {
        public PagingInfo PagingInfo { get; set; }
        public string[] Fields { get; set; } //["id", "name"]
    }

    public class PagingInfo
    {
        public int Offset { get; set; }
        public int Size { get; set; }
    }
}