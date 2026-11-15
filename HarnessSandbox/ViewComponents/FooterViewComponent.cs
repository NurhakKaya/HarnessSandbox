using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services.Navigation;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace HarnessSandbox.ViewComponents;

/// <summary>
/// Renders the site footer. The site name comes from the "Site name" property
/// (alias <c>siteName</c>) on the root node via the "Site settings" composition,
/// falling back to the root node's own name when the property is empty or missing.
/// </summary>
public sealed class FooterViewComponent : ViewComponent
{
    /// <summary>Property alias defined on the "Site settings" composition.</summary>
    public const string SiteNamePropertyAlias = "siteName";

    private readonly IUmbracoContextAccessor _umbracoContextAccessor;
    private readonly IDocumentNavigationQueryService _navigationQueryService;
    private readonly IPublishedContentStatusFilteringService _statusFilteringService;

    public FooterViewComponent(
        IUmbracoContextAccessor umbracoContextAccessor,
        IDocumentNavigationQueryService navigationQueryService,
        IPublishedContentStatusFilteringService statusFilteringService)
    {
        _umbracoContextAccessor = umbracoContextAccessor;
        _navigationQueryService = navigationQueryService;
        _statusFilteringService = statusFilteringService;
    }

    public IViewComponentResult Invoke()
    {
        IPublishedContent? current = _umbracoContextAccessor.TryGetUmbracoContext(out IUmbracoContext? context)
            ? context.PublishedRequest?.PublishedContent
            : null;

        IPublishedContent? root = current?.Root(_navigationQueryService, _statusFilteringService);

        return View(new FooterViewModel(ResolveSiteName(root)));
    }

    /// <summary>
    /// Returns the "Site name" property value if set, otherwise the root node's name.
    /// Returns an empty string when there is no root (e.g. rendered outside an Umbraco request).
    /// </summary>
    internal static string ResolveSiteName(IPublishedContent? root)
    {
        if (root is null)
        {
            return string.Empty;
        }

        string? siteName = root.HasProperty(SiteNamePropertyAlias)
            ? root.Value<string>(SiteNamePropertyAlias)
            : null;

        return string.IsNullOrWhiteSpace(siteName) ? root.Name : siteName.Trim();
    }
}

/// <summary>View model for the footer view component.</summary>
/// <param name="SiteName">The resolved site name; empty when none could be resolved.</param>
public sealed record FooterViewModel(string SiteName);
