using System.IO.Compression;
using System.Text;
using AngleSharp.Dom;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Toolkit.Buttons;
using Xunit;
using Xunit.Abstractions;

namespace Syncfusion.Blazor.Toolkit.Tests.Base
{
    /// <summary>
    /// Behavioural tests for the render-time Tier-1 theme emitter (<see cref="SfThemeRoot"/>):
    /// delivery in the render output, renderer-scoped ownership, owner-dispose recovery,
    /// duplicate prevention, the JavaScript fallback and real static-SSR output.
    /// </summary>
    public class SfThemeRootTests : BunitTestContext
    {
        private readonly ITestOutputHelper _output;

        public SfThemeRootTests(ITestOutputHelper output)
        {
            _output = output;
        }

        // These tests exercise the REAL emitter.
        protected override bool StubThemeRoot => false;

        #region Delivery

        [Fact(Timeout = 10000, DisplayName = "Theme: a single SfThemeRoot renders the style element with the full raw payload")]
        public void SingleRoot_EmitsStyleWithRawUnencodedPayload()
        {
            IRenderedComponent<ThemeHost> cut = RenderComponent<ThemeHost>(p => p.Add(x => x.ShowB, false));

            IElement style = Assert.Single(cut.FindAll("style#sf-theme-root"));
            Assert.Equal("true", style.GetAttribute("data-sf-theme-root"));

            // AngleSharp normalises CRLF to LF when parsing raw-text elements.
            Assert.Equal(Normalize(SfThemeRoot.Payload), Normalize(style.TextContent));

            // Raw (not HTML-encoded) CSS: child combinators must survive verbatim.
            Assert.Contains(">", style.TextContent, StringComparison.Ordinal);
            Assert.DoesNotContain("&gt;", cut.Markup, StringComparison.Ordinal);
            Assert.DoesNotContain("&amp;", style.OuterHtml, StringComparison.Ordinal);
        }

        [Fact(Timeout = 10000, DisplayName = "Theme: payload is safe to embed and contains every required feature block")]
        public void Payload_IsSafeAndComplete()
        {
            string payload = SfThemeRoot.Payload;

            // A literal </style would terminate the element early and leak CSS into the page as text.
            Assert.DoesNotContain("</style", payload, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(":root", payload, StringComparison.Ordinal);                 // variables / tokens
            Assert.Contains("--color-sf-primary:", payload, StringComparison.Ordinal);   // theme tokens
            Assert.Contains("@font-face", payload, StringComparison.Ordinal);            // icon font
            Assert.Contains("base64,", payload, StringComparison.Ordinal);               // embedded font
            Assert.Contains("e-toolkit-icons", payload, StringComparison.Ordinal);       // icon definitions
            Assert.Contains("@keyframes", payload, StringComparison.Ordinal);            // animations
            Assert.Contains("prefers-color-scheme", payload, StringComparison.Ordinal);  // dark scheme
            Assert.Contains("forced-colors", payload, StringComparison.Ordinal);         // forced colors / HC
        }

        [Fact(Timeout = 10000, DisplayName = "Theme: payload size budget (raw + gzip) is measured and bounded")]
        public void Payload_SizeBudget()
        {
            byte[] raw = Encoding.UTF8.GetBytes(SfThemeRoot.Payload);
            using MemoryStream compressed = new();
            using (GZipStream gzip = new(compressed, CompressionLevel.Optimal, leaveOpen: true))
            {
                gzip.Write(raw, 0, raw.Length);
            }

            _output.WriteLine($"sf-theme-root payload: raw = {raw.Length:N0} bytes ({raw.Length / 1024.0:F1} KiB), gzip = {compressed.Length:N0} bytes ({compressed.Length / 1024.0:F1} KiB)");

            Assert.True(raw.Length < 160 * 1024, $"Raw payload grew to {raw.Length} bytes.");
            Assert.True(compressed.Length < 80 * 1024, $"Gzip payload grew to {compressed.Length} bytes.");
        }

        #endregion

        #region Ownership inside one renderer

        [Fact(Timeout = 10000, DisplayName = "Ownership: a second root in the same renderer renders nothing (no duplication)")]
        public void SecondRoot_InSameRenderer_RendersNothing()
        {
            IRenderedComponent<ThemeHost> cut = RenderComponent<ThemeHost>();

            IElement style = Assert.Single(cut.FindAll("style#sf-theme-root"));
            Assert.Equal("a", style.ParentElement!.Id);
            Assert.Empty(cut.Find("#b").Children);
        }

        [Fact(Timeout = 10000, DisplayName = "Recovery: disposing the owner transfers the style element to the surviving root")]
        public void OwnerDisposed_TransfersToSurvivor()
        {
            IRenderedComponent<ThemeHost> cut = RenderComponent<ThemeHost>();
            Assert.Equal("a", Assert.Single(cut.FindAll("style#sf-theme-root")).ParentElement!.Id);

            cut.SetParametersAndRender(p => p.Add(x => x.ShowA, false));

            IElement style = Assert.Single(cut.FindAll("style#sf-theme-root"));
            Assert.Equal("b", style.ParentElement!.Id);
        }

        [Fact(Timeout = 10000, DisplayName = "Recovery: replacing the owner in a single render (navigation) never loses or duplicates the theme")]
        public void OwnerReplacedInSameBatch_ThemeSurvives()
        {
            IRenderedComponent<ThemeHost> cut = RenderComponent<ThemeHost>(p => p.Add(x => x.ShowB, false));
            Assert.Equal("a", Assert.Single(cut.FindAll("style#sf-theme-root")).ParentElement!.Id);

            cut.SetParametersAndRender(p => p.Add(x => x.ShowA, false).Add(x => x.ShowB, true));

            Assert.Equal("b", Assert.Single(cut.FindAll("style#sf-theme-root")).ParentElement!.Id);
        }

        [Fact(Timeout = 10000, DisplayName = "Recovery: ownership is released when the last root goes and re-acquired by the next one (A -> none -> B -> A)")]
        public void ReleaseAndReacquire()
        {
            IRenderedComponent<ThemeHost> cut = RenderComponent<ThemeHost>(p => p.Add(x => x.ShowB, false));
            Assert.Single(cut.FindAll("style#sf-theme-root"));

            cut.SetParametersAndRender(p => p.Add(x => x.ShowA, false));
            Assert.Empty(cut.FindAll("style#sf-theme-root"));

            cut.SetParametersAndRender(p => p.Add(x => x.ShowB, true));
            Assert.Equal("b", Assert.Single(cut.FindAll("style#sf-theme-root")).ParentElement!.Id);

            cut.SetParametersAndRender(p => p.Add(x => x.ShowA, true));
            Assert.Single(cut.FindAll("style#sf-theme-root"));
            Assert.Equal("b", cut.Find("style#sf-theme-root").ParentElement!.Id);
        }

        [Fact(Timeout = 10000, DisplayName = "Recovery: removing a non-owner never disturbs the owner")]
        public void NonOwnerDisposed_OwnerUnchanged()
        {
            IRenderedComponent<ThemeHost> cut = RenderComponent<ThemeHost>();

            cut.SetParametersAndRender(p => p.Add(x => x.ShowB, false));

            Assert.Equal("a", Assert.Single(cut.FindAll("style#sf-theme-root")).ParentElement!.Id);
        }

        [Fact(Timeout = 10000, DisplayName = "Recovery: a re-render of the parent does not duplicate or remove the style element")]
        public void ParentRerender_IsStable()
        {
            IRenderedComponent<ThemeHost> cut = RenderComponent<ThemeHost>();
            string markupBefore = cut.Markup;

            for (int i = 0; i < 5; i++)
            {
                cut.SetParametersAndRender(p => p.Add(x => x.Counter, cut.Instance.Counter + 1));
            }

            Assert.Single(cut.FindAll("style#sf-theme-root"));
            Assert.Equal(markupBefore, cut.Markup);
            // No render-tree diff for the emitter: the 93 KB markup frame is not re-sent on parent renders.
            Assert.Empty(cut.GetChangesSinceFirstRender());
        }

        #endregion

        #region Renderer isolation

        [Fact(Timeout = 10000, DisplayName = "Isolation: independent renderers (SSR requests / circuits / tabs) each get their own emitter")]
        public void IndependentRenderers_AreIsolated()
        {
            using RealThemeContext first = new();
            using RealThemeContext second = new();

            IRenderedComponent<ThemeHost> a = first.RenderComponent<ThemeHost>(p => p.Add(x => x.ShowB, false));
            IRenderedComponent<ThemeHost> b = second.RenderComponent<ThemeHost>(p => p.Add(x => x.ShowB, false));

            Assert.Single(a.FindAll("style#sf-theme-root"));
            Assert.Single(b.FindAll("style#sf-theme-root"));

            // Tearing one renderer down must not affect the other.
            first.DisposeComponents();
            Assert.Single(b.FindAll("style#sf-theme-root"));
        }

        [Fact(Timeout = 10000, DisplayName = "Isolation: scope identity = (service provider, dispatcher); same pair shares, different pair does not")]
        public void ScopeIdentity_IsServiceProviderAndDispatcher()
        {
            using ServiceProvider provider = new ServiceCollection().BuildServiceProvider();
            Dispatcher d1 = Dispatcher.CreateDefault();
            Dispatcher d2 = Dispatcher.CreateDefault();

            SfThemeScope a1 = SfThemeScope.For(provider, d1);
            SfThemeScope a2 = SfThemeScope.For(provider, d1);
            SfThemeScope b = SfThemeScope.For(provider, d2);

            using ServiceProvider other = new ServiceCollection().BuildServiceProvider();
            SfThemeScope c = SfThemeScope.For(other, d1);

            Assert.Same(a1, a2);
            Assert.NotSame(a1, b);   // standalone HtmlRenderer instances on one shared root provider
            Assert.NotSame(a1, c);   // SSR requests / circuits (different DI scope, even if dispatcher is shared)
        }

        [Fact(Timeout = 20000, DisplayName = "Isolation: scopes do not keep renderers (service provider / dispatcher) alive")]
        public void Scopes_DoNotLeak()
        {
            WeakReference scope = CreateScopeAndReturnWeakReference();

            for (int i = 0; i < 5 && scope.IsAlive; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            Assert.False(scope.IsAlive, "SfThemeScope was kept alive after its service provider and dispatcher became unreachable.");
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        private static WeakReference CreateScopeAndReturnWeakReference()
        {
            ServiceProvider provider = new ServiceCollection().BuildServiceProvider();
            Dispatcher dispatcher = Dispatcher.CreateDefault();
            WeakReference scope = new(SfThemeScope.For(provider, dispatcher));
            provider.Dispose();
            return scope;
        }

        #endregion

        #region Real components

        [Fact(Timeout = 10000, DisplayName = "Components: the style is the previous sibling of the component root, never inside it")]
        public void SfButton_EmitsStyleBeforeRoot_NotInside()
        {
            IRenderedComponent<SfButton> cut = RenderComponent<SfButton>(p => p.Add(x => x.Content, "OK"));

            Assert.Equal("style", cut.Nodes[0].NodeName.ToLowerInvariant());
            IElement button = cut.Find("button");
            Assert.Equal("style", button.PreviousElementSibling!.NodeName.ToLowerInvariant());
            Assert.Empty(button.QuerySelectorAll("style"));
        }

        [Fact(Timeout = 10000, DisplayName = "Components: many toolkit components in one renderer emit exactly one style element")]
        public void ManyComponents_OneStyle()
        {
            IRenderedComponent<MixedHost> cut = RenderComponent<MixedHost>();

            Assert.Single(cut.FindAll("style#sf-theme-root"));
            Assert.NotEmpty(cut.FindAll("button.e-btn"));
        }

        [Fact(Timeout = 10000, DisplayName = "Components: the button group's own children (:first-child / input + label) are unaffected by the emitter")]
        public void ButtonGroup_EmitterIsOutsideGroup()
        {
            IRenderedComponent<GroupHost> cut = RenderComponent<GroupHost>();

            IElement group = cut.Find("div.e-btn-group");
            Assert.Empty(group.QuerySelectorAll("style"));
            Assert.Equal("style", group.PreviousElementSibling!.NodeName.ToLowerInvariant());
            Assert.Equal("input", group.FirstElementChild!.NodeName.ToLowerInvariant());
            Assert.Single(cut.FindAll("style#sf-theme-root"));
        }

        #endregion

        #region JavaScript fallback

        [Fact(Timeout = 10000, DisplayName = "Fallback: JavaScript is NOT used when a SfThemeRoot owner exists")]
        public void Fallback_NotUsed_WhenOwnerExists()
        {
            RenderComponent<SfButton>(p => p.Add(x => x.Content, "OK"));

            Assert.DoesNotContain(JSInterop.Invocations, i => i.Identifier == "sfBlazorToolkit.themeRoot.ensure");
        }

        [Fact(Timeout = 10000, DisplayName = "Fallback: a component that omits SfThemeRoot triggers exactly one idempotent JS injection per runtime")]
        public void Fallback_Used_ForComponentWithoutEmitter_OncePerRuntime()
        {
            RenderComponent<BareToolkitComponent>();
            RenderComponent<BareToolkitComponent>();

            Assert.Single(JSInterop.Invocations.Where(i => i.Identifier == "sfBlazorToolkit.themeRoot.ensure"));
        }

        [Fact(Timeout = 10000, DisplayName = "Fallback: it is skipped once a real emitter appears in the renderer")]
        public void Fallback_Skipped_WhenAnotherComponentOwns()
        {
            RenderComponent<SfButton>(p => p.Add(x => x.Content, "OK"));
            RenderComponent<BareToolkitComponent>();

            Assert.DoesNotContain(JSInterop.Invocations, i => i.Identifier == "sfBlazorToolkit.themeRoot.ensure");
        }

        #endregion

        #region Static SSR (real HtmlRenderer output, no JavaScript)

        [Fact(Timeout = 20000, DisplayName = "Static SSR: theme is part of the response HTML with no JavaScript runtime available")]
        public async Task StaticSsr_ResponseHtml_ContainsTheme_WithoutJavaScript()
        {
            using ServiceProvider root = BuildSsrProvider();

            string html = await RenderRequestAsync(root, 
                typeof(SfButton), new Dictionary<string, object?> { ["Content"] = "SSR" });

            Assert.Equal(1, Count(html, "id=\"sf-theme-root\""));
            Assert.True(html.IndexOf("id=\"sf-theme-root\"", StringComparison.Ordinal) < html.IndexOf("<button", StringComparison.Ordinal),
                "The theme must precede the component in the HTML so the first paint is styled.");
            Assert.Contains("--color-sf-primary:", html, StringComparison.Ordinal);
            Assert.Contains("@font-face", html, StringComparison.Ordinal);
            Assert.Contains(">", html[html.IndexOf("<style", StringComparison.Ordinal)..html.IndexOf("</style>", StringComparison.Ordinal)], StringComparison.Ordinal);
            Assert.DoesNotContain("&gt;", html[html.IndexOf("<style", StringComparison.Ordinal)..html.IndexOf("</style>", StringComparison.Ordinal)], StringComparison.Ordinal);
        }

        [Fact(Timeout = 30000, DisplayName = "Static SSR: independent requests (new scope each) each contain exactly one theme")]
        public async Task StaticSsr_MultipleRequests_AreIsolated()
        {
            using ServiceProvider root = BuildSsrProvider();

            string[] pages = await Task.WhenAll(Enumerable.Range(0, 6).Select(i =>
                RenderRequestAsync(root, typeof(SfButton), new Dictionary<string, object?> { ["Content"] = "R" + i })));

            Assert.All(pages, page => Assert.Equal(1, Count(page, "id=\"sf-theme-root\"")));
        }

        [Fact(Timeout = 30000, DisplayName = "Static SSR: concurrent renderers sharing ONE root provider do not steal ownership from each other")]
        public async Task StaticSsr_SharedRootProvider_ConcurrentRenderers()
        {
            using ServiceProvider root = BuildSsrProvider();
            ILoggerFactory loggerFactory = root.GetRequiredService<ILoggerFactory>();

            async Task<string> RenderAsync(string text)
            {
                await using HtmlRenderer renderer = new(root, loggerFactory);
                return await renderer.Dispatcher.InvokeAsync(async () =>
                {
                    var output = await renderer.RenderComponentAsync<SfButton>(
                        ParameterView.FromDictionary(new Dictionary<string, object?> { ["Content"] = text }));
                    return output.ToHtmlString();
                });
            }

            string[] pages = await Task.WhenAll(RenderAsync("A"), RenderAsync("B"), RenderAsync("C"));

            Assert.All(pages, page => Assert.Equal(1, Count(page, "id=\"sf-theme-root\"")));
        }

        private static ServiceProvider BuildSsrProvider()
        {
            ServiceCollection services = new();
            services.AddLogging();
            services.AddLocalization();
            services.AddOptions();
            services.AddSyncfusionBlazorToolkit();
            // Static SSR: any JS call throws, exactly like the prerender runtime.
            services.AddScoped<IJSRuntime, NoJavaScriptRuntime>();
            return services.BuildServiceProvider();
        }

        private static async Task<string> RenderRequestAsync(ServiceProvider root, Type componentType, Dictionary<string, object?> parameters)
        {
            using IServiceScope scope = root.CreateScope();
            ILoggerFactory loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            await using HtmlRenderer renderer = new(scope.ServiceProvider, loggerFactory);

            return await renderer.Dispatcher.InvokeAsync(async () =>
            {
                var output = await renderer.RenderComponentAsync(componentType, ParameterView.FromDictionary(parameters));
                return output.ToHtmlString();
            });
        }

        private static int Count(string haystack, string needle)
        {
            int count = 0;
            for (int i = haystack.IndexOf(needle, StringComparison.Ordinal); i >= 0; i = haystack.IndexOf(needle, i + needle.Length, StringComparison.Ordinal))
            {
                count++;
            }

            return count;
        }

        private static string Normalize(string value)
        {
            return value.Replace("\r\n", "\n", StringComparison.Ordinal).Trim();
        }

        #endregion

        #region Test components

        public sealed class RealThemeContext : BunitTestContext

        {

            protected override bool StubThemeRoot => false;

        }


        public sealed class ThemeHost : ComponentBase
        {
            [Parameter]
            public bool ShowA { get; set; } = true;

            [Parameter]
            public bool ShowB { get; set; } = true;

            [Parameter]
            public int Counter { get; set; }

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                if (ShowA)
                {
                    builder.OpenElement(0, "section");
                    builder.AddAttribute(1, "id", "a");
                    builder.OpenComponent<SfThemeRoot>(2);
                    builder.CloseComponent();
                    builder.CloseElement();
                }

                if (ShowB)
                {
                    builder.OpenElement(10, "section");
                    builder.AddAttribute(11, "id", "b");
                    builder.OpenComponent<SfThemeRoot>(12);
                    builder.CloseComponent();
                    builder.CloseElement();
                }
            }
        }

        public sealed class MixedHost : ComponentBase
        {
            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                for (int i = 0; i < 4; i++)
                {
                    builder.OpenComponent<SfButton>(i);
                    builder.AddAttribute(100 + i, nameof(SfButton.Content), "B" + i);
                    builder.CloseComponent();
                }
            }
        }

        public sealed class GroupHost : ComponentBase
        {
            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenComponent<SfButtonGroup>(0);
                builder.AddAttribute(1, nameof(SfButtonGroup.Mode), SelectionMode.Single);
                builder.AddAttribute(2, nameof(SfButtonGroup.ChildContent), (RenderFragment)(inner =>
                {
                    for (int i = 0; i < 3; i++)
                    {
                        inner.OpenComponent<Button>(10 + i);
                        inner.AddAttribute(20 + i, nameof(Button.ChildContent), (RenderFragment)(c => c.AddContent(0, "Item")));
                        inner.CloseComponent();
                    }
                }));
                builder.CloseComponent();
            }
        }

        /// <summary>A derived toolkit component that (incorrectly) does not render the emitter.</summary>
        public sealed class BareToolkitComponent : SfBaseComponent
        {
            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenElement(0, "div");
                builder.CloseElement();
            }
        }

        private sealed class NoJavaScriptRuntime : IJSRuntime
        {
            public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            {
                throw new InvalidOperationException("JavaScript interop calls cannot be issued during static rendering.");
            }

            public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
            {
                throw new InvalidOperationException("JavaScript interop calls cannot be issued during static rendering.");
            }
        }

        #endregion
    }
}
