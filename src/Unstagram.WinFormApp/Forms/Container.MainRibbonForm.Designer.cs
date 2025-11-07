namespace Unstagram.WinFormApp
{
    partial class ContainerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ContainerForm));
            ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            BBI_OpenInstagramAnalyzer = new DevExpress.XtraBars.BarButtonItem();
            skinPaletteRibbonGalleryBarItem1 = new DevExpress.XtraBars.SkinPaletteRibbonGalleryBarItem();
            skinPaletteDropDownButtonItem1 = new DevExpress.XtraBars.SkinPaletteDropDownButtonItem();
            RPG_MainMenu = new DevExpress.XtraBars.Ribbon.RibbonPage();
            RPG_MainSubForms = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            ((System.ComponentModel.ISupportInitialize)ribbon).BeginInit();
            SuspendLayout();
            // 
            // ribbon
            // 
            ribbon.ExpandCollapseItem.Id = 0;
            ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbon.ExpandCollapseItem, BBI_OpenInstagramAnalyzer, skinPaletteRibbonGalleryBarItem1, skinPaletteDropDownButtonItem1 });
            ribbon.Location = new System.Drawing.Point(0, 0);
            ribbon.MaxItemId = 4;
            ribbon.Name = "ribbon";
            ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { RPG_MainMenu });
            ribbon.QuickToolbarItemLinks.Add(skinPaletteDropDownButtonItem1);
            ribbon.Size = new System.Drawing.Size(1278, 158);
            ribbon.StatusBar = ribbonStatusBar;
            // 
            // BBI_OpenInstagramAnalyzer
            // 
            BBI_OpenInstagramAnalyzer.Caption = "Instagram Analyzer";
            BBI_OpenInstagramAnalyzer.Id = 1;
            BBI_OpenInstagramAnalyzer.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("BBI_OpenInstagramAnalyzer.ImageOptions.Image");
            BBI_OpenInstagramAnalyzer.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("BBI_OpenInstagramAnalyzer.ImageOptions.LargeImage");
            BBI_OpenInstagramAnalyzer.Name = "BBI_OpenInstagramAnalyzer";
            BBI_OpenInstagramAnalyzer.ItemClick += BBI_OpenInstagramAnalyzer_ItemClick;
            // 
            // skinPaletteRibbonGalleryBarItem1
            // 
            skinPaletteRibbonGalleryBarItem1.Caption = "skinPaletteRibbonGalleryBarItem1";
            skinPaletteRibbonGalleryBarItem1.Id = 2;
            skinPaletteRibbonGalleryBarItem1.Name = "skinPaletteRibbonGalleryBarItem1";
            // 
            // skinPaletteDropDownButtonItem1
            // 
            skinPaletteDropDownButtonItem1.ActAsDropDown = true;
            skinPaletteDropDownButtonItem1.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
            skinPaletteDropDownButtonItem1.Id = 3;
            skinPaletteDropDownButtonItem1.Name = "skinPaletteDropDownButtonItem1";
            // 
            // RPG_MainMenu
            // 
            RPG_MainMenu.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { RPG_MainSubForms });
            RPG_MainMenu.Name = "RPG_MainMenu";
            RPG_MainMenu.Text = "Main";
            // 
            // RPG_MainSubForms
            // 
            RPG_MainSubForms.ItemLinks.Add(BBI_OpenInstagramAnalyzer);
            RPG_MainSubForms.Name = "RPG_MainSubForms";
            RPG_MainSubForms.Text = "Main Features";
            // 
            // ribbonStatusBar
            // 
            ribbonStatusBar.Location = new System.Drawing.Point(0, 695);
            ribbonStatusBar.Name = "ribbonStatusBar";
            ribbonStatusBar.Ribbon = ribbon;
            ribbonStatusBar.Size = new System.Drawing.Size(1278, 24);
            // 
            // ContainerForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1278, 719);
            Controls.Add(ribbonStatusBar);
            Controls.Add(ribbon);
            IconOptions.Image = (System.Drawing.Image)resources.GetObject("ContainerForm.IconOptions.Image");
            IsMdiContainer = true;
            Name = "ContainerForm";
            Ribbon = ribbon;
            StatusBar = ribbonStatusBar;
            Text = "Container";
            ((System.ComponentModel.ISupportInitialize)ribbon).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage RPG_MainMenu;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup RPG_MainSubForms;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private DevExpress.XtraBars.BarButtonItem BBI_OpenInstagramAnalyzer;
        private DevExpress.XtraBars.SkinPaletteRibbonGalleryBarItem skinPaletteRibbonGalleryBarItem1;
        private DevExpress.XtraBars.SkinPaletteDropDownButtonItem skinPaletteDropDownButtonItem1;
    }
}