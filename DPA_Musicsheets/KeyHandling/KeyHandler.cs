using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DPA_Musicsheets.KeyHandling
{
    class KeyHandler : IKeyHandler
    {
        public Key[] Keys { get; private set; }
        public Action Action { get; private set; }
        public KeyHandler(Key[] keys, Action action)
        {
            Keys = keys;
            Action = action;
        }
        public void Handle()
        {
            Action?.Invoke();
        }
    }
}
