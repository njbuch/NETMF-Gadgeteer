//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TestApp {
    using Gadgeteer;
    using GTM = Gadgeteer.Modules;
    
    
    public partial class Program : Gadgeteer.Program {
        
        /// <summary>The SPlus module using sockets 6 and 11 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.SPlus sPlus1;
        
        /// <summary>The SPlus module using socket H2 of sPlus1 and socket 12 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.SPlus sPlus2;
        
        /// <summary>The Display N18 module using socket H1 of sPlus1.</summary>
        private Gadgeteer.Modules.GHIElectronics.Display_N18 display_N181;
        
        /// <summary>The Display N18 module using socket H1 of sPlus2.</summary>
        private Gadgeteer.Modules.GHIElectronics.Display_N18 display_N182;
        
        /// <summary>The Display N18 module using socket H2 of sPlus2.</summary>
        private Gadgeteer.Modules.GHIElectronics.Display_N18 display_N183;
        
        /// <summary>This property provides access to the Mainboard API. This is normally not necessary for an end user program.</summary>
        protected new static GHIElectronics.Gadgeteer.FEZSpider Mainboard {
            get {
                return ((GHIElectronics.Gadgeteer.FEZSpider)(Gadgeteer.Program.Mainboard));
            }
            set {
                Gadgeteer.Program.Mainboard = value;
            }
        }
        
        /// <summary>This method runs automatically when the device is powered, and calls ProgramStarted.</summary>
        public static void Main() {
            // Important to initialize the Mainboard first
            Program.Mainboard = new GHIElectronics.Gadgeteer.FEZSpider();
            Program p = new Program();
            p.InitializeModules();
            p.ProgramStarted();
            // Starts Dispatcher
            p.Run();
        }
        
        private void InitializeModules() {
            this.sPlus1 = new GTM.GHIElectronics.SPlus(6, 11);
            this.sPlus2 = new GTM.GHIElectronics.SPlus(this.sPlus1.SHubSocket2, 12);
            this.display_N181 = new GTM.GHIElectronics.Display_N18(this.sPlus1.SHubSocket1);
            this.display_N182 = new GTM.GHIElectronics.Display_N18(this.sPlus2.SHubSocket1);
            this.display_N183 = new GTM.GHIElectronics.Display_N18(this.sPlus2.SHubSocket2);
        }
    }
}
