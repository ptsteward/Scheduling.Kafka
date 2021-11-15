﻿using KafkaTesting.ksqlDB.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Reflection;

namespace KafkaTesting.ksqlDB.Extensions
{
    public static class Registrations
    {
        public static IServiceCollection AddKsqlStreamProvider(this IServiceCollection services, Uri ksqldbServer, Assembly ksqlQueryAssembly, string ksqlNamespace = "KsqlQueries")
        {
            services.AddSingleton<IKsqlQueryReader, KsqlQueryReader>(_ => new KsqlQueryReader(ksqlQueryAssembly, ksqlNamespace))
                .AddSingleton<IKsqlStreamParser, KsqlStreamParser>()
                .AddSingleton<IKsqlRowParser, KsqlRowParser>()
                .AddSingleton<IKsqlStreamProvider, KsqlStreamProvider>();
            services.AddHttpClient<IKsqlClient, KsqlClient>(client =>
            {
                client.BaseAddress = ksqldbServer;
                client.DefaultRequestVersion = HttpVersion.Version20;
                client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
            });
            return services;
        }
    }
}
