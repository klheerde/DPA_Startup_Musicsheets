using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DPA_Musicsheets.KeyHandling
{
    interface IKeyHandler
    {
        //Key Key { get; }
        //ModifierKeys[] ModifierKeys { get; }
        //void Handle(object sender, ExecutedRoutedEventArgs e);
        Key[] Keys { get; }
        void Handle();
    }
}
