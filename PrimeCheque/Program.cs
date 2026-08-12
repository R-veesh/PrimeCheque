using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.UI.Xaml;

namespace PrimeCheque
{
    public static class Program2
    {
        [DllImport("Microsoft.ui.xaml.dll")]
        private static extern void XamlCheckProcessRequirements();

        [STAThread]
        static void Main(string[] args)
        {
            var logPath = @"C:\ProgramData\PrimeOne\PrimeCheque\startup.log";
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
                File.AppendAllText(logPath, $"\n[{DateTime.Now:O}] 1. Process started\n");
                
                XamlCheckProcessRequirements();
                
                WinRT.ComWrappersSupport.InitializeComWrappers();
                
                Application.Start((p) =>
                {
                    File.AppendAllText(logPath, $"[{DateTime.Now:O}] 2. App initialization started\n");
                    var context = new Microsoft.UI.Dispatching.DispatcherQueueSynchronizationContext(Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread());
                    SynchronizationContext.SetSynchronizationContext(context);
                    new App();
                });
            }
            catch (Exception ex)
            {
                File.AppendAllText(logPath, $"[{DateTime.Now:O}] 6. FATAL EXCEPTION: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}\n");
            }
            finally
            {
                File.AppendAllText(logPath, $"[{DateTime.Now:O}] 7. Process shutdown\n");
            }
        }
    }
}
