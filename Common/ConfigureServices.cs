using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Common
{
    public delegate ConfigureServicesResult ConfigureServices(
        HostBuilderContext hostContext,
        IServiceCollection services);
}