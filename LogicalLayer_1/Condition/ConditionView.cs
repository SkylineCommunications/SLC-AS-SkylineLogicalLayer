using LogicalLayer_1.ParameterMonitor;
using LogicalLayer_1.Utils;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.CICD.Parsers.Protocol.Xml;
using Skyline.DataMiner.Net.Messages.SLDataGateway;
using Skyline.DataMiner.Utils.InteractiveAutomationScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicalLayer_1.Condition
{
    public class ConditionView : Dialog
    {
        private readonly IEngine _engine;
        private readonly Label _conditionName = new Label("Condition Name: ") { Width = 200 };
        private readonly Label _condition = new Label("Condition: ") { Width = 200 };
        private readonly Label _visualize = new Label("Visualize: ") { Width = 200 };

        public ConditionView(IEngine engine) : base(engine)
        {
            _engine = engine;
            Title = "Condition";
            ConditionName = new TextBox
            {
                Width = 200,
                Height = 20,
            };
            Condition = new TextBox
            {
                Width = 200,
                Height = 80,
                IsMultiline = true,
            };
            Visualize = new DropDown
            {
                Width = 200,
                Options = new List<string>
                {
                    "Yes",
                    "No",
                },
                Selected = "Yes",
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
            Back.Pressed += Back_Pressed;
            Close.Pressed += (s, e) => OnClosePressed?.Invoke(this, EventArgs.Empty);
            SetupLayout();
        }

        public event EventHandler OnBackPressed;

        public event EventHandler<ConditionEventArgs> OnAddConditionPressed;

        public event EventHandler OnClosePressed;

        public TextBox ConditionName { get; set; }

        public TextBox Condition { get; set; }

        public DropDown Visualize { get; set; }

        public Button Add { get; set; }

        public Button Back { get; set; }

        public Button Close { get; set; }

        private void Back_Pressed(object sender, EventArgs e)
        {
            OnBackPressed?.Invoke(sender, e);
        }

        private void Add_Pressed(object sender, EventArgs e)
        {
            OnAddConditionPressed?.Invoke(sender, new ConditionEventArgs
            {
                ConditionName = ConditionName.Text,
                Condition = Condition.Text,
                Visualize = Visualize.Selected == "Yes" ? true : false,
            });
        }

        private void SetupLayout()
        {
            int rowNumber = 0;

            LayoutDesigner.SetComponentsOnRow(
                dialog: this,
                row: rowNumber,
                orderedWidgets: new Widget[] { _conditionName, ConditionName });

            LayoutDesigner.SetComponentsOnRow(
                dialog: this,
                row: ++rowNumber,
                orderedWidgets: new Widget[] { _condition, Condition });

            LayoutDesigner.SetComponentsOnRow(
                dialog: this,
                row: ++rowNumber,
                orderedWidgets: new Widget[] { _visualize, Visualize });

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
