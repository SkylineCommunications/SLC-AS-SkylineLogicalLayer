﻿using LogicalLayer_1.ParameterMonitor;
using LogicalLayer_1.Utils;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.CICD.Parsers.Common.Xml;
using Skyline.DataMiner.CICD.Parsers.Protocol.Xml;
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

namespace LogicalLayer_1.ElementAlarmMonitor
{
    public class ElementAlarmMonitorView : Dialog
    {
        private readonly IEngine _engine;
        private readonly Label _elementAlarmMonitorName = new Label("Element Alarm Monitor Name: ") { Width = 200 };
        private readonly Label _elementName = new Label("Element Name: ") { Width = 200 };
        private readonly Label _parameterName = new Label("Parameter Name: ") { Width = 200 };
        private readonly Label _index = new Label("Index: ") { Width = 200 };
        private Protocol _protocol;
        private Element _element;
        private ParameterInfo _table;
        private List<ParameterInfo> _parameters;
        private IDms dms;

        public ElementAlarmMonitorView(IEngine engine)
            : base(engine)
        {
            _engine = engine;
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
            Add.Pressed += (s, e) => OnAdd(s, e);
            Back.Pressed += Back_Pressed;
            Element.SetOptions(LayoutDesigner.GetDropdownValuesWithSelect(dms.GetElements().Select(x => x.Name).OrderBy(x => x)));
            Element.Selected = LayoutDesigner.OptionSelected;
            Element.Changed += Element_Changed;
            Parameter.Changed += Parameter_Changed;
            Close.Pressed += (s, e) => OnClosePressed?.Invoke(this, EventArgs.Empty);
            SetupLayout();
        }

        public event EventHandler<ElementAlarmMonitorEventArgs> OnAddPressed;

        public event EventHandler<ElementCellAlarmMonitorEventArgs> OnAddCellPressed;

        public event EventHandler OnBackPressed;

        public event EventHandler OnClosePressed;

        public TextBox ElementAlarmMonitorName { get; set; }

        public DropDown Element { get; set; }

        public DropDown Parameter { get; set; }

        public DropDown Index { get; set; }

        public Button Add { get; set; }

        public Button Back { get; set; }

        public Button Close { get; set; }

        private void Back_Pressed(object sender, EventArgs e)
        {
            OnBackPressed?.Invoke(sender, e);
        }

        private void OnAdd(object sender, EventArgs e)
        {
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

            if (String.IsNullOrWhiteSpace(Index.Selected) || Index.Selected == LayoutDesigner.OptionSelected)
            {
                OnAddPressed?.Invoke(this, new ElementAlarmMonitorEventArgs
                {
                    ElementAlarmMonitorName = ElementAlarmMonitorName.Text,
                    Element = _engine.FindElement(Element.Selected),
                    ElementParameter = Parameter.Selected.StartsWith("[") ? Parameter.Selected : String.Empty,
                    Parameter = _parameters.FirstOrDefault(x => x.Description == Parameter.Selected),
                    Index = Index.Selected,
                });
            }
            else
            {
                var primaryKey = _element.FindPrimaryKey(_table.ID, Index.Selected);
                var column = _parameters.FirstOrDefault(x => x.Description == Parameter.Selected);
                OnAddCellPressed?.Invoke(this, new ElementCellAlarmMonitorEventArgs
                {
                    ElementAlarmMonitorName = ElementAlarmMonitorName.Text,
                    Element = _engine.FindElement(Element.Selected),
                    ElementParameter = String.Empty,
                    Table = column.ParentTable,
                    Column = column,
                    Index = primaryKey,
                });
            }
        }

        private void Element_Changed(object sender, DropDown.DropDownChangedEventArgs e)
        {
            _element = _engine.FindElement(Element.Selected);
            var protocol = _element.Protocol;
            _parameters = protocol.FilterParameters(ParameterFilterOptions.MonitoredOnly);
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

            Parameter.SetOptions(options.OrderBy(x => x));
            Parameter.Selected = LayoutDesigner.OptionSelected;
        }

        private void Parameter_Changed(object sender, DropDown.DropDownChangedEventArgs e)
        {
            var selectedParameter = _parameters.First(x => x.Description == Parameter.Selected);
            if (selectedParameter.ParentTable == null)
            {
                Index.Selected = String.Empty;
            }
            else
            {
                _table = _parameters.First(x => x.Description == Parameter.Selected).ParentTable;
                //Index.SetOptions(LayoutDesigner.GetDropdownValuesWithSelect(dms.GetElement(Element.Selected).GetTable(selectedParameterTable.ID).GetPrimaryKeys().OrderBy(x => x)));
                Index.SetOptions(LayoutDesigner.GetDropdownValuesWithSelect(_element.GetTableDisplayKeys(_table.Name).OrderBy(x => x)));
                Index.Selected = LayoutDesigner.OptionSelected;
            }
        }

        private void SetupLayout()
        {
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

            LayoutDesigner.SetComponentsOnRow(
                dialog: this,
                row: ++rowNumber,
                orderedWidgets: new Widget[] { _index, Index });

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