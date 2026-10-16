using System;
using System.Collections.Generic;
using Blazor.Toolkit.Samples.Client.Pages.Overview;

namespace Blazor.Toolkit.Samples.Client.Services;

/// <summary>
/// Bundled release notes used as a last-resort fallback when the
/// GitHub Releases API is unreachable from the browser (CORS, offline,
/// rate-limit, etc). The shape matches the <see cref="ReleaseNotes"/>
/// page's internal model so the page can render without code changes.
/// </summary>
/// <remarks>
/// Each body is a verbatim copy of the corresponding release body served
/// by the GitHub Releases API. Auto-linkify on the page (bare URLs,
/// `*.md` filenames, `#NNN` PR/issue refs, and `@user` mentions) and the
/// markdown sanitizer work on these bodies too, so the fallback renders
/// the same way as the live API content.
/// </remarks>
internal static class StaticFallbackReleases
{
    public static List<ReleaseNotes.FallbackReleaseEntry> GetAll() => All;

    private static readonly List<ReleaseNotes.FallbackReleaseEntry> All = new()
    {
        new()
        {
            TagName = "v2.0.0",
            Name = "v2.0.0",
            HtmlUrl = "https://github.com/syncfusion/blazor-toolkit/releases/tag/v2.0.0",
            PublishedAt = new DateTimeOffset(2026, 10, 9, 15, 13, 37, TimeSpan.Zero),
            IsLatest = true,
            Body = """
## v2.0.0

### Breaking Changes

- **Namespace consolidation**: Domain-specific namespaces were removed. All public types now live under `Syncfusion.Blazor.Toolkit`.
  - Migration: Replace the old domain imports with `@using Syncfusion.Blazor.Toolkit` in `_Imports.razor` or C# files. Update any fully qualified type names.
- **API surface (approved 2.0 contract)**: Compared to NuGet 1.0.2, 194 named compatibility diagnostics are accepted for this major version. See API-COMPATIBILITY.md and API-BREAK-INVENTORY.md for the full list and migration guidance.
  - **Chart**: Many renderer and internal types are now `internal`. Use the public components, settings, events, and templates. Don't subclass or construct internal renderers.
  - **Collections**: Several public `List<T>` members are now `IList<T>` or other interface contracts. Don't cast results to `List<T>`.
  - **Getter-only ownership**: `UploadedFiles.Files` and protected `ChartData.CurrentPoints` are getter-only. Mutate them with `Clear`/`Add` instead of replacing the list.
  - **Parent properties**: `*Parent` properties on date, time, and textbox components are `object` instead of `dynamic`.
  - **Helpers**: `ChartHelper` and `DataVizCommonHelper` can no longer be instantiated or subclassed. `StringToNumber` remains where applicable.
  - **`DataAdaptor<T>`**: Subclasses must declare `where T : notnull`.
- **Theme delivery model**: The new `SfThemeRoot` emitter writes the shared theme layer as a single `<style id="sf-theme-root">` element. The layer contains:
  - `:root` design tokens
  - base and utility styles
  - the icon font
  - keyframes
  - dark, high-contrast, and forced-colors rules
- **CSS isolation**: Component styles moved to Blazor CSS isolation (`.razor.css` and a scoped bundle). Selectors and cascade behavior may differ from the old global stylesheet. App-level overrides that targeted unscoped toolkit class names may need adjusting.

### Added

- `SfThemeRoot` and `SfThemeScope`, with renderer-scoped owner election and recovery.
- A JavaScript fallback that injects the theme into `document.head` when `<SfThemeRoot />` is omitted. It only runs when no owner exists for that renderer.
- Contrast theme support, readiness reports, and an updated sample browser UI (#86).
- Unit tests for theme-root emission, ownership, and placement (`SfThemeRootTests`, `SfThemeRootEnforcementTests`).
- Lifecycle standardization (#65).

### Fixed

**Styling and theming**
- Styles not applied in static SSR mode. Theme and component styles now emit during the server render pass.
- Spinner and Checkbox regressions after the CSS isolation migration.
- Missing styles in Calendars and TextArea.
- NumericTextBox and Calendar styles after the isolation migration.
- Invalid selectors in generated CSS.
- Loss of dark-theme color overrides after the isolation and theme-root changes.
- Conflict resolution and stability in `SfThemeRoot` and theme-scope ownership.

**Charts**
- Chart rendering under static SSR. The default SVG size and layout path now work when post-render measurement or JS is unavailable (#66).
- Internal chart components now render through typed render fragments instead of unresolved Razor tags. The renderer classes are not exposed publicly.
- Label and index alignment is preserved when sorting date-category chart points with null X values.
- Stripline tooltips now use the owning chart's theme.
- Axis-overlap state is isolated per chart layout, so concurrent charts no longer clear each other's previous-axis references and coordinates.
- Fixed Toolkit Charts warnings, including CS8602 (#69, #71).

**Components and data**
- Corrected nullable data, reflection, calendar, input, and dialog flows.
  - Null grouping keys are retained.
  - Reflection accessors are no-ops where appropriate.
  - Prompt cancellation returns null.
- Cleared analyzer warnings and nullable issues, and fixed NumericTextBox defects (#94).

**Tests and samples**
- Restored bUnit compilation, configured chart-only browser interop fixtures, and synchronized asynchronous rendering assertions.
- Fixed server app stylesheet issues in the sample host.
- Fixed console errors in the sample browser.

### Migration Notes

- 2.0.0 is not source-compatible with 1.0.2.
- Replace all domain-specific `@using` directives with `@using Syncfusion.Blazor.Toolkit`.
- Review custom CSS overrides after the CSS isolation migration.
- Make sure toolkit components, or an explicit `SfThemeRoot`, are present in apps that need theme generation.
- Update code that consumes `SfDialog.GetButtonItems()` to use `IList<DialogButton>?` or another suitable collection interface.
- Propagate `where T : notnull` when extending `DataAdaptor<T>`.

## What's Changed

* Lifecycle standardization changes included by @Indhumathy-Loganathan in https://github.com/syncfusion/blazor-toolkit/pull/65
* Fixed the warnings in the Toolkit Charts by @Yokesh-SF4393 in https://github.com/syncfusion/blazor-toolkit/pull/69
* Fixed CS8602 warnings in the toolkit charts by @Yokesh-SF4393 in https://github.com/syncfusion/blazor-toolkit/pull/71
* Fixed Blazor Toolkit chart rendering issue in SSR by @Yokesh-SF4393 in https://github.com/syncfusion/blazor-toolkit/pull/66
* Included Contrast theme support, readiness reports and updated sample browser UI by @Indhumathy-Loganathan in https://github.com/syncfusion/blazor-toolkit/pull/86
* Clear analyzer warnings, nullable issues, and NumericTextBox defects by @Indhumathy-Loganathan in https://github.com/syncfusion/blazor-toolkit/pull/94
* CSS isolation, namespace consolidation, and SfThemeRoot theme delivery by @Indhumathy-Loganathan in https://github.com/syncfusion/blazor-toolkit/pull/97
* Dependency updates (npm packages and GitHub Actions) by @dependabot[bot]: #52, #59, #61, #63, #64, #67, #68, #72, #73, #74, #75, #76, #87, #88, #89, #90, #91, #92, #95

**Full Changelog**: https://github.com/syncfusion/blazor-toolkit/compare/v1.0.2...v2.0.0
"""
        },
        new()
        {
            TagName = "v1.0.2",
            Name = "v1.0.2",
            HtmlUrl = "https://github.com/syncfusion/blazor-toolkit/releases/tag/v1.0.2",
            PublishedAt = new DateTimeOffset(2026, 9, 1, 14, 34, 46, TimeSpan.Zero),
            IsLatest = false,
            Body = """
## What's Changed
## Common

### Features

- **Accessibility Improvements**: Improved ARIA attributes, accessible names, keyboard navigation, and focus management across all Blazor Toolkit components, including Charts. Enhanced screen reader support for NVDA and Narrator. by @Indhumathy-Loganathan in https://github.com/syncfusion/blazor-toolkit/pull/34 and https://github.com/syncfusion/blazor-toolkit/pull/37

- **High Contrast Theme Support**: Added support for HighContrast and HighContrast-Light themes. Introduced dedicated High Contrast styling across components and improved support for Windows forced-colors mode. by @Indhumathy-Loganathan in https://github.com/syncfusion/blazor-toolkit/pull/58

- **Lifecycle Updates**: Standardized component initialization, parameter updates, rendering, and disposal patterns across all Blazor Toolkit components.


## Charts

### Features

- Updated `ChartSorting` to prevent modification of bound parameter values (`PropertyName` and `Direction`) during component operation. by @Yokesh-SF4393 in https://github.com/syncfusion/blazor-toolkit/pull/33

- Added validation for required chart configuration properties, including `XName`, `YName`, and `PropertyName`. by @Yokesh-SF4393 in https://github.com/syncfusion/blazor-toolkit/pull/31

- Improved text measurement caching to reduce repeated JavaScript interop operations across chart instances. by @Yokesh-SF4393 in https://github.com/syncfusion/blazor-toolkit/pull/15 and https://github.com/syncfusion/blazor-toolkit/pull/44

- Simplified SVG markup by removing unnecessary inline styling and mutable style definitions.


## TextBox

### Breaking Changes

- Removed the `Multiline` property and its associated multiline rendering support. Use `SfTextArea` for multiline text input scenarios.


## TextArea

### Bug Fixes

- Resolved issues related to resize behavior.
- Fixed float label rendering issues.
- Fixed validation state reporting issues.


## CheckBox

### Breaking Changes

- Updated the tri-state selection sequence to: **Checked → Unchecked → Indeterminate → Checked**.

### Features

- Improved synchronization of the indeterminate state for visual rendering and assistive technologies.
- Improved form validation and checkbox grouping behavior.

### Bug Fixes

- Resolved issues related to state persistence and user interaction.

## Uploader

### Bug Fixes

- Resolved issues related to file list rendering and updates.


## Spinner

### Bug Fixes

- Resolved overlay rendering issues.


## Button and ButtonGroup

### Bug Fixes

- Resolved selection state issues in ButtonGroup components.

## New Contributors
* @PrinceOliver made their first contribution in https://github.com/syncfusion/blazor-toolkit/pull/38
* @dependabot[bot] made their first contribution in https://github.com/syncfusion/blazor-toolkit/pull/50

**Full Changelog**: https://github.com/syncfusion/blazor-toolkit/compare/v1.0.1...v1.0.2
"""
        },
        new()
        {
            TagName = "v1.0.1",
            Name = "v1.0.1",
            HtmlUrl = "https://github.com/syncfusion/blazor-toolkit/releases/tag/v1.0.1",
            PublishedAt = new DateTimeOffset(2026, 6, 26, 13, 48, 3, TimeSpan.Zero),
            IsLatest = false,
            Body = """
# Release Notes - v1.0.1

This release delivers new components across the Syncfusion Blazor Toolkit. Version 1.0.1 expands component capabilities with improved performance, accessibility refinements, and new customization options. The 16 production-ready components have been enhanced based on user feedback to provide a more robust and developer-friendly experience.

## New Components

**Button:** Used for triggering actions in web apps. Supports different looks, icons, and toggle states.

> - **Multiple Visual Styles** - Choose from Default, Outline, Flat, or Round buttons with color options like Info, Success, Warning, and Danger.
> - **Icon Placement** - Add icons on the left, right, top, or bottom of button text using the IconCss property.
> - **Toggle State** - Turn buttons into switches that stay on or off until clicked again, great for settings and bookmarks.
> - **Form Support** - Works with Blazor forms for submit and reset actions with built-in validation.


**ButtonGroup:** Is a container component that groups multiple buttons together with support for single and multiple selection modes.

> - **Selection Modes** - Allows single (radio-like) or multiple (checkbox-like) button selection within the group for toolbars and filter interfaces.
> - **Vertical Layout** - Arranges buttons in a vertical stack when IsVertical is set to true, ideal for sidebars and tool panels.
> - **Icon Support** - Adds icons to buttons using IconCss property with flexible positioning (left, right, top, bottom) around button text.
> - **Form Integration** - Uses Name and Value properties to submit selected button data in forms, making it work seamlessly with Blazor's form validation.
> 

**Calendar:** Used for displaying and selecting dates with support for multiple views, multi-selection, and international calendar systems.

> - **Multi-Date Selection** - Allows selecting multiple non-consecutive dates at once, ideal for scheduling and event planning.
> - **Islamic Calendar Support** - Switches to Hijri calendar system using CalendarMode property for Arabic and Islamic applications.
> - **Week Number Display** - Shows week numbers alongside dates with configurable first week rule for business reporting needs.
> - **Date Range Restriction** - Limits selectable dates using Min and Max properties, perfect for booking systems and form validation.

**DatePicker:** Combines a text input with an embedded calendar popup for intuitive date selection.

> - **Input Mask Support** - Guides date entry with a specific format pattern, preventing invalid date inputs and showing placeholders for each segment.
> - **Multiple Input Formats** - Accepts various date format patterns when typing dates manually, with support for culture-specific formats like ISO and Gregorian.
> - **Date Range Restriction** - Limits selectable dates using Min and Max properties, with out-of-range dates disabled in the popup.
> - **Floating Label** - Placeholder text floats above the input when focused or filled, providing a clean interface that saves vertical space.
> 

**DateTimePicker:** Enables selection of both date and time values in a unified interface with configurable formats and time intervals.

> - **Time Interval Configuration** - Set time intervals (e.g., 15, 30, 60 minutes) to control the granularity of time selection in the dropdown list.
> - **Time Range Restriction** - Separate MinTime and MaxTime properties restrict time selection independent of date constraints, perfect for business hours.
> - **Date and Time Format** - Customize display using standard .NET format patterns like "dd/MM/yyyy hh:mm:ss" with culture-specific support.
> - **Mask Input Support** - Guides datetime entry with a mask pattern based on the Format property, showing placeholders for each segment.

**TimePicker:** Provides specialized time-only selection with configurable intervals, format options, and validation for precise time entry.

> - **Flexible Time Intervals** - Configure the Step property (e.g., 15, 30, 60 minutes) to control the granularity of time options displayed in the popup list.
> - **Time Range Restriction** - Set Min and Max time values to restrict selectable times within a specific range, with out-of-range times automatically disabled.
> - **12/24 Hour Formats** - Support for both 12-hour format with AM/PM (h:mm tt) and 24-hour format (HH:mm), plus optional seconds display.
> - **Mask Input Support** - Enable the EnableMask property to guide time entry with format placeholders, preventing invalid input and showing segment indicators.

**Chart:** Provides feature-rich data visualization with multiple chart types and interactive capabilities.

> - **New Chart Types** - Added support for a wide range of chart types including Area, Bar, Bubble, Column, Line, Scatter, Spline (with Area), Multicolored (Area and Line), Step (Line and Area), and stacking variants with 100% options to support diverse visualization needs.
> - **Indicators and Trendlines** - Supports rendering of technical indicators and trendlines to help identify patterns, trends, and analytical insights directly within the chart.
> - **Annotations and Data Labels** - Enables annotations and data labels to highlight specific data points and provide contextual information for better understanding.
> - **Zooming and Panning** - Allows smooth zooming and panning to explore data efficiently across different ranges and scales.
> - **Selection and Highlight** - Provides interactive selection and highlighting to focus on specific data segments for detailed analysis.
> - **Crosshair** - Displays crosshair lines for precise tracking of values across both axes.
> - **Tooltip** - Shows enhanced tooltips with detailed data information when hovering over chart elements.
> - **Legend with Toggle Support** - Allows users to dynamically show or hide series using interactive legend toggle functionality.
> - **Gradient Support** - Supports gradient fills for series, indicators, and trendlines to improve visual appearance.
> - **Strip Lines** - Highlights specific ranges or threshold areas within the chart using configurable strip lines.
> - **Markers** - Enables customizable markers to emphasize individual data points clearly.
> - **Multiple Axis** - Supports multiple axes to represent different data scales within a single chart.
> - **Multiple Panes** - Allows rendering of multiple panes to organize related datasets into separate sections within the same chart.

**CheckBox:** Enables users to make binary or tri-state selections with support for labels, custom styling, and form integration.

> - **Tri-State Mode** - Supports checked, unchecked, and indeterminate states via EnableTriState property, ideal for "Select All" scenarios in hierarchical lists.
> - **Label Positioning** - Flexible label placement using LabelPosition property (Before or After), adapting to RTL layouts automatically.
> - **Theme Colors** - Built-in color classes like e-primary, e-success, e-warning, and e-danger for matching application themes.
> - **Form Validation** - Seamless integration with Blazor's EditForm and DataAnnotations for model-based validation scenarios.

**NumericTextBox:** Provides numeric input with formatting, validation, and spin button controls for precise number entry.

> - **Currency Formatting** - Display values with currency symbols using Format="C2" and culture-specific decimal separators.
> - **Percentage Display** - Format values as percentages using Format="P2" with automatic decimal conversion between display and raw values.
> - **Custom Format Patterns** - Use # and 0 placeholders to create custom display patterns like "###.##" for optional decimals or "000.00" for zero-padded values.
> - **Strict Mode** - Automatically constrains values between Min and Max bounds, correcting out-of-range inputs on blur.

**RadioButton:** Enables users to select a single option from a group of mutually exclusive choices with label and styling support.

> - **Label Positioning** - Place labels before or after the radio button using LabelPosition property, with automatic RTL layout adaptation.
> - **Disabled State** - Prevent interaction on individual radio buttons using the Disabled property while retaining visual presence.
> - **Theme Colors** - Apply built-in color classes like e-success, e-info, e-warning, and e-danger to match application themes.
> - **Form Validation** - Seamless integration with Blazor's EditForm and DataAnnotations for model binding and validation scenarios.

**Switch:** Provides a toggle control for boolean state selection with customizable on/off labels and seamless form integration.

> - **On/Off State Labels** - Display text inside the switch for ON and OFF states using OnLabel and OffLabel properties, improving user understanding of current state.
> - **Disabled State** - Prevent user interaction using the Disabled property while maintaining visual presence, ideal for conditional form fields.
> - **Form Validation** - Seamless integration with Blazor's EditForm and DataAnnotations for model-based validation scenarios.

**TextArea:** Provides multi-line text input with configurable rows, columns, and resize behavior for flexible text entry.

> - **Resize Mode** - Control resize direction using ResizeMode property (None, Horizontal, Vertical, Both) to limit or allow user resizing.
> - **Row and Column Count** - Set visible dimensions using RowCount (lines) and ColumnCount (character width) properties for appropriate input sizing.
> - **Float Label Types** - Support for floating labels using FloatLabelType property (Never, Auto, Always) with placeholder text behavior.
> - **Maximum Length** - Enforce character limits using MaxLength property, preventing input beyond specified limit.

**TextBox:** Provides comprehensive text input with support for multiple input types, multiline mode, and customizable autocomplete behavior.

> - **Input Types** - Configure input behavior using Type property (Text, Password, Email, Number, Tel, URL, Search) for appropriate keyboards and validation.
> - **Multiline Mode** - Enable multiline textarea input using Multiline property for capturing longer text like comments and descriptions.
> - **Autocomplete** - Control browser autocomplete suggestions using Autocomplete property (On/Off) for sensitive fields like passwords.
> - **Clear Button** - ShowClearButton property displays a clear button when typing, allowing one-click content clearing.

**Uploader:** Provides file upload with drag-and-drop, progress tracking, and validation for reliable file transfers.

> - **Chunk Upload** - Upload large files by splitting them into smaller chunks using OnChunkUploadStart, OnChunkSuccess, and OnChunkFailure events for reliable uploads.
> - **File Validation** - Restrict file types using AllowedExtensions property and validate size using MaxFileSize and MinFileSize properties.
> - **Directory Upload** - Enable DirectoryUpload property to allow uploading entire folders instead of individual files.
> - **Sequential Upload** - Enable SequentialUpload property to upload multiple files one after another instead of simultaneously.
> - **Auto Upload** - AutoUpload property automatically uploads files immediately after selection without requiring manual trigger.

**Dialog:** Provides modal and non-modal overlay dialogs with drag, resize, and positioning support for versatile popup interactions.

> - **Modal Dialog** - IsModal property creates an overlay that blocks interaction with the rest of the application until the dialog is closed.
> - **Drag Support** - AllowDragging property enables dragging the dialog by its header to reposition within the viewport.
> - **Resize Support** - EnableResize property with ResizeHandles allows resizing dialog in specific directions like SouthEast, NorthEast, or All.
> - **Custom Positioning** - DialogPositionData property allows positioning dialog at specific X and Y coordinates within a target container.
> - **Template Support** - DialogTemplates component allows customization of Header, Content, and Footer areas with custom layouts.

**Tooltip:** Displays contextual information on hover, click, or focus with customizable positioning, animation, and content support.

> - **Animation Effects** - Animation property with Effect enum (FadeIn, FadeOut, ZoomIn, ZoomOut) controls smooth open and close transitions.
> - **Mouse Trail** - MouseTrail property enables tooltip to follow cursor movement within the target element for dynamic content tracking.
> - **Sticky Mode** - IsSticky property keeps tooltip displayed until manually closed by the user, ideal for persistent help content.
> - **Open Mode Control** - OpensOn property allows triggering tooltip on specific events - Auto, Hover, Click, Focus, or Custom.

**Spinner:** Provides circular loading indicators with customizable size, thickness, labels, and overlay support for seamless integration.

> - **Size Property** - Size property allows customizing spinner dimensions with pixel or percentage values for consistent visual scaling.
> - **Thickness Property** - Thickness property controls the stroke width of the spinner animation, allowing visual prominence adjustment.
> - **Label Property** - Label property displays descriptive text below the spinner animation to communicate what is being loaded.
> - **Custom Templates** - SpinnerTemplates component enables creating custom loading elements with CSS or SVG for brand-specific animations.
> - **Overlay Mode** - Adding e-spin-overlay class via CssClass property creates a modal overlay that blocks interaction during loading.
"""
        }
    };
}
