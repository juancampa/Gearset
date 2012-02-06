using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Management;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System.Reflection;

namespace Gearset.Component
{
    public class GraphicsDevice : INotifyPropertyChanged
    {
        private String licenseMessage;
        public String LicenseMessage
        {
            get
            {
                if (isValid)
                    return "Product Key is valid";
                else if (remainingDays > 0)
                    if (TimeEnabled)
                        return "Product Key is not valid. Trial period running.";
                    else
                        return "Product Key is not valid.";
                else
                    if (TimeEnabled)
                        return "Product Key is not valid. Trial period has ended. Consider purchasing a license.";
                    else
                        return "Product Key is not valid.";
            }
        }

        public String RemainingDaysString
        {
            get { return remainingDays.ToString(); }
        }

        private String hardwareId;
        public String HardwareId
        {
            get { return hardwareId; }
            set { hardwareId = value; OnPropertyChanged("HardwareId"); }
        }

        private String license;
        public String License
        {
            get { return license; }
            set { license = value; OnPropertyChanged("License"); OnLicenseChanged(); }
        }

        private int remainingDays;
        public int RemainingDays
        {
            get { return remainingDays; }
            set { remainingDays = value; OnPropertyChanged("RemainingDays"); OnPropertyChanged("RemainingDaysString"); }
        }

        private bool isValid;
        public bool IsValid
        {
            get { return isValid; }
            set
            {
                bool prevValue = isValid;
                isValid = value;
                OnPropertyChanged("LicenseMessage");
                if (isValid != prevValue)
                    OnLicenseStatusChanged();
            }
        }

        public string ProductName { get; private set; }

        /// <summary>
        /// Determines whether the current program requires time-bombed license
        /// manager.
        /// </summary>
        public bool TimeEnabled { get; set; }

        public GraphicsDevice(String productName)
        {
            ProductName = productName;
            License = "(Paste your license here)";
            HardwareId = String.Empty;
        }

        private void OnLicenseChanged()
        {
            Console.WriteLine("License Changing, LicenseChanged event not null? {0}, new License: {1}", LicenseChanged!=null, this.License);
            if (LicenseChanged != null)
                LicenseChanged(this, EventArgs.Empty);
        }


        private void OnLicenseStatusChanged()
        {
            if (LicenseStatusChanged != null)
                LicenseStatusChanged(this, EventArgs.Empty);
        }

        private void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raised when the license string changes.
        /// </summary>
        public event EventHandler LicenseChanged;

        /// <summary>
        /// Raised when the license status (valid or invalid changes). Only raised
        /// when an actual change is made (i.e. not everytime the property is set).
        /// </summary>
        public event EventHandler LicenseStatusChanged;

        
    }
    public class GraphicsDeviceManager
    {
        private GraphicsDevice license;
        private String productName;

        /// <summary>
        /// Raised when the license status changes.
        /// </summary>
        public event EventHandler<EventArgs> Reset; // LicenseStatusChanged
        private LicenseWindow window;

        public bool IsAvailable { get { return CheckPermitsUse(); } } // Permits use
        public int ParameterIndex { get { return license.RemainingDays; } }                 // Remaining days
        public bool IsStarted { get { return !license.IsValid && ParameterIndex > 0; } }    // Is trial

        public GraphicsDeviceManager(String productName)
        {
            this.productName = productName;

            // Initialize the current license status.
            license = new GraphicsDevice(productName);
            license.HardwareId = FindHardwareId();
            RegistryEditor regedit = new RegistryEditor(Registry.CurrentUser, "Software/The Complot/" + productName);
            license.License = regedit.Read("License");
            license.TimeEnabled = false;

            // Listen for when the license becomes valid.
            license.LicenseStatusChanged += new EventHandler(license_LicenseStatusChanged);

            // No license written yet?
            if (license.License == null)
            {
                license.License = String.Empty;
            }
            CheckLicense();
        }

        void license_LicenseStatusChanged(object sender, EventArgs e)
        {
            // The trial period ended.
            if (Reset != null)
                Reset(this, EventArgs.Empty);
        }

        void license_LicenseChanged(object sender, EventArgs e)
        {
            Console.WriteLine("License Manager: License Just Changed, Checking Permission");
            CheckPermitsUse();
        }

        private int consistentIf34firstRunIf76;
        private long startDate;
        private long lastUse;
        private bool licenseNullMessageThrown;

        private bool CheckPermitsUse()
        {
            Console.WriteLine("Checking Permits Use");
            if (license.TimeEnabled && CheckFirstRun())
            {
                Console.WriteLine("First Run");
                if (license.TimeEnabled)
                    SaveInitialData();
            }
            else
            {
                Console.WriteLine("Not First Run");
                if (license.IsValid = CheckLicense())
                    return true;
                else if (!license.TimeEnabled)
                    // If the program does not require a time-bomb. Just return
                    // false as there's no ClockHack that could be performed.
                    return false;
            }

            Console.WriteLine("Checking clock hack");
            if (CheckClockHack())
            {
                Console.WriteLine("Saving Last Use time stamp");
                SaveLastUse();
                int remainingDays;
                if (CheckTimeTrial(out remainingDays))
                {
                    Console.WriteLine("Time trial still on");
                    license.RemainingDays = remainingDays;
                    return true;
                }
                else
                {
                    Console.WriteLine("Time trial is over");
                    license.RemainingDays = remainingDays;
                    return false;
                }
            }
            else
            {
                Console.WriteLine("WTF, returning false");
                return false;
            }
        }

        /// <summary>
        /// Returns true if this is the first run.
        /// </summary>
        private bool CheckFirstRun()
        {
            // Chuleta:
            // conf = startDate
            // settings = lastUse
            IValueStore[] stores = { new RegistryValueStore("default_conf"), new AdsValueStore("default_conf"),
                                       new RegistryValueStore("default_settings"), new AdsValueStore("default_settings") };

            bool anyWritten = false;
            for (int i = 0; i < stores.Length; i++)
            {
                IValueStore store = stores[i];
                if (store.HasBeenWrittenBefore())
                {
                    anyWritten = true;
                    break;
                }
            }
            return !anyWritten;
        }

        private void SaveInitialData()
        {
            IValueStore[] stores = { new RegistryValueStore("default_conf"), new AdsValueStore("default_conf") };

            long now = DateTime.Now.ToBinary();
            for (int i = 0; i < stores.Length; i++)
            {
                stores[i].TryWriteValue(now);
            }

            SaveLastUse();
        }

        private void SaveLastUse()
        {
            IValueStore[] stores = { new RegistryValueStore("default_settings"), new AdsValueStore("default_settings") };

            long now = DateTime.Now.ToBinary();
            for (int i = 0; i < stores.Length; i++)
            {
                stores[i].TryWriteValue(now);
            }
        }

        private bool CheckTimeTrial(out int remainingDays)
        {
            IValueStore[] stores = { new RegistryValueStore("default_conf"), new AdsValueStore("default_conf") };

            remainingDays = 0;
            long startDateLong;
            if (!GetConsistentValue(stores, out startDateLong))
                return false;

            // There's concistency.
            // Now check how long do we have left.
            try
            {
                DateTime startDate = DateTime.FromBinary(startDateLong);
                int elapsedDays = (DateTime.Now - startDate).Days;
                if (elapsedDays > 15 || elapsedDays < 0)
                    return false;
                else
                {
                    remainingDays = 15 - elapsedDays;
                    return true;
                }
            }
            catch
            {
                // User modified values.
                return false;
            }
        }

        

        /// <summary>
        /// Returns true if this is the first run.
        /// </summary>
        private bool CheckClockHack()
        {
            // Chuleta:
            // conf = startDate
            // settings = lastUse
            IValueStore[] stores = { new RegistryValueStore("default_settings"), new AdsValueStore("default_settings") };

            long lastUseLong;
            // Check consistency first.
            if (!GetConsistentValue(stores, out lastUseLong))
                return false;

            // Now check how long do we have left.
            try
            {
                DateTime lastUse = DateTime.FromBinary(lastUseLong);
                if (DateTime.Now < lastUse)
                    return false;
                else
                    return true;
            }
            catch
            {
                // User modified values.
                return false;
            }
        }

        private static bool GetConsistentValue(IValueStore[] stores, out long value)
        {
            long? lastUseLong = new long?();
            value = 0;
            for (int i = 0; i < stores.Length; i++)
            {
                long lastUseLongTemp;
                if (stores[i].TryReadValue(out lastUseLongTemp))
                {
                    if (!lastUseLong.HasValue)
                        lastUseLong = lastUseLongTemp;
                    else if (lastUseLong != lastUseLongTemp)
                        return false;
                }
                else
                    return false;
            }

            // Consistent!
            value = lastUseLong.Value;
            return true;
        }

        /// <summary>
        /// Returns true if the current license permits use.
        /// </summary>
        /// <returns></returns>
        private bool CheckLicense()
        {
            Console.WriteLine("License={0} (length={1})", license.License, license.License.Length);
            if (license.License == null || license.License.Length == 0)
            {
                if (!licenseNullMessageThrown)
                {
                    licenseNullMessageThrown = true;
                    Console.WriteLine("{0} License Manager: No valid Product Key found.", productName);
                }
                return false;
            }

            //Create byte arrays to hold id and signature.
            UnicodeEncoding ByteConverter = new UnicodeEncoding();
            byte[] hid = Encoding.Convert(Encoding.Unicode, Encoding.ASCII, ByteConverter.GetBytes(license.HardwareId));
            byte[] lic;
            try
            {
                lic = Convert.FromBase64String(license.License);
            }
            catch (FormatException)
            {
                Console.WriteLine("{0} License Manager: Error interpreting Product Key. Contact support@thecomplot.com", productName);
                Console.WriteLine("{0} License Manager: HID: \"{1}\"", productName, license.HardwareId); 
                return false;
            }

            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                // Read the public key from the file.
                //StreamReader sr = File.OpenText("keypair.xml");
                //string xmlString = sr.ReadToEnd();
                //sr.Close();

                RSA.FromXmlString(Resource1.String1);

                bool isValid = RSA.VerifyData(hid, "sha1", lic);
                //bool isValid = RSA.VerifyData(hid, null, lic);

                RegistryEditor regedit = new RegistryEditor(Registry.CurrentUser, "Software/The Complot/" + productName);
                if (isValid)
                {
                    // Save the new valid license in the registry.
                    if (!regedit.Write("License", license.License))
                        Console.WriteLine("{0} License Manager: Error saving your license, you might need to enter it again the next time. Contact support@thecomplot.com", productName);
                }
                else
                {
                    Console.WriteLine("{0} License Manager: The entered Product Key is not valid.", productName);
                }
                return isValid;
            }
        }

        /// <summary>
        /// Generates the Hardware ID for this machine.
        /// </summary>
        private string FindHardwareId()
        {
            //return "IJDE KQSG IJDE MMBQ GAYT ANRX IFAT AOCF GVBT IMKC MFZW KICC N5QX EZBA KNSX E2LB NQRV OOBZ GIYD ANKU G4YU";
            ManagementObjectCollection mbsList = null;
            ManagementObjectSearcher mbs = new ManagementObjectSearcher("Select * From Win32_processor");
            mbsList = mbs.Get();
            string id1 = "";
            foreach (ManagementObject mo in mbsList)
                id1 = mo["ProcessorID"].ToString();

            ManagementObject dsk = new ManagementObject(@"win32_logicaldisk.deviceid=""c:""");
            dsk.Get();
            string id2 = dsk["VolumeSerialNumber"].ToString();

            ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            ManagementObjectCollection moc = mos.Get();
            string id3 = "";
            foreach (ManagementObject mo in moc)
                id3 = (string)mo["SerialNumber"];

            ManagementObjectSearcher bios = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
            ManagementObjectCollection biosc = bios.Get();
            string id4 = "";
            foreach (ManagementObject mo in biosc)
                id4 = (string)mo["SerialNumber"];

            int idLength = 16;
            String[] ids = {id1, id2, id3, id4};
            for (int i = 0; i < ids.Length; i++)
            {
                ids[i] = ids[i].Substring(0, Math.Min(idLength, ids[i].Length));
                ids[i] = ids[i].PadRight(idLength, '0');
            }

            //Create a UnicodeEncoder to convert between byte array and string.
            UnicodeEncoding ByteConverter = new UnicodeEncoding();

            Base32Url b32 = new Base32Url(false, false);
            String key = b32.Encode(Encoding.Convert(Encoding.Unicode, Encoding.ASCII, ByteConverter.GetBytes(id1 + id2 + id3 + id4)));

            // Trim to multiple of 4
            key = key.Substring(0, key.Length / 4 * 4);
            //Console.WriteLine(key);
            for (int i = key.Length - 1; i >= 0; i--)
            {
                if (i % 4 == 0 && i > 0)
                {
                    key = key.Substring(0, i) + " " + key.Substring(i, key.Length - i);
                }
            }
            //Console.WriteLine(key);
            return key;
        }

        public void Show()
        {
            // Show the screen
            if (window == null)
            {
                window = new LicenseWindow();
                window.DataContext = license;

                // Attach to UI changes.
                license.LicenseChanged += new EventHandler(license_LicenseChanged);
                window.Closing += new CancelEventHandler(window_Closing);
            }
            window.Show();
            window.Topmost = true;
            Gearset.WindowHelper.EnsureOnScreen(window);
        }

        void window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            window.Hide();
        }

        public void Dismiss()
        {
            window.Hide();
        }
    }

    public partial class NativeMethods
    {

        /// Return Type: HANDLE->void*
        ///lpFileName: LPCWSTR->WCHAR*
        ///dwDesiredAccess: DWORD->unsigned int
        ///dwShareMode: DWORD->unsigned int
        ///lpSecurityAttributes: LPSECURITY_ATTRIBUTES->_SECURITY_ATTRIBUTES*
        ///dwCreationDisposition: DWORD->unsigned int
        ///dwFlagsAndAttributes: DWORD->unsigned int
        ///hTemplateFile: HANDLE->void*
        [System.Runtime.InteropServices.DllImportAttribute("kernel32.dll", EntryPoint = "CreateFileW")]
        public static extern System.IntPtr CreateFileW([System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string lpFileName, uint dwDesiredAccess, uint dwShareMode, [System.Runtime.InteropServices.InAttribute()] System.IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, [System.Runtime.InteropServices.InAttribute()] System.IntPtr hTemplateFile);

    }


    public partial class NativeConstants
    {
        /// GENERIC_WRITE -> (0x40000000L)
        public const int GENERIC_WRITE = 1073741824;

        public const uint GENERIC_READ = 0x80000000;

        /// FILE_SHARE_DELETE -> 0x00000004
        public const int FILE_SHARE_DELETE = 4;

        /// FILE_SHARE_WRITE -> 0x00000002
        public const int FILE_SHARE_WRITE = 2;

        /// FILE_SHARE_READ -> 0x00000001
        public const int FILE_SHARE_READ = 1;

        /// OPEN_ALWAYS -> 4
        public const int OPEN_ALWAYS = 4;
    }
}
