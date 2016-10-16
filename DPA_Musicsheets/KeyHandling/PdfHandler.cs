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
        public EditorWrapper EditorWrapper { get; set; }
        public Key[] Keys { get; private set; } = new Key[] { Key.LeftCtrl, Key.P, Key.S };
        public PdfHandler() { }
        public PdfHandler(EditorWrapper editorWrapper) { EditorWrapper = editorWrapper; }
        public void Handle()
        {
            EditorWrapper?.SaveAsPdf();
        }
    }
}
