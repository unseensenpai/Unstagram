using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Windows.Forms;

namespace Unstagram.WinFormApp.Forms
{
    public partial class InstagramAnalyzerForm : DevExpress.XtraEditors.XtraForm
    {
        public InstagramAnalyzerForm()
        {
            InitializeComponent();
        }

        public void InitializeUIValeus()
        {
            // RadioGroup içeriğini doldur.
        }

        private void RG_ContentTypeSelector_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            // Seçilen indexe göre open file atarken json veya html isteği yap. multi select olacak.

        }

        private void ACE_OpenFiles_Click(object sender, System.EventArgs e)
        {
            // Open file dialog (multi-select). Use radio selection to decide filter (default JSON).
            using var ofd = new OpenFileDialog
            {
                Multiselect = true,
                Title = "Select analyze files",
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                CheckFileExists = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            // If you have a radio group control named RG_ContentTypeSelector and want to support HTML:
            try
            {
                if (this.Controls.Find("RG_ContentTypeSelector", true).FirstOrDefault() is DevExpress.XtraEditors.RadioGroup rg)
                {
                    var sel = rg.SelectedIndex;
                    // assume index 0 = JSON, 1 = HTML (adjust if different)
                    ofd.Filter = sel == 1
                        ? "HTML files (*.html;*.htm)|*.html;*.htm|All files (*.*)|*.*"
                        : "JSON files (*.json)|*.json|All files (*.*)|*.*";
                }
            }
            catch
            {
                // ignore and keep default filter
            }

            if (ofd.ShowDialog(this) != DialogResult.OK) return;



            // Dosyaları seçtir. Seçilen dosyalara göre XtraTabları yeşil veya kırmızı yap. 
            // Yeşil olan tablarda GridView'ler aktif olur.
            // Kırmızı olan tablarda GridView'ler pasif olur.
            // Seçilen dosyaların içeriklerine göre GridView'leri doldur.
            // Dosya okuma işleminde json mı html mi olacağını radio editten al.
        }

        private void GV_DoubleClick(object sender, System.EventArgs e)
        {
            if (sender is GridView view)
            {
                switch (view.Name)
                {
                    // TODO View tipine göre modele git ve urli çalıştır.

                    default:
                        break;
                }
            }
        }

        private void GV_KeyDown (object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (sender is not GridView view) return;

            if (e.Control && e.KeyCode == Keys.C)
            {
                try
                {
                    object value = view.GetFocusedValue();
                    // If no focused value try focused column cell specifically
                    if (value == null && view.FocusedColumn != null)
                    {
                        value = view.GetRowCellValue(view.FocusedRowHandle, view.FocusedColumn);
                    }

                    if (value != null)
                    {
                        string text = value.ToString();
                        Clipboard.SetText(text);
                        // optionally show a short feedback (comment out if undesired)
                        // XtraMessageBox.Show(this, "Copied to clipboard", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    // mark handled to avoid further processing
                    e.Handled = true;
                }
                catch (Exception ex)
                {
                    // swallow but inform in debug / minimal UI
                    System.Diagnostics.Debug.WriteLine($"Copy to clipboard failed: {ex}");
                }
            }
        }
    }
}