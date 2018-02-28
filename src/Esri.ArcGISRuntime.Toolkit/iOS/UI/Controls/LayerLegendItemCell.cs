﻿// /*******************************************************************************
//  * Copyright 2017 Esri
//  *
//  *  Licensed under the Apache License, Version 2.0 (the "License");
//  *  you may not use this file except in compliance with the License.
//  *  You may obtain a copy of the License at
//  *
//  *  http://www.apache.org/licenses/LICENSE-2.0
//  *
//  *   Unless required by applicable law or agreed to in writing, software
//  *   distributed under the License is distributed on an "AS IS" BASIS,
//  *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  *   See the License for the specific language governing permissions and
//  *   limitations under the License.
//  ******************************************************************************/

using Esri.ArcGISRuntime.Mapping;
using System;
using UIKit;

namespace Esri.ArcGISRuntime.Toolkit.UI.Controls
{
    internal class LayerLegendItemCell : UITableViewCell
    {
        private readonly SymbolDisplay _symbolDisplay;
        private readonly UILabel _textLabel;

        public LayerLegendItemCell(IntPtr handle) : base(handle)
        {
            SelectionStyle = UITableViewCellSelectionStyle.None;
            TranslatesAutoresizingMaskIntoConstraints = false;

            _symbolDisplay = new SymbolDisplay()
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            _textLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(UIFont.LabelFontSize),
                TextColor = UIColor.Black,
                BackgroundColor = UIColor.Clear,
                ContentMode = UIViewContentMode.Center,
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            ContentView.AddSubviews(_symbolDisplay, _textLabel);
        }

        public void Update(LegendInfo info)
        {
            _symbolDisplay.Symbol = info?.Symbol;
            _textLabel.Text = info?.Name;
        }

        private bool _constraintsUpdated = false;

        public override void UpdateConstraints()
        {
            base.UpdateConstraints();

            _symbolDisplay.SetContentCompressionResistancePriority((float)UILayoutPriority.DefaultHigh, UILayoutConstraintAxis.Vertical);

            if(_constraintsUpdated)
            {
                return;
            }

            _constraintsUpdated = true;
            var margin = ContentView.LayoutMarginsGuide;

            _symbolDisplay.LeadingAnchor.ConstraintEqualTo(margin.LeadingAnchor).Active = true;
            _symbolDisplay.TopAnchor.ConstraintEqualTo(margin.TopAnchor).Active = true;

            _textLabel.LeadingAnchor.ConstraintGreaterThanOrEqualTo(_symbolDisplay.TrailingAnchor, 2).Active = true;
            _textLabel.CenterYAnchor.ConstraintEqualTo(margin.CenterYAnchor).Active = true;
        }
    }
}
