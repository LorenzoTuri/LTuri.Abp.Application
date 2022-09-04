namespace LTuri.Abp.Application.Services.Response
{
    /// <summary>
    /// TODO: need other metadatas?
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CollectionResponse<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public PaginationResponse Pagination { get; set; } = new PaginationResponse();
        public Dictionary<string, object> Aggregations { get; set; } = new Dictionary<string, object>();
    }
}
