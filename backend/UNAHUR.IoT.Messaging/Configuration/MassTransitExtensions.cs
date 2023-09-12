using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace UNAHUR.IoT.Messaging.Configuration
{
    public static class MassTransitExtensions
    {
        // source : https://github.com/MassTransit/Sample-Dispatcher/blob/master/src/Sample.Shared/SampleConfigurationExtensions.cs

        /// <summary>
        /// Extends <see cref="IBusRegistrationConfigurator"/> to add common configuraiton settings
        /// </summary>
        /// <param name="configurator"><see cref="IBusRegistrationConfigurator"/></param>
        /// <param name="configure">Action to add extra configuration</param>
        public static void ConfigureMassTransit(this IBusRegistrationConfigurator configurator, Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>? configure = null)
        {

            configurator.SetKebabCaseEndpointNameFormatter();

            configurator.UsingRabbitMq((context, cfg) =>
            {
                //TODO: aca va el nombre del servivicio nuevamente
                //cfg.UsePrometheusMetrics(serviceName: "order_service");
                //cfg.UseInstrumentation();
                var settings = context.GetRequiredService<IOptions<RabbitMqSettings>>().Value;

                cfg.Host(settings.Host, settings.VirtualHost, h =>
                {
                    h.Username(settings.Username);
                    h.Password(settings.Password);
                });
                configure?.Invoke(context, cfg);


            });
        }



    }
}
