using dn32.infra.Nucleo.Doc.Controllers;
using dn32.infra.Nucleo.Doc.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace dn32.infra
{
    public static class DnDocExtension
    {
        internal static string ApiBaseUrl { get; set; }

        public static string G(this string key)
        {
            return key == null ? null : (DnGlobalization?.Get(key) ?? key);
        }

        internal static IDnGlobalization DnGlobalization { get; set; }

        public static IMvcBuilder AdicionarDnDoc(this IMvcBuilder builder, string apiBaseUrl)
        {
            ApiBaseUrl = apiBaseUrl;
            builder.Services.AdicionarDnDoc();
            return builder;
        }

        public static IMvcBuilder AddDnGlobalizationDoc<T>(this IMvcBuilder builder) where T : IDnGlobalization, new()
        {
            DnGlobalization = new T();
            return builder;
        }

        public static IServiceCollection AdicionarDnDoc(this IServiceCollection services)
        {
            services.Configure<StaticFileOptions>(opts =>
            {
                opts.FileProvider = new DocEmbeddedStaticFileProvider();
            });

            return services;
        }

        public static IApplicationBuilder UsarDnDoc(this IApplicationBuilder app)
        {
            app.UseStaticFiles();
            return app;
        }
    }
}
