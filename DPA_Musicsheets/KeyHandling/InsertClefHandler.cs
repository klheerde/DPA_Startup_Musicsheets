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
    class InsertClefHandler : IKeyHandler
    {
        public Action InsertFunc { get; set; }

        //NOTE: System is left alt key.
        public Key[] Keys { get; private set; } = new Key[] { Key.LeftAlt, Key.C };

        public InsertClefHandler() { }
        public InsertClefHandler(Action action) { InsertFunc = action; }

        public void Handle()
        {
            InsertFunc?.Invoke();
        }
    }
}
