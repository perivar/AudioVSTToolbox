// Sound Fingerprinting framework
// https://code.google.com/p/soundfingerprinting/
// Code license: GNU General Public License v2
// ciumac.sergiu@gmail.com

using System;
using System.ComponentModel;

namespace Wave2Zebra2Preset.Model
{
    /// <summary>
    ///   Class for folder representation in the UI element
    /// </summary>
    [Serializable]
    
    public class Item : INotifyPropertyChanged
    {
        /// <summary>
        ///   Number of music files within the folder
        /// </summary>
        private int _count;

        /// <summary>
        ///   Path to folder
        /// </summary>
        private string _path;

        /// <summary>
        ///   See if current item is folder or a separate file
        /// </summary>
        public bool IsFolder { get; set; }

        /// <summary>
        ///   Path to the folder
        /// </summary>
        public string Path
        {
            get { return _path; }
            set
            {
                if (_path != value)
                {
                    _path = value;
                    OnPropertyChanged("Path");
                }
            }
        }

        /// <summary>
        ///   Number of songs within the folder
        /// </summary>
        public Int32 Count
        {
            get { return _count; }
            set
            {
                if (_count != value)
                {
                    _count = value;
                    OnPropertyChanged("Count");
                }
            }
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        ///   Property changed event handler
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void OnPropertyChanged(string property)
        {
            PropertyChangedEventHandler temp = PropertyChanged;
            if (temp != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}