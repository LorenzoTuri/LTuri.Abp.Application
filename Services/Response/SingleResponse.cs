namespace LTuri.Abp.Application.Services.Response
{
    public class SingleResponse<T>
    {
        public T Data { get; set; }

        public SingleResponse(T Data)
        {
            this.Data = Data;
        }
    }
}
