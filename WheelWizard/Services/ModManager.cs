using CT_MKWII_WPF.Models.Settings;
using CT_MKWII_WPF.Services.Installation;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CT_MKWII_WPF.Services
{
    public class ModManager : INotifyPropertyChanged
    {
        private static readonly Lazy<ModManager> _instance = new(() => new ModManager());
        public static ModManager Instance => _instance.Value;

        private ObservableCollection<Mod> _mods;
        public ObservableCollection<Mod> Mods
        {
            get => _mods;
            private set
            {
                _mods = value;
                OnPropertyChanged();
            }
        }

        private ModManager()
        {
            _mods = new ObservableCollection<Mod>();
        }
        
        public bool IsModInstalled(int modID)
        {
            return Mods.Any(mod => mod.ModID == modID);
        }

        public async Task LoadModsAsync()
        {
            _mods = await ModInstallation.LoadModsAsync();
            foreach (var mod in _mods)
            {
                mod.PropertyChanged += Mod_PropertyChanged;
            }
            OnPropertyChanged(nameof(Mods));
        }

        public async Task SaveModsAsync()
        {
            await ModInstallation.SaveModsAsync(_mods);
        }

        public void AddMod(Mod mod)
        {
            if (!ModInstallation.ModExists(_mods, mod.Title))
            {
                mod.PropertyChanged += Mod_PropertyChanged;
                _mods.Add(mod);
                SortModsByPriority();
                SaveModsAsync();
            }
        }

        public void RemoveMod(Mod mod)
        {
            if (_mods.Contains(mod))
            {
                _mods.Remove(mod);
                SaveModsAsync();
            }
        }

        private void Mod_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Mod.IsEnabled) || e.PropertyName == nameof(Mod.Title) || e.PropertyName == nameof(Mod.Author) || e.PropertyName == nameof(Mod.ModID) || e.PropertyName == nameof(Mod.Priority))
            {
                SaveModsAsync();
                SortModsByPriority();
            }
        }

        private void SortModsByPriority()
        {
            var sortedMods = new ObservableCollection<Mod>(_mods.OrderBy(m => m.Priority));
            _mods = sortedMods;
            OnPropertyChanged(nameof(Mods));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
