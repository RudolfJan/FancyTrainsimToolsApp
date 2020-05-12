using FancyTrainsimTools.Desktop.Views;
using Logging.Library;
using Microsoft.Extensions.DependencyInjection;
using Mvvm.Library;
using System;
using System.Windows;
using System.Windows.Input;

namespace FancyTrainsimTools.Desktop.ViewModels
  {
  public class ShellViewModel : BindableBase, IShellViewModel
    {
    private readonly IServiceProvider _serviceProvider;
    public ICommand ExitCommand { get; } 
    public ICommand AboutCommand { get; } 
    public ICommand ShowLoggingCommand { get; }
    public ICommand GameAssetsCommand { get; }

 public ShellViewModel()
      {
      ExitCommand = new RelayCommand(Exit);
      AboutCommand = new RelayCommand(ShowAbout);
      ShowLoggingCommand= new RelayCommand(ShowLogging);
      GameAssetsCommand= new RelayCommand(ShowGameAssets);
      _serviceProvider = App.serviceProvider;
      }

    public void Exit()
      {
      Application.Current.Shutdown();
      }

    public void ShowGameAssets()
      {
      Log.Trace("Called GameAssetsView");
      var gameAssetsView = _serviceProvider.GetService<GameAssetsView>();
      gameAssetsView.Show();
      }

    public void ShowAbout()
      {
      Log.Trace("Called AboutView using a very long text here to see if wrapping actually works, so I will be happy");
      var aboutView = _serviceProvider.GetService<AboutView>();
      aboutView.Show();
      }

    public void ShowLogging()
      {
      Log.Trace("Called loggingView");
      var loggingView = _serviceProvider.GetService<LoggingView>();
      loggingView.Show();
      }


    }
  }
