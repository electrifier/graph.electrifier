﻿using System.Diagnostics;
using electrifier.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace electrifier.Views;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed partial class WorkbenchPage : Page
{
#if DEBUG
    public InfoBarSeverity InfoBarSeverity => InfoBarSeverity.Warning;
#else
    public InfoBarSeverity InfoBarSeverity => InfoBarSeverity.Warning;
#endif

    /// <summary>
    /// 
    /// </summary>
    public WorkbenchViewModel ViewModel
    {
        get;
    }

    /// <summary>
    /// 
    /// </summary>
    public WorkbenchPage()
    {
        ViewModel = App.GetService<WorkbenchViewModel>();
        InitializeComponent();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}
