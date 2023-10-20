﻿using FlaUI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaUI.Core.AutomationElements.Infrastructure;
using FlaUI.Core.Patterns;
using FlaUIRecorder.Tests.Common.Patterns;

namespace FlaUIRecorder.Tests.Common
{
    public class TestPropertyLibray : IPropertyLibray
    {
        public TestPropertyLibray()
        {
            PatternAvailability = new TestAutomationElementPatternAvailabilityProperties();
            Element = new TestAutomationElementProperties();
            //Annotation = new AnnotationPatternProperties();
            //Dock = new DockPatternProperties();
            //Drag = new DragPatternProperties();
            //DropTarget = new DropTargetPatternProperties();
            //ExpandCollapse = new ExpandCollapsePatternProperties();
            //GridItem = new GridItemPatternProperties();
            //Grid = new GridPatternProperties();
            //LegacyIAccessible = new LegacyIAccessiblePatternProperties();
            //MultipleView = new MultipleViewPatternProperties();
            //RangeValue = new RangeValuePatternProperties();
            //Scroll = new ScrollPatternProperties();
            //SelectionItem = new SelectionItemPatternProperties();
            //Selection = new SelectionPatternProperties();
            //SpreadsheetItem = new SpreadsheetItemPatternProperties();
            //Styles = new StylesPatternProperties();
            //TableItem = new TableItemPatternProperties();
            //Table = new TablePatternProperties();
            //Toggle = new TogglePatternProperties();
            //Transform2 = new Transform2PatternProperties();
            //Transform = new TransformPatternProperties();
            Value = new ValuePatternProperties();
            //Window = new WindowPatternProperties();
        }

        public IAutomationElementPatternAvailabilityProperties PatternAvailability { get; }
        public IAutomationElementProperties Element { get; }
        public IAnnotationPatternProperties Annotation { get; }
        public IDockPatternProperties Dock { get; }
        public IDragPatternProperties Drag { get; }
        public IDropTargetPatternProperties DropTarget { get; }
        public IExpandCollapsePatternProperties ExpandCollapse { get; }
        public IGridItemPatternProperties GridItem { get; }
        public IGridPatternProperties Grid { get; }
        public ILegacyIAccessiblePatternProperties LegacyIAccessible { get; }
        public IMultipleViewPatternProperties MultipleView { get; }
        public IRangeValuePatternProperties RangeValue { get; }
        public IScrollPatternProperties Scroll { get; }
        public ISelectionItemPatternProperties SelectionItem { get; }
        public ISelectionPatternProperties Selection { get; }
        public ISpreadsheetItemPatternProperties SpreadsheetItem { get; }
        public IStylesPatternProperties Styles { get; }
        public ITableItemPatternProperties TableItem { get; }
        public ITablePatternProperties Table { get; }
        public ITogglePatternProperties Toggle { get; }
        public ITransform2PatternProperties Transform2 { get; }
        public ITransformPatternProperties Transform { get; }
        public IValuePatternProperties Value { get; }
        public IWindowPatternProperties Window { get; }
    }
}
