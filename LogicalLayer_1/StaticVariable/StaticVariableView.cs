using LogicalLayer_1.ParameterMonitor;
using LogicalLayer_1.Utils;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.Utils.InteractiveAutomationScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace LogicalLayer_1.StaticVariable
{
    public class StaticVariableView : Dialog
    {
        private readonly Label _staticVariableName = new Label("Static Variable Name: ") { Width = 200 };
        private readonly Label _staticVariableValue = new Label("Value: ") { Width = 200 };
        private DateTime _closingTime;

        public StaticVariableView(IEngine engine, DateTime closingTime) : base(engine)
        {
            _closingTime = closingTime;
            Title = $"Static Variable - Will close at {_closingTime.TimeOfDay.Hours.ToString().PadLeft(2, '0')}:{_closingTime.TimeOfDay.Minutes.ToString().PadLeft(2, '0')}";
            StaticVariableName = new TextBox
            {
                Width = 200,
            };
            StaticVariableValue = new TextBox
            {
                Width = 200,
            };
            Add = new Button("Add")
            {
                Width = 200,
            };
            Back = new Button("Back")
            {
                Width = 200,
            };
            Close = new Button("Close")
            {
                Width = 200,
            };
            KeepAlive = new Button("Keep Alive")
            {
                Width = 200,
            };
            Add.Pressed += Add_Pressed;
            KeepAlive.Pressed += KeepAliveScript;
            Back.Pressed += (s, e) => OnBackPressed?.Invoke(this, EventArgs.Empty);
            Close.Pressed += (s, e) => OnClosePressed?.Invoke(this, EventArgs.Empty);
            SetupLayout();
        }

        public event EventHandler<StaticVariableEventArgs> OnAddPressed;

        public event EventHandler OnBackPressed;

        public event EventHandler OnClosePressed;

        public event EventHandler UpdateClosingTime;

        public TextBox StaticVariableName { get; set; }

        public TextBox StaticVariableValue { get; set; }

        public Button MyProperty { get; set; }

        public Button Add { get; set; }

        public Button Back { get; set; }

        public Button Close { get; set; }

        public Button KeepAlive { get; set; }

        private void KeepAliveScript(object sender, EventArgs e)
        {
            Engine.KeepAlive();
            UpdateClosingTime.Invoke(this, EventArgs.Empty);
            _closingTime = DateTime.Now + Engine.Timeout;
            Title = $"Static Variable - Will close at {_closingTime.TimeOfDay.Hours.ToString().PadLeft(2, '0')}:{_closingTime.TimeOfDay.Minutes.ToString().PadLeft(2, '0')}";
            SetupLayout();
        }

        private void Add_Pressed(object sender, EventArgs e)
        {
            KeepAliveScript(sender, e);
            if (String.IsNullOrWhiteSpace(StaticVariableName.Text))
            {
                return;
            }

            if (String.IsNullOrWhiteSpace(StaticVariableValue.Text))
            {
                return;
            }

            OnAddPressed?.Invoke(this, new StaticVariableEventArgs
            {
                Name = StaticVariableName.Text,
                Value = StaticVariableValue.Text,
            });
        }

        private void SetupLayout()
        {
            Clear();
            int rowNumber = 0;

            LayoutDesigner.SetComponentsOnRow(
                dialog: this,
                row: rowNumber,
                orderedWidgets: new Widget[] { _staticVariableName, StaticVariableName });

            LayoutDesigner.SetComponentsOnRow(
                dialog: this,
                row: ++rowNumber,
                orderedWidgets: new Widget[] { _staticVariableValue, StaticVariableValue });

            LayoutDesigner.SetComponentsOnRow(
                dialog: this,
                row: ++rowNumber,
                orderedWidgets: new Widget[] { Back, Add });

            LayoutDesigner.SetComponentsOnRow(
                dialog: this,
                row: ++rowNumber,
                orderedWidgets: new Widget[] { Close });

            LayoutDesigner.SetComponentsOnRow(
                dialog: this,
                row: ++rowNumber,
                orderedWidgets: new Widget[] { new WhiteSpace() });

            LayoutDesigner.SetComponentsOnRow(
                dialog: this,
            row: ++rowNumber,
                orderedWidgets: new Widget[] { KeepAlive });
        }
    }
}
