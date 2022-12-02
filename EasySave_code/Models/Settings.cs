using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.Models
{
    class Settings
    {
        // --- Attributes ---
        public static Settings instance { get; set; }
        public Languages language { get; set; }


        // --- Constructors ---
        private Settings() { }


        // --- Methods ----
        // Singleton 
        public static Settings GetInstance()
        {
            if (instance == null)
            {
                instance = new Settings();
            }
            return instance;
        }
        public void Update(Languages _language)
        {
            this.language = _language;
        }
    }
}
