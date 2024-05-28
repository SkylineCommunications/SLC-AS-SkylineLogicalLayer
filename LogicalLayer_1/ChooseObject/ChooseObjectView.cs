namespace LogicalLayer_1.ChoseObject
{
    using LogicalLayer_1.Utils;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;
    using System;

    public class ChooseObjectView : Dialog
    {
        public ChooseObjectView(IEngine engine)
            : base(engine)
        {
            Title = "Choose to populate table";
            ParameterMonitor = new Button("Parameter Monitor")
            {
                Width = 200,
            };
            ElementMonitor = new Button("Element Monitor")
            {
                Width = 200,
            };
            ViewMonitor = new Button("View Monitor")
            {
                Width = 200,
            };
            Condition = new Button("Condition")
            {
                Width = 200,
            };
            Close = new Button("Close")
            {
                Width = 200,
            };
            ParameterMonitor.Pressed += (s, e) => OnParameterMonitorPressed?.Invoke(this, EventArgs.Empty);
            ElementMonitor.Pressed += (s, e) => OnElementAlarmMonitorPressed?.Invoke(this, EventArgs.Empty);
            ViewMonitor.Pressed += (s, e) => OnViewMonitorPressed?.Invoke(this, EventArgs.Empty);
            Condition.Pressed += (s, e) => OnConditionPressed?.Invoke(this, EventArgs.Empty);
            Close.Pressed += (s, e) => OnClosePressed?.Invoke(this, EventArgs.Empty);
            SetupLayout();
        }

        public event EventHandler OnParameterMonitorPressed;

        public event EventHandler OnElementAlarmMonitorPressed;

        public event EventHandler OnViewMonitorPressed;

        public event EventHandler OnConditionPressed;

        public event EventHandler OnClosePressed;

        public Button ParameterMonitor { get; set; }

        public Button ElementMonitor { get; set; }

        public Button ViewMonitor { get; set; }

        public Button Condition { get; set; }

        public Button Close { get; set; }

        private void SetupLayout()
        {
            int rowNumber = 0;

            LayoutDesigner.SetComponentsOnRow(
                dialog: this,
                row: rowNumber,
                orderedWidgets: new Widget[] { ParameterMonitor });

            LayoutDesigner.SetComponentsOnRow(
                dialog: this,
                row: ++rowNumber,
                orderedWidgets: new Widget[] { ElementMonitor });

            LayoutDesigner.SetComponentsOnRow(
                dialog: this,
                row: ++rowNumber,
                orderedWidgets: new Widget[] { ViewMonitor });

            LayoutDesigner.SetComponentsOnRow(
                dialog: this,
                row: ++rowNumber,
                orderedWidgets: new Widget[] { Condition });

            LayoutDesigner.SetComponentsOnRow(
                dialog: this,
                row: ++rowNumber,
                orderedWidgets: new Widget[] { Close });
        }
    }
}
