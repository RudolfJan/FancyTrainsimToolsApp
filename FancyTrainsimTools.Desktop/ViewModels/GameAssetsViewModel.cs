using Assets.Library.Logic;
using Assets.Library.Models;
using FancyTrainsimTools.Desktop.Models;
using Microsoft.Extensions.DependencyInjection;
using Mvvm.Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;

namespace FancyTrainsimTools.Desktop.ViewModels
  {
  public class GameAssetsViewModel: BindableBase
    {
    public GameAssetsModel GameAssets { get; set; } = new GameAssetsModel();
    public ICommand SetProductFilterCommand { get; }
    public ICommand SetAssetFilterCommand { get; }
    public ICommand GetInGameDirTreeCommand { get; }
    public ICommand CancelGetInGameDirCommand { get; }
    public ICommand CopyToArchiveCommand { get; }
    public ICommand GetArchiveDirTreeCommand { get; }
    public ICommand CancelArchiveDirTreeCommand { get; }
    public ICommand CopyToGameCommand { get; }

    public GameAssetsViewModel()
      {
      // ProviderProduct initialization
      SetProductFilterCommand = new RelayCommand(SetProductFilter);
      SetAssetFilterCommand = new RelayCommand(SetAssetFilter);
      GetInGameDirTreeCommand = new RelayCommand(GetInGameDirTree);
      CancelGetInGameDirCommand = new RelayCommand(CancelGetInGameDir);
      CopyToArchiveCommand = new RelayCommand<object>(CopyToArchive,CanCopy);
      GetArchiveDirTreeCommand = new RelayCommand(GetArchiveDirTree);
      CancelArchiveDirTreeCommand = new RelayCommand(CancelArchiveDirTree);
      CopyToGameCommand = new RelayCommand<object>(CopyToGame,CanCopy);
      var serviceProvider = App.serviceProvider;
      GameAssets.ProviderProductFilter = serviceProvider.GetService<ProviderProductFilterModel>();
      GameAssets.ProviderProductList = ProviderProductCollectionDataAccess.ReadAllProviderProductsFromDatabase();
      GameAssets.FilteredProviderProductList =
        ProviderProductCollectionDataAccess.ApplyProductsFilter(GameAssets.ProviderProductList,
          GameAssets.ProviderProductFilter);

// BluePrint initialization
      GameAssets.BluePrintFilter = serviceProvider.GetService<BluePrintFilterModel>();

      GameAssets.BluePrintList = AssetCollectionDataAccess.ReadAllAssetsFromDatabase();
      GameAssets.FilteredBluePrintList =
        AssetCollectionDataAccess.ApplyAssetsFilter(GameAssets.BluePrintList,
          GameAssets.BluePrintFilter);
      }

    private void SetProductFilter()
      {
      GameAssets.FilteredProviderProductList =
        ProviderProductCollectionDataAccess.ApplyProductsFilter(GameAssets.ProviderProductList,
          GameAssets.ProviderProductFilter);
        
      // Fore some reason, no notification is generated if this changes, so do it manually in this case
      OnPropertyChanged("GameAssets");
      }

    private void SetAssetFilter()
      {
      GameAssets.FilteredBluePrintList = AssetCollectionDataAccess.ApplyAssetsFilter(GameAssets.BluePrintList,
        GameAssets.BluePrintFilter);      // Fore some reason, no notification is generated if this changes, so do it manually in this case
      OnPropertyChanged("GameAssets");
      }

    private void GetInGameDirTree()
      {
      //TODO may move most of this to the library.
      ProviderProductCollectionDataAccess.SaveAllProviderProducts(Settings.GameAssetFolder,true,false);
      GameAssets.ProviderProductList = ProviderProductCollectionDataAccess.ReadAllProviderProductsFromDatabase();
      GameAssets.FilteredProviderProductList =
        ProviderProductCollectionDataAccess.ApplyProductsFilter(GameAssets.ProviderProductList,
          GameAssets.ProviderProductFilter);
      AssetCollectionDataAccess.SaveAllUnpackedAssets(Settings.GameAssetFolder,true,false);
      AssetCollectionDataAccess.SaveAllPackedAssets(Settings.GameAssetFolder,true,false);
      GameAssets.BluePrintList = AssetCollectionDataAccess.ReadAllAssetsFromDatabase();
      GameAssets.FilteredBluePrintList =
        AssetCollectionDataAccess.ApplyAssetsFilter(GameAssets.BluePrintList,
          GameAssets.BluePrintFilter);
      OnPropertyChanged("GameAssets");
      }


    private void CancelGetInGameDir()
      {
      throw new NotImplementedException();
      }

    private bool CanCopy(object arg)
      {
      // arg is in this function the SelectedItems object from a list like class 
      // This is tricky code. arg can be null and in that case you cannot determine its type. This results into an exception
      // if arg is not null, you must inspect the number of arguments in te list.
      // Note: de debugger will give a null reference exception anyway. You should ignore that and press continue during debugging
      try
        {
        return ((IList) arg)?.Count > 0;
        }
      catch
        {
        return false;
        }
      }

    private void CopyToArchive(object arg)
      {
      List<ProviderProductModel> SelectedProviderProducts = (List<ProviderProductModel>) arg; 
      ProviderProductCollectionDataAccess.CopyAssets(SelectedProviderProducts, false,
        Settings.GameAssetFolder, Settings.ArchiveAssetFolder);
      }
    private void CopyToGame(object arg)
      {
      List<ProviderProductModel> SelectedProviderProducts = (List<ProviderProductModel>) arg; 
      ProviderProductCollectionDataAccess.CopyAssets(SelectedProviderProducts, true,
        Settings.GameAssetFolder, Settings.ArchiveAssetFolder);
      }

    private void GetArchiveDirTree()
      {
      ProviderProductCollectionDataAccess.SaveAllProviderProducts(Settings.ArchiveAssetFolder,false,true);
      GameAssets.ProviderProductList = ProviderProductCollectionDataAccess.ReadAllProviderProductsFromDatabase();
      GameAssets.FilteredProviderProductList =
        ProviderProductCollectionDataAccess.ApplyProductsFilter(GameAssets.ProviderProductList,
          GameAssets.ProviderProductFilter);
      AssetCollectionDataAccess.SaveAllUnpackedAssets(Settings.ArchiveAssetFolder,false,true);
      AssetCollectionDataAccess.SaveAllPackedAssets(Settings.ArchiveAssetFolder,false,true);
      GameAssets.BluePrintList = AssetCollectionDataAccess.ReadAllAssetsFromDatabase();
      GameAssets.FilteredBluePrintList =
        AssetCollectionDataAccess.ApplyAssetsFilter(GameAssets.BluePrintList,
          GameAssets.BluePrintFilter);
      OnPropertyChanged("GameAssets");
      }


    private void CancelArchiveDirTree()
      {
      throw new NotImplementedException();
      }


    }
  }

