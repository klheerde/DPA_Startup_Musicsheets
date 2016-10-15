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
    class OpenHandler : IKeyHandler
    {
        public Action OpenFunc { get; set; }
        public Key[] Keys { get; private set; } = new Key[] { Key.LeftCtrl, Key.O };

        public OpenHandler() { }
        public OpenHandler(Action action) { OpenFunc = action; }

        public void Handle()
        {
            OpenFunc?.Invoke();
        }
    }
}
