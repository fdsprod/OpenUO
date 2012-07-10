using System;
using System.Windows.Forms;
using OpenUO.Core.Diagnostics;
using OpenUO.Core.Patterns;

namespace Ultima.Winforms.Sample
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if DEBUG
            new DebugTraceListener { TraceLevel = TraceLevels.Verbose };
#endif
            IoCContainer container = new IoCContainer();

            container.RegisterModule<OpenUO.Ultima.UltimaSDKCoreModule>();
            container.RegisterModule<OpenUO.Ultima.Windows.Forms.UltimaSDKBitmapModule>();

            container.Register<SampleForm>();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(container.Resolve<SampleForm>());
        }
    }
}
