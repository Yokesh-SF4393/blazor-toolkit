# Roadmap — Syncfusion® Toolkit for Blazor

**Chart-first, quality before breadth.**

Chart is the flagship component. The MIT toolkit remains independently useful. **No Data Grid is planned.** Growth toward 40+ supporting components is exploratory, without a promised list or dates.

This document represents the major investments maintainers intend to pursue. It is **not** approval of a release schedule, a compatibility guarantee, or a funded support commitment. Component selection, scope, and timing remain subject to maintainer review and community feedback. Many items require investigation and design; plans may change, items may be cut or deferred, and this document will be updated to reflect progress and learnings.

[README](https://github.com/syncfusion/blazor-toolkit/blob/main/README.md) · [Changelog](https://github.com/syncfusion/blazor-toolkit/blob/main/CHANGELOG.md) · [Documentation index](https://github.com/syncfusion/blazor-toolkit/blob/main/.github/Index.md) · [Test evidence matrix](https://github.com/syncfusion/blazor-toolkit/blob/main/.github/TEST-MATRIX.md)

Community input is welcome. Prefer issues and discussions that include a concrete scenario, render-mode matrix, expected behavior, and measurements. Support for specific proposals is best expressed by reactions and detailed scenario comments on the original issue.

---

## Proposed themes

* **Address top feedback and pain points** — reliability, accessibility, and quality of what already ships
* **Fundamentals** — security, observability of quality (tests, a11y, trim/AOT evidence), supply-chain hygiene
* **Invest in the modern stack** — Chart as flagship; supporting form and layout primitives that complete common dashboards
* **Quality before breadth** — no aggressive series parity or dated component lists; no multi-provider theme matrix
* **Community-shaped exploration** — supporting components and optional tracks only after evidence gates

This roadmap deliberately does **not** adopt competitor shipping cadence, component counts, AI surfaces, or grid-family products that conflict with the charter or current source.

---

## How this document was shaped

| Pattern considered | Fit | What this roadmap does |
|---|---|---|
| Chart series expansion (pie/doughnut, radar, financial, waterfall, heatmap, range) | Suitable as evaluation only | Listed under Chart evolution. Not a delivery commitment. Public `ChartSeriesType` does not include these values. |
| Accessibility evidence over conformance claims | Suitable | Priority 2. WCAG 2.2 AA remains a goal; measured application-level results are the gate. |
| Trimming / WebAssembly AOT publish evidence | Suitable | Library already declares `IsTrimmable` / `IsAotCompatible` and enables analyzers. Missing piece: supported-host publish logs and runtime checks. |
| CSS-variable theming and opt-in alternate themes | Suitable, scoped | `SfThemeRoot` already emits tokens + dark / high-contrast / forced-colors. Material 3, Fluent 2, Bootstrap-aligned themes = evaluation only, must stay opt-in. |
| Localization and RTL | Suitable after quality gates | Culture-aware date/number formatting exists. First-class RTL surface is not in the public API → evaluation track. |
| Layout primitives for chart dashboards and forms | Suitable if gated | Tab, list, menu, dropdown/autocomplete planned for evaluation. Sidebar and Card are not yet public components. |
| Supply-chain and dependency triage | Suitable | Standing gate via [SECURITY.md](https://github.com/syncfusion/blazor-toolkit/blob/main/.github/SECURITY.md) and committed SBOM. |
| Community influence via issues/discussions | Suitable | Unchanged. Proposals need scenario, render mode, behavior, measurements. |
| AI chat, tool calling, smart paste, MCP helpers | Not suitable now | No AI surface in-repo. Deferred until Chart + a11y evidence is solid. |
| Data Grid, TreeGrid, Pivot, Spreadsheet, Gantt, Scheduler | Not suitable | Explicit non-goals. |
| Multi-CSS provider model (Bootstrap, Tailwind, Material, Ant Design as peers) | Not suitable | One tokenized theme + contrast variants only. |
| Aggressive series parity or dated component lists | Not suitable | Exploration only. No dates, no parity promises. |

---

## Present in the current source

Checked against package **2.0.1**. Public types live under `Syncfusion.Blazor.Toolkit`.

| Area | Current scope |
|---|---|
| **Chart** | 21 public `ChartSeriesType` values (line, area, column, bar, scatter, bubble, spline, spline area, step line, step area, multicolored line/area, stacking variants including 100% and stacking step area). Axes, legends, tooltips, selection, zoom/pan, annotations, data labels, trendlines, striplines. |
| **Buttons & selection** | Button, ButtonGroup, CheckBox, RadioButton, Switch |
| **Calendars** | Calendar, DatePicker, DateTimePicker, TimePicker |
| **Inputs** | TextBox, TextArea, NumericTextBox, Uploader |
| **Popups & notification** | Dialog, Tooltip, Spinner. `Popups/Popup` is a model only — not a shipped `SfPopup`. |
| **Theme** | `SfThemeRoot`: one shared CSS-variable layer (default, dark, high-contrast, forced-colors). Component CSS isolated. No alternate design-system themes shipped. |
| **Targets** | .NET 8 / 9 / 10. Render-mode requirements vary; TFMs and sample hosts do not prove equivalent behavior in every mode. See [component guidance](https://github.com/syncfusion/blazor-toolkit/blob/main/README.md#skills-and-component-guidance) and [render-mode guidance](https://github.com/syncfusion/blazor-toolkit/blob/main/.github/RENDER-MODE-SECURITY.md). |

**Already present (not future work):** Chart annotations, trendlines, striplines.

**Not in current Chart API:** pie/doughnut, radar/spider, polar, financial (candlestick/OHLC), waterfall, heatmap, range series, technical indicators.

**Explicitly absent and not committed:** Data Grid (or any grid family).

---

## Priorities

Order is intentional. Competitor breadth does not reorder these gates. Priority is driven by user impact, accessibility/security risk, maintenance cost, and available validation — **not** component count.

| Order | Focus | Scenarios and evidence sought |
|---|---|---|
| **1** | Chart reliability and usability | Null/empty data, sorting, concurrent layouts, live updates, disposal; reproducible regressions and measurements for larger datasets. No virtualization or performance threshold promised. |
| **2** | Accessibility in composed applications | Chart text alternatives, keyboard/focus, labels & validation, forced-colors, screen-reader checks; retain browser and AT versions + results. |
| **3** | Existing component quality | Calendar culture & date boundaries, input validation, upload failure handling, popup focus/exit, loading announcements. |
| **4** | Broader validation and release evidence | SDK/runtime job gaps, browser & render-mode checks, trimmed/WebAssembly AOT publish logs + runtime checks, package metadata and dependency evidence. |
| **5** | Supporting component breadth | Explore additions that complement charts and common forms/layouts — only after scope, maintenance cost, and evidence needs are reviewed. |

---

## Chart evolution

Chart remains the flagship. New series are **evaluation candidates**, not commitments. A candidate advances only with:

- concrete user scenario  
- render-mode matrix  
- accessibility plan  
- reproducible performance notes  

Live-update and disposal evidence for **already shipped** series stays ahead of new types.

| Candidate | Why listed | Gate |
|---|---|---|
| Pie / doughnut | Common composition next to cartesian series; not in `ChartSeriesType` | Scenario + a11y text alternative + empty-data behavior |
| Radar / spider | Common ask in comparable libraries; different axis model | Do not start until Priority 1 cartesian reliability evidence exists |
| Basic financial (candlestick / OHLC) | Distinct from a full stock-chart suite | Bounded scenario only; technical indicators and stock-chart shell out of scope |
| Waterfall, heatmap, range variants | Useful dashboard complements; each has layout + a11y cost | Evaluate one at a time; do not batch into a “series parity” release |

**Out of scope** unless both strong community demand **and** a validation plan exist: technical indicators, full stock-chart suite, 3D charts, any promise of virtualization or numeric performance thresholds.

---

## Supporting components

Names below are exploration and planning only. **Sidebar, Card, Tab, dropdowns, List, AutoComplete, and Menu are not present as public components.** Popup is a model under `Popups/Popup`, not a finished component. Order, scope, and timing may change.

| Status | Components | Notes |
|---|---|---|
| **In progress** | Sidebar, Card, Popup | Not shipped. Each needs public API sketch, accessibility plan, and render-mode evidence before treated as implemented. |
| **Planned for evaluation** | Tab, Dropdowns (select / autocomplete), List, AutoComplete, Menu | Prefer controls that complete common form and chart-dashboard layouts. |
| **Future opportunities** | Additional navigation/layout (app bar, drawer variants), data-entry helpers, basic timeline (only if evidence justifies) | Scheduler-like, Gantt, spreadsheet, and grid-family controls are **not** in this set. |

### Candidate checklist (required before implementation)

- What user scenario does it serve that existing Dialog, Tooltip, inputs, or Chart cannot?
- Which render modes are in scope, and which are explicitly unsupported?
- What is the accessibility plan (name, role, keyboard, focus exit, forced colors)?
- What is the bundle-size and theme-surface impact on the single tokenized theme?
- What localization or RTL surface does it add, if any?
- Does it reduce pressure to add a Data Grid, or does it quietly become one?

Inclusion in the table is **not** a delivery commitment, release guarantee, compatibility promise, or timeline.

---

## Cross-cutting tracks

These do **not** outrank Priorities 1–4.

### Theming
Keep the current tokenized theme (default, dark, high-contrast, forced-colors) as the default. Material 3, Fluent 2, or Bootstrap-aligned themes may be evaluated **only** as opt-in packs that reuse existing CSS variables. Do **not** adopt a multi-provider CSS architecture.

### Localization and RTL
Continue improving culture and date-boundary evidence for calendars and numeric inputs. A documented RTL/direction story is in scope only after the quality gates above. Do not claim RTL support the public API does not expose.

### Trimming, AOT, and bundle size
Declarations and analyzers are already enabled. Follow-up: supported-host publish logs and runtime checks. Analyzer-clean builds are not deployment certification.

### Supply chain
Continue monthly dependency triage, SBOM checksum review, and shipped-script coverage described in [SECURITY.md](https://github.com/syncfusion/blazor-toolkit/blob/main/.github/SECURITY.md) and [artifacts/sbom/README.md](https://github.com/syncfusion/blazor-toolkit/blob/main/artifacts/sbom/README.md). This is a standing release gate.

### AI-assisted experiences
**Not in scope at this time.** Optional prompt surfaces, tool calling, or documentation helpers will not be evaluated until Chart reliability and accessibility evidence are solid. Any future surface would need to stay optional and provider-agnostic. This is a deferral, not a roadmap item.

---

## Longer-term direction

Growing toward **40+ components** is a direction for exploration, not a committed inventory, release count, or delivery date. Proposals for supporting controls, theming, localization, and RTL must show a concrete application need and a validation plan.

This direction does **not** commit to Data Grid, TreeGrid, Pivot, or feature parity with the commercial Syncfusion suite (or any competitor).

### Explicit non-goals

* Data Grid or any grid family  
* Feature parity with the commercial Syncfusion Blazor suite or any competitor’s component list  
* Promised virtualization or numeric performance thresholds  
* AI chat or tool-calling components  
* A multi-CSS provider matrix  

---

## Current evidence vs planned validation

Figures below are retained evidence already recorded in this repository. They are **not** a new test run, remote CI results, or release guarantees. Re-count after meaningful test changes using the command in [TEST-MATRIX.md](https://github.com/syncfusion/blazor-toolkit/blob/main/.github/TEST-MATRIX.md). Historical releases: [CHANGELOG.md](https://github.com/syncfusion/blazor-toolkit/blob/main/CHANGELOG.md).

| Area | Evidence available now | Follow-up needed |
|---|---|---|
| **Build & unit tests** | [TEST-MATRIX.md](https://github.com/syncfusion/blazor-toolkit/blob/main/.github/TEST-MATRIX.md): bUnit **2,735** tests passed (0 failed, 0 skipped) on .NET 8 as of 2026-10-03 verification note. CI declared for bUnit on .NET 8/9/10. Library **2.0.1**. | Re-count after test changes. Broader runtime evidence and released-package checks remain open. |
| **Browser coverage** | Playwright projects for visual regression and accessibility smoke; CI lists them for .NET 8/9/10. | Interaction coverage, additional hosts/browsers, full required matrix. See TEST-MATRIX.md. |
| **Accessibility** | Source mechanisms, [ACCESSIBILITY.md](https://github.com/syncfusion/blazor-toolkit/blob/main/.github/ACCESSIBILITY.md), screen-reader smoke notes, [docs/VPAT-2.5-INT.md](https://github.com/syncfusion/blazor-toolkit/blob/main/docs/VPAT-2.5-INT.md). WCAG 2.2 AA goal, last reviewed for 2.0.1. | Measured application-level evaluations. In-repo VPAT is not by itself an approved ACR for every composed app. |
| **Trimming / AOT** | `IsTrimmable`, `IsAotCompatible`, trim/AOT analyzers set. | Supported-host publish logs and runtime checks. |
| **Security / supply chain** | [SECURITY.md](https://github.com/syncfusion/blazor-toolkit/blob/main/.github/SECURITY.md) (self-attestation 2026-10-19), [THREAT-MODEL.md](https://github.com/syncfusion/blazor-toolkit/blob/main/.github/THREAT-MODEL.md), monthly `dotnet list package --vulnerable`, NuGet audit, CycloneDX SBOM under [artifacts/sbom/](https://github.com/syncfusion/blazor-toolkit/blob/main/artifacts/sbom/README.md). | Ongoing dependency triage, shipped-script coverage, final-package checks. |
| **API compatibility** | [CHANGELOG.md](https://github.com/syncfusion/blazor-toolkit/blob/main/CHANGELOG.md), [API-COMPATIBILITY.md](https://github.com/syncfusion/blazor-toolkit/blob/main/docs/API-COMPATIBILITY.md), [API-BREAK-INVENTORY.md](https://github.com/syncfusion/blazor-toolkit/blob/main/docs/API-BREAK-INVENTORY.md) document the 2.0 contract. | Maintainer review for any further breaks. |

_Last evidence review: 2026-10-19, aligned to package 2.0.1 and in-repo evidence. This date does not approve a delivery schedule._
