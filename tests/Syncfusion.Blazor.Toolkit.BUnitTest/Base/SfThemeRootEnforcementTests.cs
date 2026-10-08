using System.Reflection;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Syncfusion.Blazor.Toolkit.Buttons;
using Syncfusion.Blazor.Toolkit.Calendars;
using Syncfusion.Blazor.Toolkit.Charts;
using Syncfusion.Blazor.Toolkit.Inputs;
using Syncfusion.Blazor.Toolkit.Popups;
using Syncfusion.Blazor.Toolkit.Spinner;
using Xunit;

namespace Syncfusion.Blazor.Toolkit.Tests.Base
{
    /// <summary>
    /// CI gate for the Tier-1 shared theme. The theme is only delivered by the root components that
    /// render <c>&lt;SfThemeRoot /&gt;</c>, so forgetting it on a new component silently re-introduces
    /// the "unstyled until something else renders" bug.
    /// <list type="bullet">
    /// <item>Every public component must be explicitly classified as a <i>root</i> or as a
    /// <i>child / infrastructure</i> component; a new unclassified component fails the build.</item>
    /// <item>Every root must render exactly one <c>SfThemeRoot</c>, and it must be the first thing its
    /// <c>BuildRenderTree</c> emits (a sibling placed before the root element - never inside it, never
    /// inside an <c>&lt;svg&gt;</c>, never inside relocated popup content).</item>
    /// </list>
    /// The check inspects the compiled IL of the Razor-generated <c>BuildRenderTree</c>, so it cannot be
    /// bypassed by formatting or by components that never call <c>base.BuildRenderTree()</c>.
    /// </summary>
    public sealed class SfThemeRootEnforcementTests
    {
        /// <summary>Consumer-facing root components. Each MUST render SfThemeRoot first.</summary>
        private static readonly Type[] ExpectedRoots =
        {
            typeof(SfButton),
            typeof(SfButtonGroup),
            typeof(SfCalendar<>),
            typeof(SfDatePicker<>),
            typeof(SfDateTimePicker<>),
            typeof(SfTimePicker<>),
            typeof(SfChart),
            typeof(SfCheckBox<>),
            typeof(SfNumericTextBox<>),
            typeof(SfRadioButton<>),
            typeof(SfSwitch<>),
            typeof(SfTextArea),
            typeof(SfTextBox),
            typeof(SfUploader),
            typeof(SfDialog),
            typeof(SfDialogProvider),
            typeof(SfTooltip),
            typeof(SfSpinner),
        };

        /// <summary>
        /// Public components that are NOT roots, with the reason they must not emit the theme.
        /// Adding a type here is a deliberate decision that must be reviewed.
        /// </summary>
        private static readonly Dictionary<string, string> KnownNonRoots = new()
        {
            ["Button"] = "SfButtonGroup child; its markup lives inside the group's element (input + label siblings) and the group is the root.",
            ["SfInputBase"] = "Structural wrapper (.e-input-base-wrapper) rendered by roots; every root renders SfThemeRoot before it.",
            ["SfDataManager"] = "Non-visual data component.",
            ["DialogButton"] = "SfDialog child configuration.",
            ["DialogButtons"] = "SfDialog child configuration.",
            ["DialogTemplates"] = "SfDialog child configuration.",
            ["DialogAnimationSettings"] = "SfDialog child configuration.",
            ["DialogPositionData"] = "SfDialog child configuration.",
            ["UploadedFile"] = "SfUploader child configuration.",
            ["UploadedFiles"] = "SfUploader internal list.",
            ["UploaderAsyncSettings"] = "SfUploader child configuration.",
            ["UploaderButtons"] = "SfUploader child configuration.",
            ["UploaderTemplates"] = "SfUploader child configuration.",
            ["SpinnerTemplates"] = "SfSpinner child configuration.",
            ["SpinnerBase"] = "SfSpinner internal base.",
            ["Border"] = "SfSpinner internal renderer.",
            ["MaskPlaceholder"] = "Date / time picker internal helper.",
            ["DatePickerMaskPlaceholder"] = "Date picker child configuration.",
            ["DateTimePickerMaskPlaceholder"] = "Date-time picker child configuration.",
            ["TimePickerMaskPlaceholder"] = "Time picker child configuration.",
            ["DataManager"] = "Non-visual data component.",
            ["JSInteropAdaptor"] = "Non-visual data adaptor component.",
            ["CalendarBase"] = "Abstract-by-convention base class of the calendar family.",
            ["CalendarBaseRender"] = "Internal calendar grid rendered inside a root.",
            ["CalendarDayCell"] = "Internal calendar grid cell.",
            ["CalendarTableHeader"] = "Internal calendar grid header.",
        };

        [Fact(Timeout = 30000, DisplayName = "Enforcement: every public component is classified as a root or a known child")]
        public void EveryPublicComponent_IsClassified()
        {
            List<string> unclassified = GetPublicComponents()
                .Where(t => !IsRoot(t) && !IsKnownNonRoot(t))
                .Select(t => t.FullName ?? t.Name)
                .OrderBy(n => n, StringComparer.Ordinal)
                .ToList();

            Assert.True(
                unclassified.Count == 0,
                "New public component(s) found that are not classified for the shared theme. If the component is " +
                "consumer-facing, render <SfThemeRoot /> as the first sibling of its root element and add it to " +
                "ExpectedRoots; if it is a child/internal component, add it to KnownNonRoots with a reason: " +
                Environment.NewLine + string.Join(Environment.NewLine, unclassified));
        }

        [Fact(Timeout = 30000, DisplayName = "Enforcement: ExpectedRoots only lists real public components")]
        public void ExpectedRoots_ArePublicComponents()
        {
            foreach (Type root in ExpectedRoots)
            {
                Assert.True(root.IsPublic, $"{root} must be public.");
                Assert.True(typeof(IComponent).IsAssignableFrom(root), $"{root} must be a component.");
            }
        }

        [Fact(Timeout = 30000, DisplayName = "Enforcement: every root renders exactly one SfThemeRoot, before any element or other component")]
        public void EveryRoot_RendersSfThemeRoot_FirstAndOnce()
        {
            List<string> failures = new();

            foreach (Type root in ExpectedRoots)
            {
                BuildRenderTreeScan scan = Scan(root);
                if (scan.ThemeRootCount == 0)
                {
                    failures.Add($"{root.Name}: does not render <SfThemeRoot />.");
                }
                else if (scan.ThemeRootCount > 1)
                {
                    failures.Add($"{root.Name}: renders <SfThemeRoot /> {scan.ThemeRootCount} times (must be exactly once).");
                }
                else if (!scan.ThemeRootIsFirst)
                {
                    failures.Add($"{root.Name}: <SfThemeRoot /> must be the first rendered node (a sibling BEFORE the root element, never inside it).");
                }
            }

            Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
        }

        [Fact(Timeout = 30000, DisplayName = "Enforcement: non-root components never emit the theme (it would land inside the parent's markup)")]
        public void KnownNonRoots_DoNotRenderSfThemeRoot()
        {
            List<string> failures = GetPublicComponents()
                .Where(t => !IsRoot(t) && IsKnownNonRoot(t))
                .Where(t => Scan(t).ThemeRootCount > 0)
                .Select(t => t.FullName ?? t.Name)
                .ToList();

            Assert.True(
                failures.Count == 0,
                "These child components render <SfThemeRoot /> but are registered as non-roots: " + string.Join(", ", failures));
        }

        [Fact(Timeout = 30000, DisplayName = "Enforcement: the render-time emitter is a pure sibling (does not wrap or alter the component root)")]
        public void SfThemeRoot_HasNoParametersOrChildContent()
        {
            PropertyInfo[] parameters = typeof(SfThemeRoot)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(p => p.GetCustomAttribute<ParameterAttribute>() is not null)
                .ToArray();

            Assert.Empty(parameters);
        }

        [Fact(Timeout = 30000, DisplayName = "Enforcement: process-wide ownership state is forbidden in SfThemeRoot / SfThemeScope")]
        public void NoProcessWideOwnershipFlag()
        {
            foreach (Type type in new[] { typeof(SfThemeRoot), typeof(SfThemeScope) })
            {
                IEnumerable<FieldInfo> fields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                foreach (FieldInfo field in fields)
                {
                    bool isBoolOrInt = field.FieldType == typeof(bool) || field.FieldType == typeof(int) || field.FieldType == typeof(long);
                    Assert.False(isBoolOrInt && !field.IsInitOnly && !field.IsLiteral,
                        $"{type.Name}.{field.Name}: a mutable static flag is a process-wide ownership flag, which is forbidden (ownership must be renderer-scoped).");
                }
            }
        }

        #region Helpers

        private static IEnumerable<Type> GetPublicComponents()
        {
            return typeof(SfThemeRoot).Assembly.GetExportedTypes()
                .Where(t => typeof(IComponent).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface && t != typeof(SfThemeRoot))
                // Chart child/configuration components (series, axes, markers, renderers, ...) live under
                // Syncfusion.Blazor.Toolkit.Charts and are rendered inside SfChart, the only chart root.
                .Where(t => !(IsChartNamespace(t) && t != typeof(SfChart)));
        }

        private static bool IsChartNamespace(Type type)
        {
            return type.Namespace is { } ns && ns.StartsWith("Syncfusion.Blazor.Toolkit.Charts", StringComparison.Ordinal);
        }

        private static bool IsRoot(Type type)
        {
            Type definition = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
            return ExpectedRoots.Contains(definition);
        }

        private static bool IsKnownNonRoot(Type type)
        {
            string name = type.Name;
            int tick = name.IndexOf('`', StringComparison.Ordinal);
            if (tick >= 0)
            {
                name = name[..tick];
            }

            return KnownNonRoots.ContainsKey(name);
        }

        private sealed record BuildRenderTreeScan(int ThemeRootCount, bool ThemeRootIsFirst);

        /// <summary>
        /// Scans the IL of the most-derived <c>BuildRenderTree</c> for calls to
        /// <c>RenderTreeBuilder.OpenElement / OpenComponent&lt;T&gt;</c> and reports how many of them open
        /// <see cref="SfThemeRoot"/> and whether the first one precedes every other element / component.
        /// </summary>
        private static BuildRenderTreeScan Scan(Type type)
        {
            MethodInfo? method = null;
            for (Type? t = type; t is not null && t != typeof(ComponentBase) && t != typeof(object); t = t.BaseType is { IsGenericType: true } b ? b.GetGenericTypeDefinition() : t.BaseType)
            {
                method = t.GetMethod(
                    "BuildRenderTree",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly,
                    binder: null,
                    types: new[] { typeof(RenderTreeBuilder) },
                    modifiers: null);
                if (method is not null)
                {
                    break;
                }
            }

            if (method?.GetMethodBody() is not { } body)
            {
                return new BuildRenderTreeScan(0, false);
            }

            byte[] il = body.GetILAsByteArray() ?? Array.Empty<byte>();
            Module module = method.Module;
            Type[]? typeArgs = method.DeclaringType!.IsGenericTypeDefinition ? method.DeclaringType.GetGenericArguments() : null;

            int themeRootCount = 0;
            int firstThemeRootOffset = -1;
            int firstOtherOffset = -1;

            for (int i = 0; i < il.Length - 4; i++)
            {
                if (il[i] != OpCodes.Call.Value && il[i] != OpCodes.Callvirt.Value)
                {
                    continue;
                }

                int token = BitConverter.ToInt32(il, i + 1);
                int table = token >>> 24;
                if (table is not (0x0A or 0x2B or 0x06))
                {
                    continue;
                }

                MethodBase? target;
                try
                {
                    target = module.ResolveMethod(token, typeArgs, null);
                }
                catch (Exception ex) when (ex is ArgumentException or BadImageFormatException or NotSupportedException)
                {
                    continue;
                }

                if (target is not MethodInfo { DeclaringType: { } declaring } info || declaring != typeof(RenderTreeBuilder))
                {
                    continue;
                }

                if (info.Name == nameof(RenderTreeBuilder.OpenComponent) && info.IsGenericMethod)
                {
                    if (info.GetGenericArguments()[0] == typeof(SfThemeRoot))
                    {
                        themeRootCount++;
                        if (firstThemeRootOffset < 0)
                        {
                            firstThemeRootOffset = i;
                        }
                    }
                    else if (firstOtherOffset < 0)
                    {
                        firstOtherOffset = i;
                    }
                }
                else if (info.Name == nameof(RenderTreeBuilder.OpenElement) || info.Name == nameof(RenderTreeBuilder.OpenComponent))
                {
                    if (firstOtherOffset < 0)
                    {
                        firstOtherOffset = i;
                    }
                }
            }

            bool isFirst = firstThemeRootOffset >= 0 && (firstOtherOffset < 0 || firstThemeRootOffset < firstOtherOffset);
            return new BuildRenderTreeScan(themeRootCount, isFirst);
        }

        #endregion
    }
}
