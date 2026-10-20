using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Syncfusion.Blazor.Toolkit.Internal;

namespace Syncfusion.Blazor.Toolkit
{
    /// <summary>
    /// Represents a single button within a <see cref="SfButtonGroup"/>. The button can display text, an icon, or both, and triggers an action when clicked.
    /// </summary>
    /// <remarks>
    /// The content for the <see cref="Button"/> can be defined using the <see cref="Content"/> property or by placing markup inside the component tag.
    /// </remarks>
    /// <example>
    /// This example demonstrates how to create a basic <see cref="SfButtonGroup"/> with several <see cref="Button"/> components.
    /// <code><![CDATA[
    /// <SfButtonGroup>
    ///   <Button Content="Left"></Button>
    ///   <Button Content="Center"></Button>
    ///   <Button Content="Right"></Button>
    /// </SfButtonGroup>
    /// ]]></code>
    /// </example>
    public partial class Button
    {

        #region Fields
        /// <exclude />
        /// <summary>
        /// Internal backing field for the selected state used during two-way binding updates.
        /// </summary>
        private bool _selected;

        /// <exclude />
        /// <summary>
        /// Tracks the target selected state during click handling for single-selection mode.
        /// </summary>
        private bool _buttonSelected;

        /// <exclude />
        /// <summary>
        /// Additional HTML attributes applied to the input element when the ButtonGroup uses selection mode.
        /// </summary>
        internal Dictionary<string, object> _inputAttributes = [];

        #endregion

        #region Internal properties

        /// <exclude />
        /// <summary>
        /// Reference to the parent <see cref="SfButtonGroup"/> that contains this button.
        /// </summary>
        [CascadingParameter]
        internal SfButtonGroup? ButtonGroup { get; set; }

        #endregion

        #region Helper Methods

        /// <exclude />
        private string GetSelectionCssClass()
        {
            string activeClass = _selected ? " e-active" : string.Empty;
            string cssClass = CssClass.Replace("e - primary", "e-primary", StringComparison.Ordinal);
            return string.IsNullOrEmpty(cssClass) ? "e-btn" + activeClass : "e-btn " + cssClass + activeClass;
        }

        #endregion

        #region Event Handlers

        /// <exclude />
        /// <summary>
        /// Handles keyboard interactions on the input element.
        /// </summary>
        /// <remarks>
        /// In single-selection (radio) mode, arrow keys natively move focus and change
        /// the checked radio. We mark that interaction so <see cref="HandleInputChangeAsync"/>
        /// can ignore it — selection should only change on Space, Enter, or mouse click.
        /// Space is handled natively (raises <c>change</c>). Enter is handled here.
        /// </remarks>
        /// <param name="args">The keyboard event arguments.</param>
        private async Task HandleInputKeyDownAsync(KeyboardEventArgs args)
        {
            if (args is null || string.IsNullOrEmpty(args.Key))
            {
                return;
            }

            // Single: arrow keys move focus only — ignore the following native change
            if (ButtonGroup?.Mode == SelectionMode.Single && IsArrowKey(args))
            {
                ButtonGroup._suppressChangeFromArrowKey = true;
                return;
            }

            // Space / Enter for both Single and Multiple
            // (bUnit only fires keydown; browser would also fire change — suppress that)
            if (args.Code == "Space" || args.Key == " " || args.Key == "Enter" || args.Code == "Enter")
            {
                if (ButtonGroup is not null)
                {
                    ButtonGroup._suppressChangeFromArrowKey = true;
                }

                await ClickHandlerAsync().ConfigureAwait(false);
            }
        }

        private static bool IsArrowKey(KeyboardEventArgs args)
        {
            return args.Key is "ArrowLeft" or "ArrowRight" or "ArrowUp" or "ArrowDown"
                || args.Code is "ArrowLeft" or "ArrowRight" or "ArrowUp" or "ArrowDown";
        }

        /// <exclude />
        /// <summary>
        /// Handles native <c>change</c> events from the underlying input element.
        /// This is the primary selection path for mouse clicks (via label[for]) and Space key.
        /// </summary>
        /// <param name="args">The change event arguments from the input.</param>
        private async Task HandleInputChangeAsync(ChangeEventArgs args)
        {
            if (ButtonGroup is null || Disabled || ButtonGroup._buttonItems is null)
            {
                return;
            }

            // Arrow navigation in Single mode (flag is on the group — change may be on another Button)
            // Ignore change after arrow (Single) or Space/Enter (already handled in keydown).
            // Prevents double-toggle in the browser; bUnit never fires change on KeyDown.
            if (ButtonGroup._suppressChangeFromArrowKey)
            {
                ButtonGroup._suppressChangeFromArrowKey = false;
                ButtonGroup.RefreshButtonsUi();
                return;
            }

            // Click / Space already handled this interaction
            if (ButtonGroup._isClicked)
            {
                return;
            }

            ButtonGroup._isClicked = true;

            if (ButtonGroup.Mode == SelectionMode.Multiple)
            {
                if (args?.Value is null)
                {
                    return;
                }

                bool newValue;
                try
                {
                    newValue = Convert.ToBoolean(args.Value, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (InvalidCastException)
                {
                    return;
                }
                catch (FormatException)
                {
                    return;
                }

                if (newValue != Selected)
                {
                    await UpdateButtonStateAsync(newValue).ConfigureAwait(false);
                }
            }
            else if (ButtonGroup.Mode == SelectionMode.Single)
            {
                await ButtonGroup.ClearSiblingsAsync(this).ConfigureAwait(false);
                await UpdateButtonStateAsync(true).ConfigureAwait(false);
            }
        }

        /// <exclude />
        /// <summary>
        /// Handles click interaction and coordinates selection state with the parent group.
        /// </summary>
        private async Task ClickHandlerAsync()
        {
            if (ButtonGroup is null || Disabled || ButtonGroup._buttonItems is null)
            {
                return;
            }

            // Another handler already processed this interaction
            if (ButtonGroup._isClicked)
            {
                return;
            }

            ButtonGroup._isClicked = true;

            if (ButtonGroup.Mode == SelectionMode.Multiple)
            {
                await UpdateButtonStateAsync(!Selected).ConfigureAwait(false);
                return;
            }

            if (ButtonGroup.Mode == SelectionMode.Single)
            {
                await ButtonGroup.ClearSiblingsAsync(this).ConfigureAwait(false);
                await UpdateButtonStateAsync(true).ConfigureAwait(false);
            }
        }

        #endregion

        #region Helper Methods

        /// <exclude />
        /// <summary>
        /// Asynchronously updates the selection state of the button.
        /// </summary>
        /// <param name="state">The new selection state to apply.</param>
        internal async Task UpdateButtonStateAsync(bool state)
        {
            bool previous = _selected;

            // Synchronous update first — prevents race with @onchange during await
            _selected = state;
            _buttonSelected = state;
            Selected = state;

            Selected = _selected = _buttonSelected =
                await SfBaseUtils.UpdatePropertyAsync(state, previous, SelectedChanged).ConfigureAwait(false);
        }

        /// <exclude />
        /// <summary>
        /// Triggers a re-render so <c>checked="@_selected"</c> is applied to the DOM.
        /// </summary>
        internal void NotifyStateChanged()
        {
            StateHasChanged();
        }

        #endregion
    }
}
