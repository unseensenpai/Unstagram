using Unstagram.WinFormApp.Forms;

namespace Unstagram.WinFormApp
{
    public partial class ContainerForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public ContainerForm()
        {
            InitializeComponent();
        }


        public void BBI_OpenInstagramAnalyzer_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InstagramAnalyzerForm instagramAnalyzer = new();
            instagramAnalyzer.MdiParent = this;
            instagramAnalyzer.Show();
        }

    }
}