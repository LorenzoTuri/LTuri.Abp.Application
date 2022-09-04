namespace LTuri.Abp.Application.Services.Response
{
    /// <summary>
    /// TODO: do we need UnfilteredTotal?
    /// </summary>
    public class PaginationResponse
    {
        public long Total { get; set; } = 0;

        public long UnfilteredTotal { get; set; } = 0;
        public long Page { get; set; } = 0;
        public long Size { get; set; } = 100;
    }
}
