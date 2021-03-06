﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Prover.GUI.Common.Controls
{
    /// <summary>
    ///     Interaction logic for PercentageControl.xaml
    /// </summary>
    public partial class PercentageControl : UserControl
    {
        // Using a DependencyProperty as the backing store for IconSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register(nameof(IconSource), typeof(ImageSource), typeof(PercentageControl),
                new FrameworkPropertyMetadata(null));

        // Using a DependencyProperty as the backing store for IconBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconBackgroundProperty =
            DependencyProperty.Register(nameof(IconBackground), typeof(Brush), typeof(PercentageControl),
                new FrameworkPropertyMetadata(null));


        // Using a DependencyProperty as the backing store for Passed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PassedProperty =
            DependencyProperty.Register(nameof(Passed), typeof(bool), typeof(PercentageControl),
                new FrameworkPropertyMetadata(PassedPropertyChanged));

        // Using a DependencyProperty as the backing store for DisplayValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayValueProperty =
            DependencyProperty.Register(nameof(DisplayValue), typeof(decimal?), typeof(PercentageControl),
                new PropertyMetadata(0.0m));


        public PercentageControl()
        {
            InitializeComponent();
            UpdateIcon(this);
        }

        public bool Passed
        {
            get { return (bool) GetValue(PassedProperty); }
            set { SetValue(PassedProperty, value); }
        }

        public ImageSource IconSource
        {
            get { return (ImageSource) GetValue(IconSourceProperty); }
            set { SetValue(IconSourceProperty, value); }
        }

        public Brush IconBackground
        {
            get { return (Brush) GetValue(IconBackgroundProperty); }
            set { SetValue(IconBackgroundProperty, value); }
        }


        public decimal? DisplayValue
        {
            get { return (decimal?) GetValue(DisplayValueProperty); }
            set { SetValue(DisplayValueProperty, value); }
        }

        public static void PassedPropertyChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var myObj = dependencyObject as PercentageControl;
            UpdateIcon(myObj);
        }

        private static void UpdateIcon(PercentageControl myObj)
        {
            if (myObj.Passed)
            {
                myObj.IconSource = myObj.Resources["PassIcon"] as ImageSource;
                myObj.IconBackground = (Brush) myObj.Resources["PassColour"];
            }
            else
            {
                myObj.IconSource = myObj.Resources["FailIcon"] as ImageSource;
                myObj.IconBackground = (Brush) myObj.Resources["FailColour"];
            }
        }
    }
}