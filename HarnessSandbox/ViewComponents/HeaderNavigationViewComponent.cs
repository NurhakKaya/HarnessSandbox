using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services.Navigation;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Extensions;

namespace HarnessSandbox.ViewComponents;

/// <summary>
/// Renders the header navigation from the "Main navigation" property on the root node
/// (the "Site settings" composition). The root is resolved from the current request,
/// so each site in a multi-site install shows its own menu.
/// </summary>
public sealed class HeaderNavigationViewComponent : ViewComponent
{
    private readonly IUmbracoContextAccessor _umbracoContextAccessor;
    private readonly IDocumentNavigationQueryService _navigationQueryService;
    private readonly IPublishedContentStatusFilteringService _statusFilteringService;

    public HeaderNavigationViewComponent(
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

        IReadOnlyList<HeaderNavigationItem> items = BuildItems(root as ISiteSettings, current?.Key);

        return items.Count == 0 ? Content(string.Empty) : View(new HeaderNavigationViewModel(items));
    }

    /// <summary>
    /// Maps the editor-picked links to menu items, skipping links without a URL.
    /// A link is marked current when it points at the page being rendered.
    /// </summary>
    internal static IReadOnlyList<HeaderNavigationItem> BuildItems(ISiteSettings? settings, Guid? currentKey)
    {
        if (settings?.MainNavigation is not { } links)
        {
            return [];
        }

        var items = new List<HeaderNavigationItem>();

        foreach (Link link in links)
        {
            if (string.IsNullOrWhiteSpace(link.Url))
            {
                continue;
            }

            string text = string.IsNullOrWhiteSpace(link.Name) ? link.Url : link.Name.Trim();
            bool isCurrent = currentKey.HasValue && link.Udi is GuidUdi udi && udi.Guid == currentKey.Value;
            bool opensInNewTab = string.Equals(link.Target, "_blank", StringComparison.OrdinalIgnoreCase);

            items.Add(new HeaderNavigationItem(text, link.Url, isCurrent, opensInNewTab));
        }

        return items;
    }
}

/// <summary>View model for the header navigation view component.</summary>
/// <param name="Items">Menu items in editor order; never empty when the view renders.</param>
public sealed record HeaderNavigationViewModel(IReadOnlyList<HeaderNavigationItem> Items);

/// <summary>A single header navigation link.</summary>
/// <param name="Text">Link text shown to the visitor.</param>
/// <param name="Url">Resolved link URL.</param>
/// <param name="IsCurrent">True when the link points at the page being rendered.</param>
/// <param name="OpensInNewTab">True when the editor set the link to open in a new window.</param>
public sealed record HeaderNavigationItem(string Text, string Url, bool IsCurrent, bool OpensInNewTab);
