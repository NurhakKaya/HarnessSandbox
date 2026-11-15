using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace HarnessSandbox.ViewComponents;

/// <summary>
/// Renders the welcome hero (heading, intro text and image) for a page that uses
/// the "Hero" composition (alias <c>hero</c>).
/// </summary>
/// <remarks>
/// The site runs Models Builder in InMemoryAuto mode, so the generated <c>IHero</c>
/// interface only exists at runtime and cannot be referenced from compiled C#.
/// The view component therefore passes the content through, and the runtime-compiled
/// view reads it through the strongly typed <c>IHero</c> model.
/// </remarks>
public sealed class HeroViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(IPublishedContent? content)
        => content is null ? Content(string.Empty) : View(content);
}
