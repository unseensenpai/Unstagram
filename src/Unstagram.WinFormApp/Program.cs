using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.UserSkins;
using System;
using System.Windows.Forms;

namespace Unstagram.WinFormApp;

internal static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        BonusSkins.Register();
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        UserLookAndFeel.Default.SetSkinStyle(SkinStyle.DevExpress);
        SkinManager.EnableMdiFormSkins();

        Application.Run(new ContainerForm());
    }
}
