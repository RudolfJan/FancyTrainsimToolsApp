using Assets.Library.Logic;
using Assets.Library.Models;
using Caliburn.Micro;
using FancyTrainsimTools.desktop.Helpers;
using FancyTrainsimTools.Desktop.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;

namespace FancyTrainsimTools.Desktop.ViewModels
  {
  public class GameAssetsViewModel: Screen
    {
    public GameAssetsModel GameAssets { get; set; } = new GameAssetsModel();
    public ICommand CopyToArchiveCommand { get; set; }
    public ICommand CopyToGameCommand { get; set; }

    public GameAssetsViewModel()
      {
      }

    protected override void OnViewLoaded(object view)
      {
      base.OnViewLoaded(view);
      // ProviderProduct initialization
      GameAssets.ProviderProductFilter = new ProviderProductFilterModel();
      CopyToArchiveCommand = new RelayCommand<object>(CopyToArchive,CanCopy);
      CopyToGameCommand = new RelayCommand<object>(CopyToGame,CanCopy);
      GameAssets.ProviderProductList = ProviderProductCollectionDataAccess.ReadAllProviderProductsFromDatabase();
      GameAssets.FilteredProviderProductList = new BindableCollection<ProviderProductModel>(
        ProviderProductCollectionDataAccess.ApplyProductsFilter(GameAssets.ProviderProductList,
          GameAssets.ProviderProductFilter));

// BluePrint initialization
      GameAssets.BluePrintFilter = new BluePrintFilterModel();

      GameAssets.BluePrintList = AssetCollectionDataAccess.ReadAllAssetsFromDatabase();
      GameAssets.FilteredBluePrintList = new BindableCollection<FlatAssetModel>(
        AssetCollectionDataAccess.ApplyAssetsFilter(GameAssets.BluePrintList,
          GameAssets.BluePrintFilter));
      }

    public void SetProductFilter()
      {
      GameAssets.FilteredProviderProductList = new BindableCollection<ProviderProductModel>(
        ProviderProductCollectionDataAccess.ApplyProductsFilter(GameAssets.ProviderProductList,
          GameAssets.ProviderProductFilter));
      NotifyOfPropertyChange(()=>GameAssets);
      }

    public void SetGameAssetsFilter()
      {
      GameAssets.FilteredBluePrintList = new BindableCollection<FlatAssetModel>( AssetCollectionDataAccess.ApplyAssetsFilter(GameAssets.BluePrintList,
        GameAssets.BluePrintFilter));      // Fore some reason, no notification is generated if this changes, so do it manually in this case
      NotifyOfPropertyChange(()=>GameAssets);
      }

    public void GetInGameDirTree()
      {
      //TODO may move most of this to the library.
      ProviderProductCollectionDataAccess.SaveAllProviderProducts(Settings.GameAssetFolder,true,false);
      GameAssets.ProviderProductList = ProviderProductCollectionDataAccess.ReadAllProviderProductsFromDatabase();
      GameAssets.FilteredProviderProductList = new BindableCollection<ProviderProductModel>(
        ProviderProductCollectionDataAccess.ApplyProductsFilter(GameAssets.ProviderProductList,
          GameAssets.ProviderProductFilter));
      AssetCollectionDataAccess.SaveAllUnpackedAssets(Settings.GameAssetFolder,true,false);
      AssetCollectionDataAccess.SaveAllPackedAssets(Settings.GameAssetFolder,true,false);
      GameAssets.BluePrintList = AssetCollectionDataAccess.ReadAllAssetsFromDatabase();
      GameAssets.FilteredBluePrintList = new BindableCollection<FlatAssetModel>(
        AssetCollectionDataAccess.ApplyAssetsFilter(GameAssets.BluePrintList,
          GameAssets.BluePrintFilter));
      NotifyOfPropertyChange(()=>GameAssets);
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

    public void CancelGetInGameDir()
      {
      throw new NotImplementedException();
      }
 
    public void CopyToArchive(object arg)
      {
      CopyBase(arg,false);
      // TODO make this efficient, do not update the whole archive, instead add the additional itesm to the list
      GetArchiveDirTree();
      }
    public void CopyToGame(object arg)
      {
      CopyBase(arg,true);
      // TODO make this efficient, do not update the whole archive, instead add the additional itesm to the list
      GetInGameDirTree();
      }

    private void CopyBase(object arg, bool toGame)
      {
      List<ProviderProductModel> SelectedProviderProducts = new List<ProviderProductModel>();
      foreach (var item in  (IList<object>)arg)
        {
        ProviderProductModel item2 = (ProviderProductModel)item;
        SelectedProviderProducts.Add(item2);
        }
      ProviderProductCollectionDataAccess.CopyAssets(SelectedProviderProducts, toGame,
        Settings.GameAssetFolder, Settings.ArchiveAssetFolder);
      
      NotifyOfPropertyChange(()=>GameAssets);
      }

    public void GetArchiveDirTree()
      {
      ProviderProductCollectionDataAccess.SaveAllProviderProducts(Settings.ArchiveAssetFolder,false,true);
      GameAssets.ProviderProductList = ProviderProductCollectionDataAccess.ReadAllProviderProductsFromDatabase();
      GameAssets.FilteredProviderProductList = new BindableCollection<ProviderProductModel>(
        ProviderProductCollectionDataAccess.ApplyProductsFilter(GameAssets.ProviderProductList,
          GameAssets.ProviderProductFilter));
      AssetCollectionDataAccess.SaveAllUnpackedAssets(Settings.ArchiveAssetFolder,false,true);
      AssetCollectionDataAccess.SaveAllPackedAssets(Settings.ArchiveAssetFolder,false,true);
      GameAssets.BluePrintList = AssetCollectionDataAccess.ReadAllAssetsFromDatabase();
      GameAssets.FilteredBluePrintList = new BindableCollection<FlatAssetModel>(
        AssetCollectionDataAccess.ApplyAssetsFilter(GameAssets.BluePrintList,
          GameAssets.BluePrintFilter));
      NotifyOfPropertyChange(()=>GameAssets);
      }

    public void CancelArchiveDirTree()
      {
      throw new NotImplementedException();
      }


    }
  }

