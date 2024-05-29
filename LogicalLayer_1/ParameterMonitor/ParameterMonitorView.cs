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
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace LogicalLayer_1.ParameterMonitor
{
    public class ParameterMonitorView : Dialog
    {
        private readonly IEngine _engine;
        private readonly Label _parameterMonitorName = new Label("Parameter Monitor Name: ") { Width = 200 };
        private readonly Label _elementName = new Label("Element Name: ") { Width = 200 };
        private readonly Label _parameterId = new Label("Parameter Name: ") { Width = 200 };
        private readonly Label _index = new Label("Index: ") { Width = 200 };
        private Element _element;
        private ParameterInfo _table;
        private List<ParameterInfo> _parameters;
        private IDms dms;

        public ParameterMonitorView(IEngine engine)
            : base(engine)
        {
            _engine = engine;
            dms = engine.GetDms();
            Title = "Parameter Monitor";
            ParameterMonitorName = new TextBox
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

        public event EventHandler<ParameterMonitorEventArgs> OnAddParameterPressed;

        public event EventHandler<CellMonitorEventArgs> OnAddCellPressed;

        public event EventHandler OnBackPressed;

        public event EventHandler OnClosePressed;

        public TextBox ParameterMonitorName { get; set; }

        public DropDown Element { get; set; }

        public DropDown Parameter { get; set; }

        public DropDown Index { get; set; }

        public Button Add { get; set; }

        public Button Back { get; set; }

        public Button Close { get; set; }

        private bool _isDiscreet { get; set; }

        private void Back_Pressed(object sender, EventArgs e)
        {
            OnBackPressed?.Invoke(sender, e);
        }

        private void OnAdd(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(ParameterMonitorName.Text))
            {
                return;
            }

            if (ParameterMonitorName.Text.Contains(" "))
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

            if (!String.IsNullOrWhiteSpace(Index.Selected) && Index.Selected != LayoutDesigner.OptionSelected)
            {
                var primaryKey = _element.FindPrimaryKey(_table.ID, Index.Selected);
                var parameter = _parameters.First(x => x.Description == Parameter.Selected);
                OnAddCellPressed?.Invoke(this, new CellMonitorEventArgs
                {
                    CellMonitorName = ParameterMonitorName.Text,
                    Element = _engine.FindElement(Element.Selected),
                    Table = parameter.ParentTable,
                    Column = _parameters.First(x => x.Description == Parameter.Selected),
                    Index = primaryKey,
                });
                return;
            }

            OnAddParameterPressed?.Invoke(this, new ParameterMonitorEventArgs
            {
                ParameterMonitorName = ParameterMonitorName.Text,
                Element = _engine.FindElement(Element.Selected),
                Parameter = _parameters.First(x => x.Description == Parameter.Selected),
                IsDiscreet = _isDiscreet,
            });
        }

        private void Element_Changed(object sender, DropDown.DropDownChangedEventArgs e)
        {
            _element = _engine.FindElement(Element.Selected);
            var protocol = _element.Protocol;
            _parameters = protocol.FilterParameters(ParameterFilterOptions.HideDefaultParameters);
            var displayedReadParams = _parameters.Where(x => !x.WriteType && !String.IsNullOrWhiteSpace(x.Description));
            Parameter.SetOptions(LayoutDesigner.GetDropdownValuesWithSelect(displayedReadParams.Select(x => x.Description).OrderBy(x => x)));
            Parameter.Selected = LayoutDesigner.OptionSelected;
        }

        private void Parameter_Changed(object sender, DropDown.DropDownChangedEventArgs e)
        {
            var selectedParameter = _parameters.First(x => x.Description == Parameter.Selected);
            _isDiscreet = selectedParameter.Discreets.Any();
            if (selectedParameter.ParentTable == null)
            {
                Index.Selected = String.Empty;
            }
            else
            {
                _table = _parameters.First(x => x.Description == Parameter.Selected).ParentTable;
                Index.SetOptions(LayoutDesigner.GetDropdownValuesWithSelect(dms.GetElement(Element.Selected).GetTable(_table.ID).GetPrimaryKeys().OrderBy(x => x)));
                Index.Selected = LayoutDesigner.OptionSelected;
            }
        }

        private void SetupLayout()
        {
            int rowNumber = 0;

            LayoutDesigner.SetComponentsOnRow(
                dialog: this,
                row: rowNumber,
                orderedWidgets: new Widget[] { _parameterMonitorName, ParameterMonitorName });

            LayoutDesigner.SetComponentsOnRow(
                dialog: this,
                row: ++rowNumber,
                orderedWidgets: new Widget[] { _elementName, Element });

            LayoutDesigner.SetComponentsOnRow(
                dialog: this,
                row: ++rowNumber,
                orderedWidgets: new Widget[] { _parameterId, Parameter });

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
