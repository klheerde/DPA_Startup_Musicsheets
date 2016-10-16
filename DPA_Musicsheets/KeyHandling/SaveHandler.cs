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
    class SaveHandler : IKeyHandler
    {
        public EditorWrapper EditorWrapper { get; set; }
        public Key[] Keys { get; private set; } = new Key[] { Key.LeftCtrl, Key.S };

        public SaveHandler() { }
        public SaveHandler(EditorWrapper editorWrapper) { EditorWrapper = editorWrapper; }

        public void Handle()
        {
            EditorWrapper?.Save();
        }

        //public Key Key { get; private set; } = Key.S;
        //public ModifierKeys[] ModifierKeys { get; private set; } = new ModifierKeys[] { ModKeys.Control };
        //public void Handle(object sender, ExecutedRoutedEventArgs e)
        //{
        //    if (EditorWrapper == null)
        //        return;
        //    EditorWrapper.Save();
        //}
    }
}
