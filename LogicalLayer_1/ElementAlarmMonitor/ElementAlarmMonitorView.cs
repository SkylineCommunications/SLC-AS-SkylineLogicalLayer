using LogicalLayer_1.ParameterMonitor;
using LogicalLayer_1.Utils;
using Newtonsoft.Json;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.ConnectorAPI.SkylineCommunications.SkylineLogicalLayer.InterAppMessages.MyMessages;
using Skyline.DataMiner.Core.DataMinerSystem.Automation;
using Skyline.DataMiner.Core.DataMinerSystem.Common;
using Skyline.DataMiner.Net.Messages;
using Skyline.DataMiner.Utils.InteractiveAutomationScript;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace LogicalLayer_1.ElementAlarmMonitor
{
    public class ElementAlarmMonitorView : Dialog
    {
        private readonly Label _elementAlarmMonitorName = new Label("Element Alarm Monitor Name: ") { Width = 200 };
        private readonly Label _elementName = new Label("Element Name: ") { Width = 200 };
        private readonly Label _parameterName = new Label("Parameter Name: ") { Width = 200 };
        private readonly Label _index = new Label("Index: ") { Width = 200 };
        private readonly string _startTimeoutLabel = "Window will close in ";
        private readonly Label _timeout = new Label() { Width = 200 };
        private readonly bool _isUpdate;
        private readonly Timer _timer;
        private DateTime _closingTime;
        private Element _element;
        private ParameterInfo _table;
        private List<ParameterInfo> _parameters;
        private IDms dms;

        public ElementAlarmMonitorView(IEngine engine, string data, DateTime closingTime)
            : base(engine)
        {
            _closingTime = closingTime;
            _timer = new Timer(20000);
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
            _timeout.Text = _startTimeoutLabel + closingTime.Subtract(DateTime.Now).TotalMinutes.ToString("F0") + " min";
            dms = engine.GetDms();
            Title = "Element Alarm Monitor";
            ElementAlarmMonitorName = new TextBox
            {
                Width = 200,
                Height = 20,
            };
            Element = new DropDown
            {
                IsDisplayFilterShown = true,
                Width = 200,
            };
            Parameter = new DropDown
            {
                IsDisplayFilterShown = true,
                Width = 200,
            };
            Index = new DropDown
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
            ElementAlarmMonitorName.Changed += KeepAlive;
            Element.Changed += KeepAlive;
            Parameter.Changed += KeepAlive;
            Index.Changed += KeepAlive;
            Add.Pressed += (s, e) => OnAdd(s, e);
            Back.Pressed += Back_Pressed;
            Update.Pressed += Update_Pressed;
            Element.SetOptions(LayoutDesigner.GetDropdownValuesWithSelect(dms.GetElements().Select(x => x.Name).OrderBy(x => x)));
            Element.Selected = LayoutDesigner.OptionSelected;
            Element.Changed += Element_Changed;
            Parameter.Changed += Parameter_Changed;
            Close.Pressed += (s, e) => OnClosePressed?.Invoke(this, EventArgs.Empty);
            if (!String.IsNullOrWhiteSpace(data) && data != "New")
            {
                _isUpdate = true;
                ElementAlarmMonitorName.IsEnabled = false;
                if (data.Contains("ElementAlarmMonitorModel"))
                {
                    ElementAlarmMonitorModel model = JsonConvert.DeserializeObject<ElementAlarmMonitorModel>(data);
                    ElementAlarmMonitorName.Text = model.ElementAlarmMonitorName;
                    Element.Selected = model.ElementName;
                    GetParameters(model.ElementName);
                    var options = FillOptions();
                    Parameter.SetOptions(LayoutDesigner.GetDropdownValuesWithSelect(options.OrderBy(x => x)));
                    Parameter.Selected = model.ParameterDescription;
                }
                else
                {
                    ElementCellAlarmMonitorModel model = JsonConvert.DeserializeObject<ElementCellAlarmMonitorModel>(data);
                    ElementAlarmMonitorName.Text = model.CellMonitorName;
                    Element.Selected = model.ElementName;
                    GetParameters(model.ElementName);
                    var options = FillOptions();
                    Parameter.SetOptions(LayoutDesigner.GetDropdownValuesWithSelect(options.OrderBy(x => x)));
                    Parameter.Selected = model.ColumnDescription;
                    _table = _parameters.First(x => x.Description == Parameter.Selected).ParentTable;
                    Index.SetOptions(LayoutDesigner.GetDropdownValuesWithSelect(dms.GetElement(Element.Selected).GetTable(_table.ID).GetDisplayKeys().OrderBy(x => x)));
                    Index.Selected = model.DisplayKey;
                }
            }

            SetupLayout();
        }

        public event EventHandler<ElementAlarmMonitorEventArgs> OnAddPressed;

        public event EventHandler<ElementAlarmMonitorEventArgs> OnUpdatePressed;

        public event EventHandler<ElementCellAlarmMonitorEventArgs> OnAddCellPressed;

        public event EventHandler<ElementCellAlarmMonitorEventArgs> OnUpdateCellPressed;

        public event EventHandler OnBackPressed;

        public event EventHandler OnClosePressed;

        public event EventHandler UpdateClosingTime;

        public TextBox ElementAlarmMonitorName { get; set; }

        public DropDown Element { get; set; }

        public DropDown Parameter { get; set; }

        public DropDown Index { get; set; }

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
            if (String.IsNullOrWhiteSpace(ElementAlarmMonitorName.Text))
            {
                return;
            }

            if (Element.Selected == LayoutDesigner.OptionSelected)
            {
                return;
            }

            if (Parameter.Selected == LayoutDesigner.OptionSelected)
            {
                return;
            }

            if (_table == null)
            {
                OnAddPressed?.Invoke(this, new ElementAlarmMonitorEventArgs
                {
                    ElementAlarmMonitorName = ElementAlarmMonitorName.Text,
                    Element = Engine.FindElement(Element.Selected),
                    ElementParameter = Parameter.Selected.StartsWith("[") ? Parameter.Selected : String.Empty,
                    Parameter = _parameters.FirstOrDefault(x => x.Description == Parameter.Selected),
                    Index = Index.Selected,
                });
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(Index.Selected) && Index.Selected != LayoutDesigner.OptionSelected)
                {
                    var primaryKey = _element.FindPrimaryKey(_table.ID, Index.Selected);
                    var column = _parameters.FirstOrDefault(x => x.Description == Parameter.Selected);
                    OnAddCellPressed?.Invoke(this, new ElementCellAlarmMonitorEventArgs
                    {
                        ElementAlarmMonitorName = ElementAlarmMonitorName.Text,
                        Element = Engine.FindElement(Element.Selected),
                        ElementParameter = String.Empty,
                        Table = column.ParentTable,
                        Column = column,
                        Index = primaryKey,
                    });
                }
            }
        }

        private void Update_Pressed(object sender, EventArgs e)
        {
            Engine.KeepAlive();
            UpdateClosingTime.Invoke(this, EventArgs.Empty);
            if (String.IsNullOrWhiteSpace(ElementAlarmMonitorName.Text))
            {
                return;
            }

            if (Element.Selected == LayoutDesigner.OptionSelected)
            {
                return;
            }

            if (Parameter.Selected == LayoutDesigner.OptionSelected)
            {
                return;
            }

            if (_table == null)
            {
                OnUpdatePressed?.Invoke(this, new ElementAlarmMonitorEventArgs
                {
                    ElementAlarmMonitorName = ElementAlarmMonitorName.Text,
                    Element = Engine.FindElement(Element.Selected),
                    ElementParameter = Parameter.Selected.StartsWith("[") ? Parameter.Selected : String.Empty,
                    Parameter = _parameters.FirstOrDefault(x => x.Description == Parameter.Selected),
                    Index = Index.Selected,
                });
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(Index.Selected) && Index.Selected != LayoutDesigner.OptionSelected)
                {
                    var primaryKey = _element.FindPrimaryKey(_table.ID, Index.Selected);
                    var column = _parameters.FirstOrDefault(x => x.Description == Parameter.Selected);
                    OnUpdateCellPressed?.Invoke(this, new ElementCellAlarmMonitorEventArgs
                    {
                        ElementAlarmMonitorName = ElementAlarmMonitorName.Text,
                        Element = Engine.FindElement(Element.Selected),
                        ElementParameter = String.Empty,
                        Table = column.ParentTable,
                        Column = column,
                        Index = primaryKey,
                    });
                }
            }
        }

        private void Element_Changed(object sender, DropDown.DropDownChangedEventArgs e)
        {
            GetParameters(e.Selected);
            var options = FillOptions();

            Parameter.SetOptions(options.OrderBy(x => x));
            Parameter.Selected = LayoutDesigner.OptionSelected;
        }

        private List<string> FillOptions()
        {
            List<string> options = new List<string>();
            if (_parameters.Count > 0)
            {
                options = LayoutDesigner.GetDropdownValuesWithSelect(_parameters.Select(x => x.Description).OrderBy(x => x)).ToList();
                options.Add("[Element Alarm State]");
            }
            else
            {
                options.Add(LayoutDesigner.OptionSelected);
                options.Add("No Alarm Template Assigned");
            }

            return options;
        }

        private void Parameter_Changed(object sender, DropDown.DropDownChangedEventArgs e)
        {
            if (Parameter.Selected == "[Element Alarm State]")
            {
                _table = null;
                Index.Selected = String.Empty;
                SetupLayout();
                return;
            }

            var selectedParameter = _parameters.First(x => x.Description == Parameter.Selected);
            if (selectedParameter.ParentTable == null)
            {
                _table = null;
                Index.Selected = String.Empty;
            }
            else
            {
                _table = _parameters.First(x => x.Description == Parameter.Selected).ParentTable;
                Index.SetOptions(LayoutDesigner.GetDropdownValuesWithSelect(_element.GetTableDisplayKeys(_table.Name).OrderBy(x => x)));
                Index.Selected = LayoutDesigner.OptionSelected;
            }

            SetupLayout();
        }

        private void GetParameters(string elementName)
        {
            _element = Engine.FindElement(elementName);
            var protocol = _element.Protocol;
            _parameters = protocol.FilterParameters(ParameterFilterOptions.MonitoredOnly);
        }

        private void SetupLayout()
        {
            Clear();
            int rowNumber = 0;

            LayoutDesigner.SetComponentsOnRow(
                dialog: this,
                row: rowNumber,
                orderedWidgets: new Widget[] { _elementAlarmMonitorName, ElementAlarmMonitorName });

            LayoutDesigner.SetComponentsOnRow(
                dialog: this,
                row: ++rowNumber,
                orderedWidgets: new Widget[] { _elementName, Element });

            LayoutDesigner.SetComponentsOnRow(
                dialog: this,
                row: ++rowNumber,
                orderedWidgets: new Widget[] { _parameterName, Parameter });

            if (_table != null)
            {
                LayoutDesigner.SetComponentsOnRow(
                    dialog: this,
                    row: ++rowNumber,
                    orderedWidgets: new Widget[] { _index, Index });
            }

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
