using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable

namespace LockApp
{
    public class StatusProvider : INotifyPropertyChanged
    {
        private Dictionary<string, object> Values = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        private object? GetValue([CallerMemberName] string name = "")
        {
            if (!Values.ContainsKey(name))
                return null;
            return Values[name];
        }

        private void SetValue(object value, [CallerMemberName] string name = "")
        {
            Values[name] = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string AccessibleText { get => (string?)GetValue() ?? ""; set => SetValue(value); }
        public string Icon { get => (string?)GetValue() ?? ""; set => SetValue(value); }
        public bool IsVisible { get => (bool?)GetValue() ?? false; set => SetValue(value); }
    }

    public class LockInfo : INotifyPropertyChanged
    {
        private Dictionary<string, object> Values = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        private object? GetValue([CallerMemberName] string name = "")
        {
            if (!Values.ContainsKey(name))
                return null;
            return Values[name];
        }

        private void SetValue(object value, [CallerMemberName] string name = "")
        {
            Values[name] = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string Clock { get => (string?)GetValue() ?? ""; set => SetValue(value); }
        public string Date { get => (string?)GetValue() ?? ""; set => SetValue(value); }
        public string Detail1 { get => (string?)GetValue() ?? ""; set => SetValue(value); }
        public string Detail2 { get => (string?)GetValue() ?? ""; set => SetValue(value); }
        public string Detail3 { get => (string?)GetValue() ?? ""; set => SetValue(value); }

        public string AlarmsIcon { get => (string?)GetValue() ?? ""; set => SetValue(value); }

        public bool IsDetailedStatusVisible { get => (bool?)GetValue() ?? false; set => SetValue(value); }

        public StatusProvider BatteryStatusProvider { get => (StatusProvider?)GetValue() ?? new(); set => SetValue(value); }
        public StatusProvider NetworkStatusProvider { get => (StatusProvider?)GetValue() ?? new(); set => SetValue(value); }
    }
}
