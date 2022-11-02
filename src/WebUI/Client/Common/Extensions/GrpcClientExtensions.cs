using BlazorCleanArchitecture.WebUI.Client.Common.Interceptors;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using ProtoBuf.Grpc.Client;

namespace BlazorCleanArchitecture.WebUI.Client.Common.Extensions
{
    public static class GrpcClientExtensions
    {
        public static IServiceCollection AddGrpcClient<T>(this IServiceCollection services)
            where T : class
            => services.AddTransient(provider =>
            {
                var interceptor = provider.GetService<GrpcClientInterceptor>();
                var httpHandler = provider.GetService<HttpClientHandler>();
                var httpClient = provider.GetService<HttpClient>();

                var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, httpHandler ?? new HttpClientHandler());

                var channel = GrpcChannel.ForAddress(httpClient.BaseAddress, new GrpcChannelOptions { HttpHandler = handler });

                var invoker = channel.Intercept(interceptor);

                return GrpcClientFactory.CreateGrpcService<T>(invoker);
            });

        public static IServiceCollection AddGrpcClient<TClass>(this IServiceCollection services, TClass type)
            where TClass : class
            => services.AddGrpcClient<TClass>();
    }
}
