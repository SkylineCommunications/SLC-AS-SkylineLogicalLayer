using LogicalLayer_1.ParameterMonitor;
using LogicalLayer_1.Utils;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.Utils.InteractiveAutomationScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicalLayer_1.StaticVariable
{
    public class StaticVariableView : Dialog
    {
        private readonly IEngine _engine;
        private readonly Label _staticVariableName = new Label("Static Variable Name: ") { Width = 200 };
        private readonly Label _staticVariableValue = new Label("Value: ") { Width = 200 };

        public StaticVariableView(IEngine engine) : base(engine)
        {
            _engine = engine;
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
            Add.Pressed += Add_Pressed;
            Back.Pressed += (s, e) => OnBackPressed?.Invoke(this, EventArgs.Empty);
            Close.Pressed += (s, e) => OnClosePressed?.Invoke(this, EventArgs.Empty);
            SetupLayout();
        }

        public event EventHandler<StaticVariableEventArgs> OnAddPressed;

        public event EventHandler OnBackPressed;

        public event EventHandler OnClosePressed;

        public TextBox StaticVariableName { get; set; }

        public TextBox StaticVariableValue { get; set; }

        public Button MyProperty { get; set; }

        public Button Add { get; set; }

        public Button Back { get; set; }

        public Button Close { get; set; }

        private void Add_Pressed(object sender, EventArgs e)
        {
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
        }
    }
}
