namespace LogicalLayer_1.Utils
{
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class LayoutDesigner
    {
        public static readonly string OptionSelected = "-Select-";
        public static readonly string OptionNone = "None";
        public static readonly string OptionNew = "New";
        public static readonly string OptionCustom = "Custom";

        public static string GetValidValue(string[] options, string selectedValue, string valueWhenInvalid, bool preSelect = false)
        {
            bool invalid = string.IsNullOrEmpty(selectedValue) || !options.Contains(selectedValue);
            return VerifyPreselect(options, selectedValue, valueWhenInvalid, preSelect, invalid);
        }

        public static string VerifyPreselect(string[] options, string selectedValue, string valueWhenInvalid, bool preSelect, bool invalid)
        {
            bool preSelectPossible = preSelect && options.Length != 0 && (invalid || selectedValue == LayoutDesigner.OptionSelected);
            if (preSelectPossible)
            {
                return options[0];
            }

            return invalid ? valueWhenInvalid : selectedValue;
        }

        public static void SetComponentsOnRow(Dialog dialog, int row, Widget[] orderedWidgets, int rowSpan = 1, int colSpan = 1, int startColumn = 0)
        {
            for (int columnNbr = 0; columnNbr < orderedWidgets.Length; columnNbr++)
            {
                dialog.AddWidget(orderedWidgets[columnNbr], row, columnNbr + startColumn, rowSpan, colSpan);
            }
        }

        public static void SetComponentsOnRow(Section section, int row, Widget[] orderedWidgets, int rowSpan = 1, int colSpan = 1, int startColumn = 0)
        {
            for (int columnNbr = 0; columnNbr < orderedWidgets.Length; columnNbr++)
            {
                section.AddWidget(
                    orderedWidgets[columnNbr],
                    new WidgetLayout(row, columnNbr + startColumn, rowSpan, colSpan));
            }
        }

        public static void SetComponentsOnRow(Dialog dialog, int row, Widget[] orderedWidgets, HorizontalAlignment horizontalAlignment, int rowSpan = 1, int colSpan = 1, int startColumn = 0)
        {
            for (int columnNbr = 0; columnNbr < orderedWidgets.Length; columnNbr++)
            {
                dialog.AddWidget(orderedWidgets[columnNbr], row, columnNbr + startColumn, rowSpan, colSpan, horizontalAlignment);
            }
        }

        public static IEnumerable<string> GetDropdownValuesWithSelect(IEnumerable<string> options, string defaultValue = "-Select-", bool allowNone = false)
        {
            string selectedValue;
            if (options == null)
            {
                selectedValue = !allowNone ? defaultValue : "None";
                return new List<string> { selectedValue };
            }

            List<string> optionsToDisplay = options.ToList();
            selectedValue = defaultValue;
            if (string.IsNullOrEmpty(defaultValue) || defaultValue == "-Select-")
            {
                selectedValue = !allowNone ? "-Select-" : "None";
            }

            optionsToDisplay.Add(selectedValue);
            return optionsToDisplay;
        }

        public static IEnumerable<string> GetDropdownValuesWithoutSelect(IEnumerable<string> options)
        {
            string selectedValue;
            if (options == null)
            {
                selectedValue = "None";
                return new List<string> { selectedValue };
            }

            return options.ToList();
        }
    }
}
