namespace RH.Shared.HttpClient
{
    public interface IHttpClientFactory
    {
        System.Net.Http.HttpClient GetHttpClient(string baseAddress);
    }
}
