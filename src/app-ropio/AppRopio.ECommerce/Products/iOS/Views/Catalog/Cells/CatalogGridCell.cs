﻿using System;
using AppRopio.Base.iOS;
using AppRopio.Base.iOS.Models.ThemeConfigs;
using AppRopio.Base.iOS.UIExtentions;
using AppRopio.ECommerce.Products.Core.Models;
using AppRopio.ECommerce.Products.Core.Services;
using AppRopio.ECommerce.Products.Core.ViewModels.Catalog.Items;
using AppRopio.ECommerce.Products.iOS.Models;
using AppRopio.ECommerce.Products.iOS.Services;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS.Views;
using MvvmCross.Platform;
using UIKit;
using AppRopio.Base.Core.Converters;
using AppRopio.Base.Core.Combiners;
using AppRopio.Base.Core.Services.ViewLookup;
using MvvmCross.iOS.Views;
using AppRopio.ECommerce.Products.Core;
using AppRopio.Base.Core.Services.Localization;
using AppRopio.ECommerce.Products.Core.Combiners;

namespace AppRopio.ECommerce.Products.iOS.Views.Catalog.Cells
{
    public partial class CatalogGridCell : MvxCollectionViewCell
    {
        protected virtual ProductsConfig Config { get { return Mvx.Resolve<IProductConfigService>().Config; } }

        protected virtual ProductsThemeConfig ThemeConfig { get { return Mvx.Resolve<IProductsThemeConfigService>().ThemeConfig; } }

        protected ILocalizationService LocalizationService => Mvx.Resolve<ILocalizationService>();

        public static NSString Key = new NSString("CatalogGridCell");
        public static UINib Nib = UINib.FromName("CatalogGridCell", NSBundle.MainBundle);

        protected virtual UIImageView Image => _image;
        protected virtual AppRopio.Base.iOS.Controls.ARLabel Name => _name;
        protected virtual AppRopio.Base.iOS.Controls.ARLabel Price => _price;
        protected virtual AppRopio.Base.iOS.Controls.ARLabel MaxPrice => _maxPrice;
        protected virtual AppRopio.Base.iOS.Controls.ARLabel OldPrice => _oldPrice;
        protected virtual UICollectionView Badges => _badges;
        protected virtual NSLayoutConstraint BadgesWidthConstraint => _badgesWidthContraint;
        protected virtual UIButton MarkButton => _markButton;

        protected CatalogGridCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                InitializeControls();
                BindControls();
            });
        }

        #region InitializationControls

        protected virtual void InitializeControls()
        {
            var cell = ThemeConfig.Products.ProductCell;

            SetupImage(Image, cell.Image);

            SetupName(Name, cell.Title);

            SetupPrice(Price, cell.Price);
            SetupMaxPrice(MaxPrice, cell.Price);

            SetupOldPrice(OldPrice, cell.OldPrice);

            SetupBadgesCollection(Badges);

            SetupMarkButton(MarkButton, cell.MarkButton);

            SetupBasketView();

            this.SetupStyle(cell);
        }

        protected virtual void SetupImage(UIImageView image, Image imageModel)
        {
            image?.SetupStyle(imageModel);
        }

        protected virtual void SetupName(UILabel name, Label nameLabel)
        {
            name?.SetupStyle(nameLabel);
        }

        protected virtual void SetupPrice(UILabel price, Label priceLabel)
        {
            price?.SetupStyle(priceLabel);
        }

        protected virtual void SetupMaxPrice(UILabel price, Label priceLabel)
        {
            price?.SetupStyle(priceLabel);
        }

        protected virtual void SetupOldPrice(UILabel oldPrice, Label oldPriceLabel)
        {
            oldPrice?.SetupStyle(oldPriceLabel);
        }

        protected virtual void SetupBadgesCollection(UICollectionView badges)
        {
            if (badges == null)
                return;
            
            (badges.CollectionViewLayout as UICollectionViewFlowLayout).ItemSize = new CoreGraphics.CGSize(BadgeCell.WIDTH, BadgeCell.HEIGHT);

            if (BadgesWidthConstraint != null)
                BadgesWidthConstraint.Constant = (BadgeCell.WIDTH * 2f).If_iPhone6(BadgeCell.WIDTH * 3f).If_iPhone6Plus(BadgeCell.WIDTH * 3f);

            badges.RegisterNibForCell(BadgeCell.Nib, BadgeCell.Key);
        }

        protected virtual void SetupMarkButton(UIButton markButton, Button button)
        {
            markButton.SetupStyle(button);
        }

        protected void SetupBasketView()
        {
            var config = Mvx.Resolve<IProductConfigService>().Config;
            if (config.Basket?.ItemAddToCart != null && Mvx.Resolve<IViewLookupService>().IsRegistered(config.Basket?.ItemAddToCart.TypeName))
            {
                var ViewModel = DataContext as ICatalogItemVM;
                var basketView = ViewModel.BasketBlockViewModel == null ? null : Mvx.Resolve<IMvxIosViewCreator>().CreateView(ViewModel.BasketBlockViewModel) as UIView;
                if (basketView != null)
                {
                    float height = ThemeConfig.Products.ProductCell.AddToCartHeight;
                    basketView.ChangeFrame(y: this.Frame.Size.Height - height, w: this.Frame.Size.Width, h: height);
                    this.AddSubview(basketView);
                }
            }
        }

        #endregion

        #region BindingControls

        protected virtual void BindControls()
        {
            var set = this.CreateBindingSet<CatalogGridCell, ICatalogItemVM>();

            BindImage(Image, set);
            BindName(Name, set);
            BindPrice(Price, set);
            BindMaxPrice(MaxPrice, set);
            BindOldPrice(OldPrice, set);
            BindBagesCollection(Badges, set);
            BindMarkButton(MarkButton, set);

            set.Apply();
        }

        protected virtual void BindImage(UIImageView image, MvxFluentBindingDescriptionSet<CatalogGridCell, ICatalogItemVM> set)
        {
            if (image == null)
                return;

            var imageLoader = new MvxImageViewLoader(() => image)
            {
                DefaultImagePath = $"res:{ThemeConfig.Products.ProductCell.Image.Path}",
                ErrorImagePath = $"res:{ThemeConfig.Products.ProductCell.Image.Path}"
            };

            set.Bind(imageLoader).To(vm => vm.ImageUrl);
        }

        protected virtual void BindName(UILabel name, MvxFluentBindingDescriptionSet<CatalogGridCell, ICatalogItemVM> set)
        {
            if (name == null) 
                return;

            set.Bind(name).To(vm => vm.Name);
        }

        protected virtual void BindPrice(UILabel price, MvxFluentBindingDescriptionSet<CatalogGridCell, ICatalogItemVM> set)
        {
            if (price == null)
                return;

            if (Config.PriceType == PriceType.To) {
                return;
            }

            MvxFluentBindingDescription<UILabel, ICatalogItemVM> priceBinding;
            if (Config.UnitNameEnabled)
                priceBinding = set.Bind(price).ByCombining(new PriceFromUnitCombiner(), new []{ "Price", "UnitName", "MaxPrice" });
            else
                priceBinding = set.Bind(price).ByCombining(new PriceFromFormatCombiner(), new []{ "Price", "MaxPrice" });
        }

        protected virtual void BindMaxPrice(UILabel maxPrice, MvxFluentBindingDescriptionSet<CatalogGridCell, ICatalogItemVM> set)
        {
            if (maxPrice == null)
                return;

            if (!(Config.PriceType == PriceType.To || Config.PriceType == PriceType.FromTo)) {
                maxPrice.Hidden = true;
                return;
            }

            MvxFluentBindingDescription<UILabel, ICatalogItemVM> priceBinding;
            if (Config.UnitNameEnabled)
                priceBinding = set.Bind(maxPrice).ByCombining(new PriceUnitCombiner(), new [] { "MaxPrice", "UnitName" });
            else
                priceBinding = set.Bind(maxPrice).To(vm => vm.MaxPrice);

            priceBinding.WithConversion(
                "StringFormat",
                new StringFormatParameter() {
                    StringFormat = (arg) => {
                        if (!Config.UnitNameEnabled) {
                            arg = new PriceFormatConverter().Convert(arg, typeof(Decimal?), null, null);
                        }
                        return $"{LocalizationService.GetLocalizableString(ProductsConstants.RESX_NAME, "Catalog_PriceTo")} {arg}";
                    }
                }
            );

            set.Bind(maxPrice).For("Visibility").To(vm => vm.MaxPrice).WithConversion("Visibility");
        }

        protected virtual void BindOldPrice(UILabel oldPrice, MvxFluentBindingDescriptionSet<CatalogGridCell, ICatalogItemVM> set)
        {
            if (oldPrice == null)
                return;

            if (!(Config.PriceType == PriceType.Default || Config.PriceType == PriceType.From)) {
                oldPrice.Hidden = true;
                return;
            }

            MvxFluentBindingDescription<UILabel, ICatalogItemVM> priceBinding;
            if (Config.UnitNameEnabled)
                priceBinding = set.Bind(oldPrice).ByCombining(new PriceUnitCombiner(), new[] { "OldPrice", "UnitNameOld" });
            else
                priceBinding = set.Bind(oldPrice).To(vm => vm.OldPrice);

            if (Config.PriceType == PriceType.From) {
                priceBinding.WithConversion(
                    "StringFormat",
                    new StringFormatParameter() {
                        StringFormat = (arg) => {
                            if (!Config.UnitNameEnabled) {
                                arg = new PriceFormatConverter().Convert(arg, typeof(Decimal?), null, null);
                            }
                            return $"{LocalizationService.GetLocalizableString(ProductsConstants.RESX_NAME, "Catalog_PriceFrom")} {arg}";
                        }
                    }
                );
            }

            set.Bind(oldPrice).For("Visibility").To(vm => vm.OldPrice).WithConversion("Visibility");
        }

        protected virtual void BindBagesCollection(UICollectionView badges, MvxFluentBindingDescriptionSet<CatalogGridCell, ICatalogItemVM> set)
        {
            if (badges == null)
                return;
            
            var dataSource = SetupBadgesViewSource(badges);
            badges.Source = dataSource;

            set.Bind(dataSource).To(vm => vm.Badges);

            badges.ReloadData();
        }

        protected virtual MvxCollectionViewSource SetupBadgesViewSource(UICollectionView badges)
        {
            return new MvxCollectionViewSource(badges, BadgeCell.Key);
        }

        protected virtual void BindMarkButton(UIButton markButton, MvxFluentBindingDescriptionSet<CatalogGridCell, ICatalogItemVM> set)
        {
            if (markButton == null)
                return;
            
            set.Bind(markButton).To(vm => vm.MarkCommand);
            set.Bind(markButton).For(v => v.Selected).To(vm => vm.Marked);
            set.Bind(markButton).For("Visibility").To(vm => vm.MarkEnabled).WithConversion("Visibility");
        }

        #endregion
    }
}
