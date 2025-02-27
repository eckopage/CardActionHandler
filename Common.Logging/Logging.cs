using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

namespace Common.Logging
{
    public static class Logging
    {
        public static Action<HostBuilderContext, LoggerConfiguration> ConfigureLogger =>
            (context, loggerConfiguration) =>
            {
                var env = context.HostingEnvironment;

                ConfigureBaseLogger(env, loggerConfiguration);

                if (env.IsDevelopment())
                {
                    ConfigureDevelopmentLogger(loggerConfiguration);
                }

                ConfigureElasticSearch(context, loggerConfiguration);
            };

        private static void ConfigureBaseLogger(IHostEnvironment env, LoggerConfiguration loggerConfiguration)
        {
            loggerConfiguration
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ApplicationName", env.ApplicationName)
                .Enrich.WithProperty("EnvironmentName", env.EnvironmentName)
                .Enrich.WithExceptionDetails()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
                .WriteTo.Console();
        }

        private static void ConfigureDevelopmentLogger(LoggerConfiguration loggerConfiguration)
        {
            loggerConfiguration.MinimumLevel.Override("Card", LogEventLevel.Debug);
        }

        private static void ConfigureElasticSearch(HostBuilderContext context, LoggerConfiguration loggerConfiguration)
        {
            var elasticUrl = context.Configuration.GetValue<string>("ElasticConfiguration:Uri");
            if (string.IsNullOrEmpty(elasticUrl) == false)
            {
                loggerConfiguration.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUrl)) {
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,
                    IndexFormat = "ecommerce-Logs-{0:yyyy.MM.dd}",
                    MinimumLogEventLevel = LogEventLevel.Debug 
                });
            }
        }
    }
}