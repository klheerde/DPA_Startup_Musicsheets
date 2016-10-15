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
        public static readonly int MAX_KEYS = 3;

        public UIElement Element { get; private set; }
        public List<Key> PressedKeys { get; private set; } = new List<Key>();
        public List<IKeyHandler> Handlers { get; private set; } = new List<IKeyHandler>();
        public KeyBinder(UIElement element)
        {
            Element = element;
            element.KeyDown += Element_KeyDown;
            element.KeyUp += Element_KeyUp;
        }

        //public void AddBindingsToUiElement(UIElement element)
        //{
        //    //foreach (IKeyHandler handler in Handlers)
        //    //element.CommandBindings.Add(HandlerToCommandBinding(handler));
        //}

        //NOTE: key up, save in array.
        private void Element_KeyDown(object sender, KeyEventArgs e)
        {
            ////NOTE: if 4th key hit, see as 1th.
            //if (PressedKeys.Count >= MAX_KEYS)
            //    PressedKeys.Clear();

            //TODO first key must be modifier
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

        private void Element_KeyUp(object sender, KeyEventArgs e)
        {
            PressedKeys.Remove(e.Key == Key.System ? e.SystemKey : e.Key);
            if (PressedKeys.Count > 0 || currentHandler == null)
                return;
            currentHandler.Handle();
            currentHandler = null;
        }

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
    }
}
