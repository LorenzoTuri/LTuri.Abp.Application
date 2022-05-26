using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Volo.Abp.Application.Dtos;

namespace LTuri.Abp.Application.Services.Response
{
    public class CollectionResponse<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public PaginationResponse Pagination { get; set; } = new PaginationResponse();
        public Dictionary<string, object> Aggregations { get; set; } = new Dictionary<string, object>();
    }
}
