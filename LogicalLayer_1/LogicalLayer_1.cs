/*
****************************************************************************
*  Copyright (c) 2024,  Skyline Communications NV  All Rights Reserved.    *
****************************************************************************

By using this script, you expressly agree with the usage terms and
conditions set out below.
This script and all related materials are protected by copyrights and
other intellectual property rights that exclusively belong
to Skyline Communications.

A user license granted for this script is strictly for personal use only.
This script may not be used in any way by anyone without the prior
written consent of Skyline Communications. Any sublicensing of this
script is forbidden.

Any modifications to this script by the user are only allowed for
personal use and within the intended purpose of the script,
and will remain the sole responsibility of the user.
Skyline Communications will not be responsible for any damages or
malfunctions whatsoever of the script resulting from a modification
or adaptation by the user.

The content of this script is confidential information.
The user hereby agrees to keep this confidential information strictly
secret and confidential and not to disclose or reveal it, in whole
or in part, directly or indirectly to any person, entity, organization
or administration without the prior written consent of
Skyline Communications.

Any inquiries can be addressed to:

	Skyline Communications NV
	Ambachtenstraat 33
	B-8870 Izegem
	Belgium
	Tel.	: +32 51 31 35 69
	Fax.	: +32 51 31 01 29
	E-mail	: info@skyline.be
	Web		: www.skyline.be
	Contact	: Ben Vandenberghe

****************************************************************************
Revision History:

DATE		VERSION		AUTHOR			COMMENTS

25/04/2024	1.0.0.1		RDM, Skyline	Initial version
****************************************************************************
*/

namespace LogicalLayer_1
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using LogicalLayer_1.ChoseObject;
    using LogicalLayer_1.Condition;
    using LogicalLayer_1.ElementAlarmMonitor;
    using LogicalLayer_1.ParameterMonitor;
    using LogicalLayer_1.ViewAlarmMonitor;
    using Newtonsoft.Json;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Core.DataMinerSystem.Automation;
    using Skyline.DataMiner.Net.Messages;
    using Skyline.DataMiner.Net.Serialization;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;
    using static System.Net.Mime.MediaTypeNames;

    /// <summary>
    /// Represents a DataMiner Automation script.
    /// engine.ShowUI();
    /// </summary>
    public class Script
    {
        private InteractiveController app;
        private IEngine engine;

        public enum ElementParameter
        {
            None = 0,
            ElementAlarmState = -1,
            ElementTimeout = -2,
        }

        /// <summary>
        /// The script entry point.
        /// </summary>
        /// <param name="engine">Link with SLAutomation process.</param>
		public void Run(IEngine engine)
        {
            try
            {
                engine.FindInteractiveClient("message test", 5, "user:" + engine.UserLoginName, AutomationScriptAttachOptions.AttachImmediately);
                app = new InteractiveController(engine);
                this.engine = engine;
                engine.SetFlag(RunTimeFlags.NoKeyCaching);
                RunSafe(engine);
            }
            catch (ScriptAbortException)
            {
                // Catch normal abort exceptions (engine.ExitFail or engine.ExitSuccess)
                throw; // Comment if it should be treated as a normal exit of the script.
            }
            catch (ScriptForceAbortException)
            {
                // Catch forced abort exceptions, caused via external maintenance messages.
                throw;
            }
            catch (ScriptTimeoutException)
            {
                // Catch timeout exceptions for when a script has been running for too long.
                throw;
            }
            catch (InteractiveUserDetachedException)
            {
                // Catch a user detaching from the interactive script by closing the window.
                // Only applicable for interactive scripts, can be removed for non-interactive scripts.
                throw;
            }
            catch (Exception e)
            {
                engine.ExitFail("Run|Something went wrong: " + e);
            }
        }

        private void RunSafe(IEngine engine)
        {
            var locationFilter = engine.GetScriptParam("Location Filter");
            switch (locationFilter.Value)
            {
                case "Parameter":
                    ParameterMonitorView parameterMonitorView = ConfigureParameterMonitorView(engine);
                    app.Run(parameterMonitorView);
                    break;

                case "Element":
                    ElementAlarmMonitorView elementAlarmMonitorView = ConfigureElementMonitorView(engine);
                    app.Run(elementAlarmMonitorView);
                    break;

                case "View":
                    ViewAlarmMonitorView viewAlarmMonitorView = ConfigureViewMonitorView(engine);
                    app.Run(viewAlarmMonitorView);
                    break;

                case "Condition":
                    ConditionView conditionView = ConfigureConditionView();
                    app.Run(conditionView);
                    break;

                default:
                    var chooseObjectView = new ChooseObjectView(engine);
                    chooseObjectView.OnParameterMonitorPressed += ChooseObjectView_OnParameterMonitorPressed;
                    chooseObjectView.OnElementAlarmMonitorPressed += ChooseObjectView_OnElementAlarmMonitorPressed;
                    chooseObjectView.OnConditionPressed += ChooseObjectView_OnConditionPressed;
                    app.Run(chooseObjectView);
                    break;
            }
        }

        private ChooseObjectView ConfigureChooseObjectView()
        {
            var chooseObjectView = new ChooseObjectView(engine);
            chooseObjectView.OnParameterMonitorPressed += ChooseObjectView_OnParameterMonitorPressed;
            chooseObjectView.OnElementAlarmMonitorPressed += ChooseObjectView_OnElementAlarmMonitorPressed;
            chooseObjectView.OnViewMonitorPressed += ChooseObjectView_OnViewMonitorPressed;
            chooseObjectView.OnConditionPressed += ChooseObjectView_OnConditionPressed;
            chooseObjectView.OnClosePressed += OnClosePressed;
            return chooseObjectView;
        }

        private ParameterMonitorView ConfigureParameterMonitorView(IEngine engine)
        {
            var parameterMonitorView = new ParameterMonitorView(engine);
            parameterMonitorView.OnAddParameterPressed += ParameterMonitorView_OnAddPressed;
            parameterMonitorView.OnAddCellPressed += ParameterMonitorView_OnAddCellPressed;
            parameterMonitorView.OnBackPressed += ParameterMonitorView_OnBackPressed;
            parameterMonitorView.OnClosePressed += OnClosePressed;
            return parameterMonitorView;
        }

        private ElementAlarmMonitorView ConfigureElementMonitorView(IEngine engine)
        {
            var elementAlarmMonitorView = new ElementAlarmMonitorView(engine);
            elementAlarmMonitorView.OnAddPressed += ElementAlarmMonitorView_OnAddPressed;
            elementAlarmMonitorView.OnAddCellPressed += ElementCellAlarmMonitorView_OnAddCellPressed;
            elementAlarmMonitorView.OnBackPressed += ElementAlarmMonitorView_OnBackPressed;
            elementAlarmMonitorView.OnClosePressed += OnClosePressed;
            return elementAlarmMonitorView;
        }

        private ViewAlarmMonitorView ConfigureViewMonitorView(IEngine engine)
        {
            var viewAlarmMonitorView = new ViewAlarmMonitorView(engine);
            viewAlarmMonitorView.OnAddPressed += ViewAlarmMonitorView_OnAddPressed;
            viewAlarmMonitorView.OnBackPressed += ElementAlarmMonitorView_OnBackPressed;
            viewAlarmMonitorView.OnClosePressed += OnClosePressed;
            return viewAlarmMonitorView;
        }

        private ConditionView ConfigureConditionView()
        {
            var conditionView = new ConditionView(engine);
            conditionView.OnAddConditionPressed += ConditionView_OnAddConditionPressed;
            conditionView.OnBackPressed += ConditionView_OnBackPressed;
            conditionView.OnClosePressed += OnClosePressed;
            return conditionView;
        }

        private void ChooseObjectView_OnParameterMonitorPressed(object sender, EventArgs e)
        {
            var parameterMonitorView = ConfigureParameterMonitorView(engine);
            app.ShowDialog(parameterMonitorView);
        }

        private void ChooseObjectView_OnElementAlarmMonitorPressed(object sender, EventArgs e)
        {
            var elementAlarmMonitorView = ConfigureElementMonitorView(engine);
            app.ShowDialog(elementAlarmMonitorView);
        }

        private void ChooseObjectView_OnViewMonitorPressed(object sender, EventArgs e)
        {
            var viewAlarmMonitorView = ConfigureViewMonitorView(engine);
            app.ShowDialog(viewAlarmMonitorView);
        }

        private void ChooseObjectView_OnConditionPressed(object sender, EventArgs e)
        {
            ConditionView conditionView = ConfigureConditionView();
            app.ShowDialog(conditionView);
        }

        private void ParameterMonitorView_OnAddCellPressed(object sender, CellMonitorEventArgs e)
        {
            var dummy = engine.GetDummy("Logical Layer");
            if (dummy == null)
            {
                return;
            }

            dummy.SetParameter(3, JsonConvert.SerializeObject(new CellMonitorModel
            {
                CellMonitorName = e.CellMonitorName,
                ElementName = e.Element.ElementName,
                ElementDmaId = e.Element.DmaId,
                ElementElementId = e.Element.ElementId,
                TableId = e.Table.ID,
                ColumnDescription = e.Column.Description,
                ColumnId = e.Column.ID,
                Index = e.Index,
            }));
        }

        private void ParameterMonitorView_OnAddPressed(object sender, ParameterMonitorEventArgs e)
        {
            var dummy = engine.GetDummy("Logical Layer");
            if (dummy == null)
            {
                return;
            }

            dummy.SetParameter(3, JsonConvert.SerializeObject(new ParameterMonitorModel
            {
                ParameterMonitorName = e.ParameterMonitorName,
                ElementName = e.Element.ElementName,
                ElementDmaId = e.Element.DmaId,
                ElementElementId = e.Element.ElementId,
                ParameterDescription = e.Parameter.Description,
                ParameterId = e.Parameter.ID,
                ParameterIsDiscreet = e.Parameter.IsDiscreet,
            }));
        }

        private void ParameterMonitorView_OnBackPressed(object sender, EventArgs e)
        {
            ChooseObjectView chooseObjectView = ConfigureChooseObjectView();
            app.ShowDialog(chooseObjectView);
        }

        private void ElementAlarmMonitorView_OnAddPressed(object sender, ElementAlarmMonitorEventArgs e)
        {
            var dummy = engine.GetDummy("Logical Layer");
            if (dummy == null)
            {
                return;
            }

            var elementAlarmMonitorModel = new ElementAlarmMonitorModel()
            {
                ElementAlarmMonitorName = e.ElementAlarmMonitorName,
                ElementName = e.Element.ElementName,
                ElementDmaId = e.Element.DmaId,
                ElementElementId = e.Element.ElementId,
                Index = e.Index,
            };

            if (e.Parameter == null)
            {
                elementAlarmMonitorModel.ParameterDescription = String.Empty;
                elementAlarmMonitorModel.ParameterId = 0;
                switch (e.ElementParameter)
                {
                    case "[Element Alarm State]":
                        elementAlarmMonitorModel.ElementParameter = ElementParameter.ElementAlarmState;
                        break;
                }
            }
            else
            {
                elementAlarmMonitorModel.ParameterDescription = e.Parameter.Description;
                elementAlarmMonitorModel.ParameterId = e.Parameter.ID;
                elementAlarmMonitorModel.ElementParameter = ElementParameter.None;
            }

            dummy.SetParameter(3, JsonConvert.SerializeObject(elementAlarmMonitorModel));
        }

        private void ElementCellAlarmMonitorView_OnAddCellPressed(object sender, ElementCellAlarmMonitorEventArgs e)
        {
            var dummy = engine.GetDummy("Logical Layer");
            if (dummy == null)
            {
                return;
            }

            var elementAlarmMonitorModel = new ElementCellAlarmMonitorModel()
            {
                CellMonitorName = e.ElementAlarmMonitorName,
                ElementName = e.Element.ElementName,
                ElementDmaId = e.Element.DmaId,
                ElementElementId = e.Element.ElementId,
                Index = e.Index,
            };

            elementAlarmMonitorModel.TableId = e.Table.ID;
            elementAlarmMonitorModel.ColumnDescription = e.Column.Description;
            elementAlarmMonitorModel.ColumnId = e.Column.ID;
            dummy.SetParameter(3, JsonConvert.SerializeObject(elementAlarmMonitorModel));
        }

        private void ElementAlarmMonitorView_OnBackPressed(object sender, EventArgs e)
        {
            ChooseObjectView chooseObjectView = ConfigureChooseObjectView();
            app.ShowDialog(chooseObjectView);
        }

        private void ViewAlarmMonitorView_OnAddPressed(object sender, ViewAlarmMonitorEventArgs e)
        {
            var dummy = engine.GetDummy("Logical Layer");
            if (dummy == null)
            {
                return;
            }

            var elementAlarmMonitorModel = new ViewAlarmMonitorModel()
            {
                ViewAlarmMonitorName = e.ViewAlarmMonitorName,
                ViewName = e.View.Name,
                ViewId = e.View.Id,
                Parameter = e.ViewParameter,
            };

            dummy.SetParameter(3, JsonConvert.SerializeObject(elementAlarmMonitorModel));
        }

        private void ViewAlarmMonitorView_OnBackPressed(object sender, EventArgs e)
        {
            ChooseObjectView chooseObjectView = ConfigureChooseObjectView();
            app.ShowDialog(chooseObjectView);
        }

        private void ConditionView_OnBackPressed(object sender, EventArgs e)
        {
            ChooseObjectView chooseObjectView = ConfigureChooseObjectView();
            app.ShowDialog(chooseObjectView);
        }

        private void ConditionView_OnAddConditionPressed(object sender, ConditionEventArgs e)
        {
            var dummy = engine.GetDummy("Logical Layer");
            if (dummy == null)
            {
                return;
            }

            dummy.SetParameter(3, JsonConvert.SerializeObject(new ConditionModel
            {
                ConditionName = e.ConditionName,
                Condition = e.Condition,
                Visualize = e.Visualize,
            }));
        }

        private void OnClosePressed(object sender, EventArgs e)
        {
            engine.ExitSuccess("End of script.");
        }
    }
}
