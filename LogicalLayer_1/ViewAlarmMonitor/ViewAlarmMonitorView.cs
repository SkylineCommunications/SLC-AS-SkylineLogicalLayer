using LogicalLayer_1.ElementAlarmMonitor;
using LogicalLayer_1.Utils;
using Newtonsoft.Json;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.ConnectorAPI.SkylineCommunications.SkylineLogicalLayer.InterAppMessages.MyMessages;
using Skyline.DataMiner.Core.DataMinerSystem.Automation;
using Skyline.DataMiner.Core.DataMinerSystem.Common;
using Skyline.DataMiner.Net.Messages.SLDataGateway;
using Skyline.DataMiner.Utils.InteractiveAutomationScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
namespace LogicalLayer_1.ViewAlarmMonitor
{
    public class ViewAlarmMonitorView : Dialog
    {
        private readonly Label _viewAlarmMonitorName = new Label("View Alarm Monitor Name: ") { Width = 200 };
        private readonly Label _viewName = new Label("View Name: ") { Width = 200 };
        private readonly Label _parameterName = new Label("Parameter Name: ") { Width = 200 };
        private IDms _dms;
        private readonly bool _isUpdate;
        private string _startTimeoutLabel = "Window will close in ";
        private Label _timeout = new Label() { Width = 200 };
        private DateTime _closingTime;
        private Timer _timer;

        public ViewAlarmMonitorView(IEngine engine, string data, DateTime closingTime) : base(engine)
        {
            _closingTime = closingTime;
            _timer = new Timer(20000);
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
            _timeout.Text = _startTimeoutLabel + closingTime.Subtract(DateTime.Now).TotalMinutes.ToString("F0") + " min";
            _dms = engine.GetDms();
            Title = "View Alarm Monitor";
            ViewAlarmMonitorName = new TextBox
            {
                Width = 200,
                Height = 20,
            };
            View = new DropDown
            {
                IsDisplayFilterShown = true,
                Width = 200,
            };
            Parameter = new DropDown
            {
                IsDisplayFilterShown = true,
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
            ViewAlarmMonitorName.Changed += KeepAlive;
            View.Changed += KeepAlive;
            Parameter.Changed += KeepAlive;
            Add.Pressed += (s, e) => OnAdd(s, e);
            Back.Pressed += Back_Pressed;
            Update.Pressed += Update_Pressed;
            View.SetOptions(LayoutDesigner.GetDropdownValuesWithSelect(_dms.GetViews().Select(x => x.Name).OrderBy(x => x)));
            View.Selected = LayoutDesigner.OptionSelected;
            View.Changed += View_Changed;
            Close.Pressed += (s, e) => OnClosePressed?.Invoke(this, EventArgs.Empty);
            if (!String.IsNullOrWhiteSpace(data) && data != "New")
            {
                ViewAlarmMonitorName.IsEnabled = false;
                var model = JsonConvert.DeserializeObject<ViewAlarmMonitorModel>(data);
                ViewAlarmMonitorName.Text = model.ViewAlarmMonitorName;
                View.Selected = model.ViewName;
                PopulateParameterViewDropdown();
                Parameter.Selected = model.Parameter;
                _isUpdate = true;
            }

            SetupLayout();
        }

        public event EventHandler<ViewAlarmMonitorEventArgs> OnAddPressed;

        public event EventHandler<ViewAlarmMonitorEventArgs> OnUpdatePressed;

        public event EventHandler OnBackPressed;

        public event EventHandler OnClosePressed;

        public event EventHandler UpdateClosingTime;

        public TextBox ViewAlarmMonitorName { get; set; }

        public DropDown View { get; set; }

        public DropDown Parameter { get; set; }

        public TextBox Index { get; set; }

        public Button Add { get; set; }

        public Button Back { get; set; }

        public Button Close { get; set; }

        public Button Update { get; set; }

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

        private void Back_Pressed(object sender, EventArgs e)
        {
            _timer.Stop();
            Engine.KeepAlive();
            UpdateClosingTime.Invoke(this, EventArgs.Empty);
            OnBackPressed?.Invoke(sender, e);
        }

        private void OnAdd(object sender, EventArgs e)
        {
            Engine.KeepAlive();
            UpdateClosingTime.Invoke(this, EventArgs.Empty);
            if (String.IsNullOrWhiteSpace(ViewAlarmMonitorName.Text))
            {
                return;
            }

            if (View.Selected == LayoutDesigner.OptionSelected)
            {
                return;
            }

            if (Parameter.Selected == LayoutDesigner.OptionSelected)
            {
                return;
            }

            OnAddPressed?.Invoke(this, new ViewAlarmMonitorEventArgs
            {
                ViewAlarmMonitorName = ViewAlarmMonitorName.Text,
                View = _dms.GetViews().First(x => x.Name == View.Selected),
                ViewParameter = "[View Alarm State]",
            });
        }

        private void Update_Pressed(object sender, EventArgs e)
        {
            Engine.KeepAlive();
            UpdateClosingTime.Invoke(this, EventArgs.Empty);
            if (String.IsNullOrWhiteSpace(ViewAlarmMonitorName.Text))
            {
                return;
            }

            if (View.Selected == LayoutDesigner.OptionSelected)
            {
                return;
            }

            if (Parameter.Selected == LayoutDesigner.OptionSelected)
            {
                return;
            }

            OnUpdatePressed?.Invoke(this, new ViewAlarmMonitorEventArgs
            {
                ViewAlarmMonitorName = ViewAlarmMonitorName.Text,
                View = _dms.GetViews().First(x => x.Name == View.Selected),
                ViewParameter = "[View Alarm State]",
            });
        }

        private void View_Changed(object sender, DropDown.DropDownChangedEventArgs e)
        {
            PopulateParameterViewDropdown();
            Parameter.Selected = LayoutDesigner.OptionSelected;
        }

        private void PopulateParameterViewDropdown()
        {
            List<string> options = new List<string>
            {
                "[View Alarm State]",
                LayoutDesigner.OptionSelected
            };
            Parameter.SetOptions(options.OrderBy(x => x));
        }

        private void SetupLayout()
        {
            Clear();
            int rowNumber = 0;

            LayoutDesigner.SetComponentsOnRow(
                dialog: this,
                row: rowNumber,
                orderedWidgets: new Widget[] { _viewAlarmMonitorName, ViewAlarmMonitorName });

            LayoutDesigner.SetComponentsOnRow(
                dialog: this,
                row: ++rowNumber,
                orderedWidgets: new Widget[] { _viewName, View });

            /*LayoutDesigner.SetComponentsOnRow(
                dialog: this,
                row: ++rowNumber,
                orderedWidgets: new Widget[] { _parameterName, Parameter });*/

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
                orderedWidgets: new Widget[] { _timeout });

            Show(false);
        }
    }
}
