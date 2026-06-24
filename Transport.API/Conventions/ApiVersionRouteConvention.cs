using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Transport.API.Conventions
{
    public class ApiVersionRouteConvention : IApplicationModelConvention
    {
        private const string VersionPrefix = "api/v1";

        public void Apply(ApplicationModel application)
        {
            foreach (var selector in application.Controllers
                .SelectMany(controller => controller.Selectors)
                .Where(selector => selector.AttributeRouteModel is not null))
            {
                var template = selector.AttributeRouteModel!.Template;
                var versionedTemplate = ToVersionedTemplate(template);

                if (versionedTemplate is not null)
                    selector.AttributeRouteModel.Template = versionedTemplate;
            }
        }

        private static string? ToVersionedTemplate(string? template)
        {
            if (string.IsNullOrWhiteSpace(template))
                return null;

            var normalized = template.TrimStart('/');
            if (!normalized.StartsWith("api/", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(normalized, "api", StringComparison.OrdinalIgnoreCase))
                return null;

            if (normalized.StartsWith("api/v1/", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, "api/v1", StringComparison.OrdinalIgnoreCase))
                return null;

            return string.Equals(normalized, "api", StringComparison.OrdinalIgnoreCase)
                ? VersionPrefix
                : VersionPrefix + normalized[3..];
        }
    }
}
