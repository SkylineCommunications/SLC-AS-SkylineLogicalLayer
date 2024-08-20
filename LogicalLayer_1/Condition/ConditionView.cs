using LogicalLayer_1.ParameterMonitor;
using LogicalLayer_1.Utils;
using Newtonsoft.Json;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.ConnectorAPI.SkylineCommunications.SkylineLogicalLayer.InterAppMessages.MyMessages;
using Skyline.DataMiner.Net.Messages.SLDataGateway;
using Skyline.DataMiner.Utils.InteractiveAutomationScript;
using System;
using System.Collections.Generic;
using System.Timers;

namespace LogicalLayer_1.Condition
{
    public class ConditionView : Dialog
    {
        private readonly string _startTimeoutLabel = "Window will close in ";
        private readonly Label _conditionName = new Label("Condition Name: ") { Width = 200 };
        private readonly Label _condition = new Label("Condition: ") { Width = 200 };
        private readonly Label _visualize = new Label("Visualize: ") { Width = 200 };
        private readonly Label _correctiveActionScript = new Label("Corrective Action Script: ") { Width = 200 };
        private readonly bool _isUpdate = false;
        private DateTime _closingTime;

        public ConditionView(IEngine engine, string data, DateTime closingTime) : base(engine)
        {
            _closingTime = closingTime;
            Title = $"Condition - Will close at {_closingTime.TimeOfDay.Hours.ToString().PadLeft(2, '0')}:{_closingTime.TimeOfDay.Minutes.ToString().PadLeft(2, '0')}";
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
            AutomaticCorrection = new CheckBox
            {

            };
            CorrectiveActionScript = new TextBox
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
            Update = new Button("Update")
            {
                Width = 200,
            };
            KeepAlive = new Button("Keep Alive")
            {
                Width = 200,
            };
            Add.Pressed += Add_Pressed;
            Back.Pressed += Back_Pressed;
            Close.Pressed += (s, e) => OnClosePressed?.Invoke(this, EventArgs.Empty);
            Update.Pressed += Update_Pressed;
            KeepAlive.Pressed += KeepAliveScript;
            if (!String.IsNullOrWhiteSpace(data) && data != "New")
            {
                _isUpdate = true;
                ConditionName.IsEnabled = false;
                ConditionModel model = JsonConvert.DeserializeObject<ConditionModel>(data);
                ConditionName.Text = model.ConditionName;
                Condition.Text = model.Condition;
                Visualize.Selected = Convert.ToString(model.Visualize);
                AutomaticCorrection.IsChecked = model.AutomaticCorrection;
                CorrectiveActionScript.Text = model.CorrectiveActionScript;
            }

            SetupLayout();
        }

        public event EventHandler OnBackPressed;

        public event EventHandler<ConditionEventArgs> OnAddConditionPressed;

        public event EventHandler<ConditionEventArgs> OnUpdateConditionPressed;

        public event EventHandler OnClosePressed;

        public event EventHandler UpdateClosingTime;

        public TextBox ConditionName { get; set; }

        public TextBox Condition { get; set; }

        public DropDown Visualize { get; set; }

        public CheckBox AutomaticCorrection { get; set; }

        public TextBox CorrectiveActionScript { get; set; }

        public Button Add { get; set; }

        public Button Back { get; set; }

        public Button Close { get; set; }

        public Button Update { get; set; }

        public Button KeepAlive { get; set; }

        private void KeepAliveScript(object sender, EventArgs e)
        {
            Engine.KeepAlive();
            UpdateClosingTime.Invoke(this, EventArgs.Empty);
            _closingTime = DateTime.Now + Engine.Timeout;
            Title = $"Condition - Will close at {_closingTime.TimeOfDay.Hours.ToString().PadLeft(2, '0')}:{_closingTime.TimeOfDay.Minutes.ToString().PadLeft(2, '0')}";
            SetupLayout();
        }

        private void Back_Pressed(object sender, EventArgs e)
        {
            KeepAliveScript(sender, e);
            OnBackPressed?.Invoke(sender, e);
        }

        private void Add_Pressed(object sender, EventArgs e)
        {
            KeepAliveScript(sender, e);
            OnAddConditionPressed?.Invoke(sender, new ConditionEventArgs
            {
                ConditionName = ConditionName.Text,
                Condition = Condition.Text,
                Visualize = Visualize.Selected == "Yes" ? true : false,
                AutomaticCorrection = AutomaticCorrection.IsChecked,
                CorrectiveActionScript = CorrectiveActionScript.Text,
            });
        }

        private void Update_Pressed(object sender, EventArgs e)
        {
            KeepAliveScript(sender, e);
            OnUpdateConditionPressed?.Invoke(sender, new ConditionEventArgs
            {
                ConditionName = ConditionName.Text,
                Condition = Condition.Text,
                Visualize = Visualize.Selected == "Yes" ? true : false,
                AutomaticCorrection = AutomaticCorrection.IsChecked,
                CorrectiveActionScript = CorrectiveActionScript.Text,
            });
        }

        private void SetupLayout()
        {
            Clear();
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
                orderedWidgets: new Widget[] { _correctiveActionScript, CorrectiveActionScript });

            if (_isUpdate)
            {
                LayoutDesigner.SetComponentsOnRow(
                    dialog: this,
                    row: ++rowNumber,
                    orderedWidgets: new Widget[] { Back, Update });
            }
            else
            {
                LayoutDesigner.SetComponentsOnRow(
                    dialog: this,
                    row: ++rowNumber,
                    orderedWidgets: new Widget[] { Back, Add });
            }

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
