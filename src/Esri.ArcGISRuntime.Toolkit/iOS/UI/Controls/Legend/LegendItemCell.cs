﻿// /*******************************************************************************
//  * Copyright 2012-2018 Esri
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using UIKit;

namespace Esri.ArcGISRuntime.Toolkit.UI.Controls
{
    internal class LegendItemCell : UITableViewCell
    {
        private readonly UILabel _textLabel;
        private readonly LayerLegend _layerLegend;
        private readonly UITableView _listView;

        public LegendItemCell(IntPtr handle)
            : base(handle)
        {
            SelectionStyle = UITableViewCellSelectionStyle.None;
            TranslatesAutoresizingMaskIntoConstraints = false;

            _textLabel = new UILabel()
            {
                LineBreakMode = UILineBreakMode.TailTruncation,
                Font = UIFont.SystemFontOfSize(UIFont.LabelFontSize),
                TextColor = UIColor.Black,
                BackgroundColor = UIColor.Clear,
                ContentMode = UIViewContentMode.Center,
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            _layerLegend = new LayerLegend()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                ShowEntireTreeHierarchy = false
            };

            _listView = new UITableView(UIScreen.MainScreen.Bounds)
            {
                ClipsToBounds = true,
                ContentMode = UIViewContentMode.ScaleAspectFill,
                SeparatorStyle = UITableViewCellSeparatorStyle.None,
                AllowsSelection = false,
                Bounces = true,
                TranslatesAutoresizingMaskIntoConstraints = false,
                AutoresizingMask = UIViewAutoresizing.All,
                RowHeight = UITableView.AutomaticDimension,
                EstimatedRowHeight = SymbolDisplay.MaxSize,
            };
            _listView.RegisterClassForCellReuse(typeof(LegendItemCell), LegendTableSource.CellId);

            ContentView.AddSubviews(_textLabel, _layerLegend, _listView);
        }

        private void Refresh(LayerContentViewModel layerContent, string propertyName)
        {
            if (propertyName == nameof(LayerContentViewModel.Sublayers))
            {
                UpdateSublayers(layerContent);
            }
            else if (propertyName == nameof(LayerContentViewModel.DisplayLegend))
            {
                Hidden = layerContent?.DisplayLegend ?? false;
            }
        }

        private void UpdateSublayers(LayerContentViewModel layerContent)
        {
            var subLayers = layerContent?.Sublayers;
            if (subLayers == null)
            {
                return;
            }

            _listView.Source = new LegendTableSource(new List<LayerContentViewModel>(subLayers));
        }

        internal void Update(LayerContentViewModel layerContent)
        {
            if (layerContent == null)
            {
                return;
            }

            if (!layerContent.IsSublayer)
            {
                _textLabel.Font = UIFont.BoldSystemFontOfSize(UIFont.LabelFontSize);
            }

            _textLabel.Text = layerContent.LayerContent?.Name;
            _layerLegend.LayerContent = layerContent.LayerContent;
            UpdateSublayers(layerContent);

            if (layerContent is INotifyPropertyChanged)
            {
                var inpc = layerContent as INotifyPropertyChanged;
                var listener = new Internal.WeakEventListener<INotifyPropertyChanged, object, PropertyChangedEventArgs>(inpc)
                {
                    OnEventAction = (instance, source, eventArgs) =>
                    {
                        Refresh(instance as LayerContentViewModel, eventArgs.PropertyName);
                    },
                    OnDetachAction = (instance, weakEventListener) => instance.PropertyChanged -= weakEventListener.OnEvent
                };
                inpc.PropertyChanged += listener.OnEvent;
            }
        }

        private bool _constraintsUpdated = false;

        public override void UpdateConstraints()
        {
            base.UpdateConstraints();

            _layerLegend.SetContentCompressionResistancePriority((float)UILayoutPriority.DefaultHigh, UILayoutConstraintAxis.Vertical);

            if (_constraintsUpdated)
            {
                return;
            }

            _constraintsUpdated = true;
            var margin = ContentView.LayoutMarginsGuide;

            _textLabel.LeadingAnchor.ConstraintEqualTo(margin.LeadingAnchor).Active = true;
            _textLabel.TopAnchor.ConstraintEqualTo(margin.TopAnchor).Active = true;
            _layerLegend.LeadingAnchor.ConstraintEqualTo(margin.LeadingAnchor).Active = true;
            _layerLegend.TopAnchor.ConstraintEqualTo(_textLabel.BottomAnchor).Active = true;
            _layerLegend.BottomAnchor.ConstraintEqualTo(_listView.TopAnchor).Active = true;
            _listView.LeadingAnchor.ConstraintEqualTo(margin.LeadingAnchor).Active = true;
            _listView.BottomAnchor.ConstraintEqualTo(margin.BottomAnchor).Active = true;
        }
    }
}