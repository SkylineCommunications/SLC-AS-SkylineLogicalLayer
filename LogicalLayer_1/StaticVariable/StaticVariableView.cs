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
        private string _startTimeoutLabel = "Window will close in ";
        private Label _timeout = new Label() { Width = 200 };
        private DateTime _closingTime;
        private Timer _timer;

        public StaticVariableView(IEngine engine, DateTime closingTime) : base(engine)
        {
            _closingTime = closingTime;
            _timer = new Timer(20000);
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
            _timeout.Text = _startTimeoutLabel + closingTime.Subtract(DateTime.Now).TotalMinutes.ToString("F0") + " min";
            Title = "Static Variable";
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
            StaticVariableName.Changed += KeepAlive;
            StaticVariableValue.Changed += KeepAlive;
            Add.Pressed += Add_Pressed;
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

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timeout.Text = _startTimeoutLabel + _closingTime.Subtract(DateTime.Now).TotalMinutes.ToString("F0") + " min";
            SetupLayout();
        }

        private void KeepAlive(object sender, EventArgs e)
        {
            Engine.KeepAlive();
            UpdateClosingTime.Invoke(this, EventArgs.Empty);
            _closingTime = DateTime.Now + Engine.Timeout;
            _timeout.Text = _startTimeoutLabel + _closingTime.Subtract(DateTime.Now).TotalMinutes.ToString("F0") + " min";
            SetupLayout();
        }

        private void Add_Pressed(object sender, EventArgs e)
        {
            Engine.KeepAlive();
            UpdateClosingTime.Invoke(this, EventArgs.Empty);
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
                orderedWidgets: new Widget[] { _timeout });

            Show(false);
        }
    }
}
