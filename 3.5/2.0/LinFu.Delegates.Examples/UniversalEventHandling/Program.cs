using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LinFu.Delegates;
namespace UniversalEventHandling
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            Form1 form = new Form1();
            // Define the handler
            CustomDelegate clickHandler = delegate(object[] args)
            {
                object source = args[0];
                MouseEventArgs e = (MouseEventArgs)args[1];
                
                MessageBox.Show(string.Format("Form1 Clicked! X: {0} Y: {1}", e.X, e.Y));
                return null;
            };
            
            // Attach the handler to the form
            EventBinder.BindToEvent("Click", form, clickHandler);
            Application.Run(form);
        }
    }
}