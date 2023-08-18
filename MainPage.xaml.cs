﻿using EldenTracker.NavigationPages;
using EldenTracker.Resources.Map;
using EldenTracker.Resources.PointsOfInterest;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace EldenTracker
{
    /// <summary>
    /// Empty page, could be used in inner or Frame switch.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Frame _contentFrame; // Add this field to your MainPage class

        private NavigationView LeftPanelNavigationView;

        private bool isNavigationPanelCreated = false;

        public ObservableCollection<PointOfInterest> PointsOfInterest { get; } = new ObservableCollection<PointOfInterest>();
        private Map Map { get; set; }
        public PointOfInterest SelectedPOI { get; set; } // Create the SelectedPOI property


        public MainPage()
        {
            InitializeComponent();

            _contentFrame = Window.Current.Content as Frame;// Create the Frame instance here or assign the reference from wherever you create it dynamically

            CreateComponents();
            LinkEvents();

            PointsOfInterest.Add(new PointOfInterest(new Point(2000, 2000)));
            PointsOfInterest.Add(new PointOfInterest(new Point(2500, 2500)));
            PointsOfInterest.Add(new PointOfInterest(new Point(3000, 3000)));
            PointsOfInterest.Add(new PointOfInterest(new Point(3500, 3500)));
            PointsOfInterest.Add(new PointOfInterest(new Point(4000, 4000)));
        }


        /// <summary>
        /// Initialize all needed class Instances
        /// </summary>
        private void CreateComponents()
        {
            Map = new Map(ScrollViewer, MapImage, LeftPanelNavigationView);
        }

        private void LinkEvents()
        {
            Map.MapEventHandlerLink(MapImage);

            MapImage.RightTapped += ScrollViewer_RightTapped;
        }


        /// <summary>
        /// Creates custom PoI (PointOfInterest) on the place you RMB'ed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollViewer_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            Point position = e.GetPosition(MapImage);

            double xCoordinate = position.X - 16;
            double yCoordinate = position.Y - 16;

            PointsOfInterest.Add(new PointOfInterest(new Point(xCoordinate, yCoordinate), PointOfInterestType.Custom));
        }

        private async void PointOfInterestControl_PointOfInterestClicked(object sender, PointOfInterest e)
        {
            var contentStackPanel = new StackPanel();

            var image = new Image
            {
                Source = new BitmapImage(new Uri(e.ImageSource.ToString())),
                Width = 100, // Set appropriate width for your image
                Height = 100 // Set appropriate height for your image
            };

            var caption1 = new TextBlock
            {
                Text = "Caption 1",
                Foreground = new SolidColorBrush(Colors.White),
                Margin = new Thickness(0, 0, 0, 10) // Add margin at the bottom
            };

            var checkBox = new CheckBox
            {
                Content = "Checkbox Caption",
                Foreground = new SolidColorBrush(Colors.White),
                Margin = new Thickness(0, 10, 0, 10) // Add margin at the top and bottom
            };

            var buttonStackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0) // Add margin at the top
            };

            var closeButton1 = new Button
            {
                Content = "Close",
                Foreground = new SolidColorBrush(Colors.White),
                Width = 0.4 * 300, // 40% of the desired width (300 is just a placeholder)
                Margin = new Thickness(0.05 * 300, 0, 0.025 * 300, 0) // 5% space on the left, 2.5% on the right
            };

            var closeButton2 = new Button
            {
                Content = "Close",
                Foreground = new SolidColorBrush(Colors.White),
                Width = 0.4 * 300, // 40% of the desired width (300 is just a placeholder)
                Margin = new Thickness(0.025 * 300, 0, 0.05 * 300, 0) // 2.5% space on the left, 5% on the right
            };

            buttonStackPanel.Children.Add(closeButton1);
            buttonStackPanel.Children.Add(closeButton2);

            contentStackPanel.Children.Add(image);
            contentStackPanel.Children.Add(caption1);
            contentStackPanel.Children.Add(checkBox);
            contentStackPanel.Children.Add(buttonStackPanel);

            var contentDialog = new ContentDialog
            {
                Title = "Point of Interest",
                Content = contentStackPanel,
                CloseButtonText = string.Empty // Hide the default Close button
            };

            closeButton1.Click += (s, args) => contentDialog.Hide();
            closeButton2.Click += (s, args) => contentDialog.Hide();

            await contentDialog.ShowAsync();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isNavigationPanelCreated)
            {
                isNavigationPanelCreated = true;
                CreateNavigationPanel(_contentFrame);
            }

            LeftPanelNavigationView.IsPaneOpen = false;
        }

        private void LeftPanelNavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                // Handle settings invocation if needed
                return;
            }

            NavigationViewItem Page1NavItem = sender.MenuItems[0] as NavigationViewItem;
            NavigationViewItem Page2NavItem = sender.MenuItems[1] as NavigationViewItem;

            if (_contentFrame != null && Page1NavItem != null && Page2NavItem != null)
            {
                if (args.InvokedItemContainer == Page1NavItem && _contentFrame.CurrentSourcePageType != typeof(Page1))
                {
                    // Navigate to Page1 if it's not already the current page
                    _contentFrame.Navigate(typeof(Page1));
                }
                else if (args.InvokedItemContainer == Page2NavItem && _contentFrame.CurrentSourcePageType != typeof(MainPage))
                {
                    // Reset the ContentFrame to its original state
                    _contentFrame.Navigate(typeof(MainPage));
                    _contentFrame.BackStack.Clear(); // Clear back stack to remove any previous navigation history
                }
            }
        }

        private void CreateNavigationPanel(Frame contentFrame)
        {
            LeftPanelNavigationView = new NavigationView
            {
                Name = "LeftPanelNavigationView",
                Style = (Style)Application.Current.Resources["NavigationViewWithoutAnimation"], // Use Application.Current.Resources to access the styles
                PaneDisplayMode = NavigationViewPaneDisplayMode.LeftCompact
            };

            LeftPanelNavigationView.ItemInvoked += LeftPanelNavigationView_ItemInvoked;

            NavigationViewItem Page1NavItem = new NavigationViewItem
            {
                Name = "Page1NavItem",
                Content = "Wiki",
                Icon = new BitmapIcon { UriSource = new Uri("ms-appx:///Assets/wiki.png") }
            };

            NavigationViewItem Page2NavItem = new NavigationViewItem
            {
                Name = "Page2NavItem",
                Content = "Open map",
                Icon = new BitmapIcon { UriSource = new Uri("ms-appx:///Assets/cross.png") }
            };

            Page1NavItem.Tapped += (sender, args) =>
            {
                contentFrame.Navigate(typeof(Page1));
            };

            LeftPanelNavigationView.MenuItems.Add(Page1NavItem);
            LeftPanelNavigationView.MenuItems.Add(Page2NavItem);

            // Add the dynamically created NavigationView to the Grid
            NavigationPanelGrid.Children.Add(LeftPanelNavigationView);
        }
    }
}
