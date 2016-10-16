using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DPA_Musicsheets.KeyHandling
{
    class KeyBinder
    {
        public static Window MainWindow { get; set; }
        public UIElement Element { get; private set; }
        public List<Key> PressedKeys { get; private set; } = new List<Key>();
        public List<IKeyHandler> Handlers { get; private set; } = new List<IKeyHandler>();
        public KeyBinder(UIElement element)
        {
            Element = element;
            element.KeyDown += Element_KeyDown;
            element.KeyUp += Element_KeyUp;
            element.LostFocus += Element_LostFocus;
            //NOTE: throw nullpointer purposely. Must be added before first creation.
            MainWindow.Deactivated += MainWindow_Deactivated;
        }

        //NOTE: key up, save in array.
        private void Element_KeyDown(object sender, KeyEventArgs e)
        {
            if (PressedKeys.Contains(e.Key))
                return;
            PressedKeys.Add(e.Key == Key.System ? e.SystemKey : e.Key);
            CheckHandlers();
        }

        private IKeyHandler currentHandler = null;
        public void CheckHandlers()
        {
            foreach (IKeyHandler handler in Handlers)
            {
                if (PressedKeys.Count == handler.Keys.Length && PressedKeys.All(k => handler.Keys.Contains(k)))
                {
                    currentHandler = handler;
                    return;
                }
            }
        }

        private void Element_KeyUp(object sender, KeyEventArgs e)
        {
            PressedKeys.Remove(e.Key == Key.System ? e.SystemKey : e.Key);
            if (PressedKeys.Count > 0 || currentHandler == null)
                return;
            currentHandler.Handle();
            currentHandler = null;
        }

        private void Element_LostFocus(object sender, RoutedEventArgs e)
        {
            PressedKeys.Clear();
            currentHandler = null;
        }

        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            Element_LostFocus(sender, null);
        }

        #region old
        //public void AddBindingsToUiElement(UIElement element)
        //{
        //    //foreach (IKeyHandler handler in Handlers)
        //    //element.CommandBindings.Add(HandlerToCommandBinding(handler));
        //}
        //private bool IsHandler(IKeyHandler handler)
        //{
        //    //NOTE: order of pressing is important. index++ add after execution.
        //    int length = handler.Keys.Length;
        //    if (length != PressedKeys.Count)
        //        return false;
        //    //NOTE: no index out of bounds becouse above.
        //    for (int i = 0; i < length; i++)
        //        if (handler.Keys[i] != PressedKeys[i])
        //            return false;
        //    return true;
        //}
        //public void Execute(IKeyHandler handler)
        //{
        //    handler.Handle();
        //    PressedKeys.Clear();
        //}
        //public CommandBinding HandlerToCommandBinding(IKeyHandler handler)
        //{
        //    ModifierKeys modifierKeys = ModifierKeys.None;
        //    foreach (ModifierKeys modifierKey in handler.ModifierKeys)
        //        modifierKeys |= modifierKey;
        //    KeyGesture keyGesture = modifierKeys == ModifierKeys.None ? 
        //        new KeyGesture(handler.Key) : new KeyGesture(handler.Key, modifierKeys);
        //    RoutedCommand routedCommand = new RoutedCommand();
        //    routedCommand.InputGestures.Add(keyGesture);
        //    CommandBinding commandBinding = new CommandBinding();
        //    commandBinding.Command = routedCommand;
        //    commandBinding.Executed += handler.Handle;
        //    return commandBinding;
        //}
        #endregion
    }
}
