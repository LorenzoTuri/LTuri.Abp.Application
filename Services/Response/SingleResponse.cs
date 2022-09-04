namespace LTuri.Abp.Application.Services.Response
{
    /// <summary>
    /// TODO: should return other metadata?
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingleResponse<T>
    {
        public T Data { get; set; }

        public SingleResponse(T Data)
        {
            this.Data = Data;
        }
    }
}
