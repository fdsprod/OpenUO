using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenUO.Core.PresentationFramework.TypeEditors
{
    public class IntegerTypeEditor : TextBox
    {
        private readonly int _step = 1;

        static IntegerTypeEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IntegerTypeEditor), new FrameworkPropertyMetadata(typeof(IntegerTypeEditor)));
        }

        public IntegerTypeEditor()
        {
            InitializeCommands();
        }

        public static RoutedCommand UpCommand
        {
            get;
            private set;
        }

        public static RoutedCommand DownCommand
        {
            get;
            private set;
        }

        private static void InitializeCommands()
        {
            UpCommand = new RoutedCommand("UpCommand", typeof(IntegerTypeEditor));
            CommandManager.RegisterClassCommandBinding(typeof(IntegerTypeEditor), new CommandBinding(UpCommand, OnUpCommand));
            CommandManager.RegisterClassInputBinding(typeof(IntegerTypeEditor), new InputBinding(UpCommand, new KeyGesture(Key.Up)));

            DownCommand = new RoutedCommand("DownCommand", typeof(IntegerTypeEditor));
            CommandManager.RegisterClassCommandBinding(typeof(IntegerTypeEditor), new CommandBinding(DownCommand, OnDownCommand));
            CommandManager.RegisterClassInputBinding(typeof(IntegerTypeEditor), new InputBinding(DownCommand, new KeyGesture(Key.Down)));
        }

        private static void OnUpCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if(sender.GetType() == typeof(IntegerTypeEditor))
            {
                var _myIntegerTypeEditor = (IntegerTypeEditor)sender;
                _myIntegerTypeEditor.CountUp();
            }
        }

        private static void OnDownCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if(sender.GetType() == typeof(IntegerTypeEditor))
            {
                var _myIntegerTypeEditor = (IntegerTypeEditor)sender;
                _myIntegerTypeEditor.CountDown();
            }
        }

        protected void CountUp()
        {
            Text = (Int32.Parse(Text) + _step).ToString();
        }

        protected void CountDown()
        {
            Text = (Int32.Parse(Text) - _step).ToString();
        }
    }
}