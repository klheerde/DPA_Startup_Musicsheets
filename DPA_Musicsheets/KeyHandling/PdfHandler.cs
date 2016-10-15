using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ModKeys = System.Windows.Input.ModifierKeys;

namespace DPA_Musicsheets.KeyHandling
{
    class PdfHandler : IKeyHandler
    {
        public Key[] Keys { get; private set; } = new Key[] { Key.LeftCtrl, Key.P, Key.S };
        public void Handle()
        {
            MessageBox.Show("ehkfhkdsjgh");
        }
    }
}
