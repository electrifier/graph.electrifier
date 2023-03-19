﻿using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using electrifier.Contracts.Services;
using electrifier.Helpers;
using electrifier.ViewModels;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.System;

namespace electrifier.Views;

// TODO: Update NavigationViewItem titles and icons in ShellPage.xaml.
public sealed partial class ShellPage : Page
{
    public ShellViewModel ViewModel
    {
        get;
    }

    public class Tab
    {
        public String Name
        {
            get; set;
        }
        public String Icon
        {
            get; set;
        }
        public ObservableCollection<Tab> Children
        {
            get; set;
        }
    }

    public ObservableCollection<Tab> ChildTabs = new();

    public string ThisComputerName
    {
        get;
    }

    internal static string GetThisComputerName()
    {
        try
        {
            return Dns.GetHostName();
        }
        catch
        {
            return Environment.MachineName;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="viewModel"></param>
    public ShellPage(ShellViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();

        ViewModel.NavigationService.Frame = NavigationFrame;
        ViewModel.NavigationViewService.Initialize(NavigationViewControl);

        // TODO: Set the title bar icon by updating /Assets/WindowIcon.ico.
        // A custom title bar is required for full window theme and Mica support.
        // https://docs.microsoft.com/windows/apps/develop/title-bar?tabs=winui3#full-customization
        App.MainWindow.ExtendsContentIntoTitleBar = true;
        App.MainWindow.SetTitleBar(AppTitleBar);
        App.MainWindow.Activated += MainWindow_Activated;

        AppTitleBarText.Text = ResourceExtensions.GetLocalized("AppDisplayName");
        ThisComputerName = $"This PC: {GetThisComputerName()}";    // TODO: i18n
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        TitleBarHelper.UpdateTitleBar(RequestedTheme);

        Debug.Assert(KeyboardAccelerators != null, nameof(KeyboardAccelerators) + " != null");
        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu));
        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoBack));
    }

    private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        var resource = args.WindowActivationState == WindowActivationState.Deactivated
            ? "WindowCaptionForegroundDisabled"
            : "WindowCaptionForeground";

        //AppTitleBarText.Foreground = (SolidColorBrush)Application.Current.Resources[resource];
    }

    private void NavigationViewControl_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
    {
        Debug.Assert(AppTitleBar != null, nameof(AppTitleBar) + " != null");

        AppTitleBar.Margin = new Thickness()
        {
            Left = sender.CompactPaneLength * (sender.DisplayMode == NavigationViewDisplayMode.Minimal ? 2 : 1),
            Top = AppTitleBar.Margin.Top,
            Right = AppTitleBar.Margin.Right,
            Bottom = AppTitleBar.Margin.Bottom
        };
    }

    private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
    {
        var keyboardAccelerator = new KeyboardAccelerator() { Key = key };

        if (modifiers.HasValue)
        {
            keyboardAccelerator.Modifiers = modifiers.Value;
        }

        keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;

        return keyboardAccelerator;
    }

    private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        var navigationService = App.GetService<INavigationService>();

        Debug.Assert(navigationService != null, nameof(navigationService) + " != null");
        var result = navigationService.GoBack();

        args.Handled = result;
    }

    private void OnItemInvoked(object sender, NavigationViewItemInvokedEventArgs e)
    {
        var clickedItem = e.InvokedItem;
        var clickedItemContainer = e.InvokedItemContainer;
    }

    private void OnItemExpanding(object sender, NavigationViewItemExpandingEventArgs e)
    {
        //var nvib = e.ExpandingItemContainer;
        //var name = "Last Expanding: " + nvib.Content.ToString();
        //ExpandingItemLabel.Text = name;
    }

    private void OnItemCollapsed(object sender, NavigationViewItemCollapsedEventArgs e)
    {
        //var nvib = e.CollapsedItemContainer;
        //var name = "Last Collapsed: " + nvib.Content;
        //CollapsedItemLabel.Text = name;
    }

    public sealed class NavigationViewItemExpandingEventArgs
    {
        public NavigationViewItemExpandingEventArgs()
        {
        }

        public object ExpandingItem
        {
            get;
        }
        public NavigationViewItemBase ExpandedItemContainer
        {
            get;
        }
        public bool IsSettingsInvoked
        {
            get;
        }
        public NavigationTransitionInfo RecommendedNavigationTransitionInfo
        {
            get;
        }
    }

    public sealed class NavigationViewItemCollapsedEventArgs
    {
        public NavigationViewItemCollapsedEventArgs()
        {
        }

        public object CollapsedItem
        {
            get;
        }
        public NavigationViewItemBase CollapsedItemContainer
        {
            get;
        }
        public bool IsSettingsInvoked
        {
            get;
        }
        public NavigationTransitionInfo RecommendedNavigationTransitionInfo
        {
            get;
        }
    }
}