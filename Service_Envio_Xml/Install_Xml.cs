using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace Service_Envio_Xml
{
    [RunInstaller(true)]
    public partial class Install_Xml : System.Configuration.Install.Installer
    {
        public Install_Xml()
        {
            InitializeComponent();
        }
    }
}
