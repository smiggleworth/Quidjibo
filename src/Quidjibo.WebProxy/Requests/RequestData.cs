namespace Quidjibo.WebProxy.Requests
{
    public class RequestData
    {
        public string[] Queues { get; set; }
    }

    public class RequestData<T> : RequestData
    {
        public T Data { get; set; }
    }
}