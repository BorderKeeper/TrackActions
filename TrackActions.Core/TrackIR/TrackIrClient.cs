using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace TrackActions.Core.TrackIR
{
    public class TrackIrClient
    {
        private dNP_GetSignatureDelegate NP_GetSignatureDelegate;
        private dNP_RegisterWindowHandle NP_RegisterWindowHandle;
        private dNP_UnregisterWindowHandle NP_UnregisterWindowHandle;
        private dNP_RegisterProgramProfileID NP_RegisterProgramProfileID;
        private dNP_QueryVersion NP_QueryVersion;
        private dNP_RequestData NP_RequestData;
        private dNP_GetData NP_GetData;
        private dNP_UnregisterNotify NP_UnregisterNotify;
        private dNP_StartCursor NP_StartCursor;
        private dNP_StopCursor NP_StopCursor;
        private dNP_ReCenter NP_ReCenter;
        private dNP_StartDataTransmission NP_StartDataTransmission;
        private dNP_StopDataTransmission NP_StopDataTransmission;
        private ulong NPFrameSignature;
        private ulong NPStaleFrames;

        [DllImport("user32.dll")]
        private static extern int GetForegroundWindow();

        [DllImport("kernel32.dll")]
        private static extern int LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpLibFileName);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetProcAddress(int hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(int hModule);

        public string TrackIR_Enhanced_Init()
        {
            this.NPFrameSignature = 0UL;
            this.NPStaleFrames = 0UL;
            string dllPath = "";
            string str1 = "";
            this.GetDLLLocation(ref dllPath);
            int num1 = (int)this.NPClient_Init(dllPath);
            string str2;
            if (this.NPClient_Init(dllPath) == NPRESULT.NP_OK)
            {
                string str3 = str1 + "NPClient interface -- initialize OK\r\n";
                int foregroundWindow = GetForegroundWindow();
                string str4 = str3 + "ForegroundWindow handle: " + foregroundWindow.ToString() + "\r\n";
                if (this.NP_RegisterWindowHandle(foregroundWindow) == NPRESULT.NP_OK)
                {
                    string str5 = str4 + "NPClient : Window handle registration successful.\r\n";
                    ushort pwVersion = (ushort)0;
                    if (this.NP_QueryVersion(ref pwVersion) == NPRESULT.NP_OK)
                    {
                        string str6 = string.Format("NaturalPoint software version is " + string.Format("{0:d}", (object)((int)pwVersion >> 8)) + "." + ((object)string.Format("{0:d}", (object)((int)pwVersion & (int)byte.MaxValue))).ToString() + "\r\n");
                        string str7 = str5 + str6;
                        int num2 = (int)this.NP_RequestData((ushort)((uint)(0 | 2) | 4U | 1U | 16U | 32U | 64U));
                        int num3 = (int)this.NP_RegisterProgramProfileID((ushort)20430);
                        if (this.NP_StopCursor() == NPRESULT.NP_OK)
                        {
                            string str8 = str7 + "Cursor stopped\r\n";
                            if (this.NP_StartDataTransmission() == NPRESULT.NP_OK)
                                return str8 + "Data Transmission started\r\n";
                            str2 = str8 + "NPCLient : Error starting data transmission\r\n";
                            return (string)null;
                        }
                        else
                        {
                            str2 = str7 + "NPClient : Error stopping cursor\r\n";
                            return (string)null;
                        }
                    }
                    else
                    {
                        str2 = str5 + "NPClient : Error querying NaturalPoint software version!!\r\n";
                        return (string)null;
                    }
                }
                else
                {
                    str2 = str4 + "NPClient : Error registering window handle!!\r\n";
                    return (string)null;
                }
            }
            else
            {
                str2 = str1 + "Error initializing NPClient interface!!\r\n";
                return (string)null;
            }
        }

        public LPTRACKIRDATA client_HandleTrackIRData()
        {
            LPTRACKIRDATA pTID = new LPTRACKIRDATA();
            if (this.NP_GetData(ref pTID) != NPRESULT.NP_OK || (int)pTID.wNPStatus != 0)
                return pTID;
            if ((long)this.NPFrameSignature != (long)pTID.wPFrameSignature)
            {
                this.NPFrameSignature = (ulong)pTID.wPFrameSignature;
                this.NPStaleFrames = 0UL;
                return pTID;
            }
            else
            {
                if (this.NPStaleFrames > 30UL)
                    return pTID;
                ++this.NPStaleFrames;
                return pTID;
            }
        }

        public string client_TestTrackIRData()
        {
            LPTRACKIRDATA pTID = new LPTRACKIRDATA();
            string str = "";
            if (this.NP_GetData(ref pTID) == NPRESULT.NP_OK)
            {
                if ((int)pTID.wNPStatus == 0)
                {
                    if ((long)this.NPFrameSignature != (long)pTID.wPFrameSignature)
                    {
                        str = string.Concat(new object[4]
                        {
              (object) string.Concat(new object[4]
              {
                (object) string.Concat(new object[4]
                {
                  (object) string.Concat(new object[4]
                  {
                    (object) string.Concat(new object[4]
                    {
                      (object) string.Concat(new object[4]
                      {
                        (object) string.Concat(new object[4]
                        {
                          (object) string.Concat(new object[4]
                          {
                            (object) str,
                            (object) "Pitch: ",
                            (object) pTID.fNPPitch,
                            (object) "\r\n"
                          }),
                          (object) "Roll: ",
                          (object) pTID.fNPRoll,
                          (object) "\r\n"
                        }),
                        (object) "Yaw: ",
                        (object) pTID.fNPYaw,
                        (object) "\r\n"
                      }),
                      (object) "PosX: ",
                      (object) pTID.fNPX,
                      (object) "\r\n"
                    }),
                    (object) "PosY: ",
                    (object) pTID.fNPY,
                    (object) "\r\n"
                  }),
                  (object) "PosZ: ",
                  (object) pTID.fNPX,
                  (object) "\r\n"
                }),
                (object) "Information NPStatus = ",
                (object) pTID.wNPStatus,
                (object) "\r\n"
              }),
              (object) "Frame: ",
              (object) pTID.wPFrameSignature,
              (object) "\r\n"
                        });
                        this.NPFrameSignature = (ulong)pTID.wPFrameSignature;
                        this.NPStaleFrames = 0UL;
                    }
                    else if (this.NPStaleFrames > 30UL)
                    {
                        str = string.Concat(new object[4]
                        {
              (object) (str + "No New Data. Paused or Not Tracking?"),
              (object) "Information NPStatus = ",
              (object) pTID.wNPStatus,
              (object) "\r\n"
                        });
                    }
                    else
                    {
                        ++this.NPStaleFrames;
                        str = string.Concat(new object[4]
                        {
              (object) string.Concat(new object[4]
              {
                (object) str,
                (object) "No New Data for ",
                (object) this.NPStaleFrames,
                (object) " frames"
              }),
              (object) "Information NPStatus = ",
              (object) pTID.wNPStatus,
              (object) "\r\n"
                        });
                    }
                }
            }
            else
                str = str + "User Disabled";
            return str;
        }

        public string TrackIR_Shutdown()
        {
            string str1 = "";
            string str2 = this.NP_StopDataTransmission() != NPRESULT.NP_OK ? str1 + "StopDataTransmission() ERROR!!\r\n" : str1 + "StopDataTransmission() OK\r\n";
            string str3 = this.NP_StartCursor() != NPRESULT.NP_OK ? str2 + "StartCursor() ERROR!!\r\n" : str2 + "StartCursor() OK\r\n";
            return this.NP_UnregisterWindowHandle() != NPRESULT.NP_OK ? str3 + "UnregisterWindowHandle() ERROR!!\r\n" : str3 + "UnregisterWindowHandle() OK\r\n";
        }

        private NPRESULT NPClient_Init(string dllPath)
        {
            //LET THE SORCERY COMMENCE
            if (IntPtr.Size == 4) //32 bit
            {
                dllPath = dllPath + "NPClient.dll";
            }
            else if (IntPtr.Size == 8) //64 bit
            {
                dllPath = dllPath + "NPClient64.dll";
            }
            if (!File.Exists(dllPath))
                return NPRESULT.NP_ERR_DLL_NOT_FOUND;
            int hModule = LoadLibrary(dllPath);
            if (hModule == 0)
                return NPRESULT.NP_ERR_DLL_NOT_FOUND;
            this.NP_GetSignatureDelegate = (dNP_GetSignatureDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(hModule, "NP_GetSignature"), typeof(dNP_GetSignatureDelegate));
            LPTRACKIRSIGNATUREDATA signature = new LPTRACKIRSIGNATUREDATA();
            LPTRACKIRSIGNATUREDATA lptrackirsignaturedata = new LPTRACKIRSIGNATUREDATA();
            lptrackirsignaturedata.DllSignature = "precise head tracking\n put your head into the game\n now go look around\n\n Copyright EyeControl Technologies";
            lptrackirsignaturedata.AppSignature = "hardware camera\n software processing data\n track user movement\n\n Copyright EyeControl Technologies";
            NPRESULT npresult;
            if (this.NP_GetSignatureDelegate(ref signature) == NPRESULT.NP_OK)
            {
                if (string.Compare(lptrackirsignaturedata.DllSignature, signature.DllSignature) == 0 && string.Compare(lptrackirsignaturedata.AppSignature, signature.AppSignature) == 0)
                {
                    npresult = NPRESULT.NP_OK;
                    this.NP_RegisterWindowHandle = (dNP_RegisterWindowHandle)Marshal.GetDelegateForFunctionPointer(GetProcAddress(hModule, "NP_RegisterWindowHandle"), typeof(dNP_RegisterWindowHandle));
                    this.NP_UnregisterWindowHandle = (dNP_UnregisterWindowHandle)Marshal.GetDelegateForFunctionPointer(GetProcAddress(hModule, "NP_UnregisterWindowHandle"), typeof(dNP_UnregisterWindowHandle));
                    this.NP_RegisterProgramProfileID = (dNP_RegisterProgramProfileID)Marshal.GetDelegateForFunctionPointer(GetProcAddress(hModule, "NP_RegisterProgramProfileID"), typeof(dNP_RegisterProgramProfileID));
                    this.NP_QueryVersion = (dNP_QueryVersion)Marshal.GetDelegateForFunctionPointer(GetProcAddress(hModule, "NP_QueryVersion"), typeof(dNP_QueryVersion));
                    this.NP_RequestData = (dNP_RequestData)Marshal.GetDelegateForFunctionPointer(GetProcAddress(hModule, "NP_RequestData"), typeof(dNP_RequestData));
                    this.NP_GetData = (dNP_GetData)Marshal.GetDelegateForFunctionPointer(GetProcAddress(hModule, "NP_GetData"), typeof(dNP_GetData));
                    this.NP_StartCursor = (dNP_StartCursor)Marshal.GetDelegateForFunctionPointer(GetProcAddress(hModule, "NP_StartCursor"), typeof(dNP_StartCursor));
                    this.NP_StopCursor = (dNP_StopCursor)Marshal.GetDelegateForFunctionPointer(GetProcAddress(hModule, "NP_StopCursor"), typeof(dNP_StopCursor));
                    this.NP_ReCenter = (dNP_ReCenter)Marshal.GetDelegateForFunctionPointer(GetProcAddress(hModule, "NP_ReCenter"), typeof(dNP_ReCenter));
                    this.NP_StartDataTransmission = (dNP_StartDataTransmission)Marshal.GetDelegateForFunctionPointer(GetProcAddress(hModule, "NP_StartDataTransmission"), typeof(dNP_StartDataTransmission));
                    this.NP_StopDataTransmission = (dNP_StopDataTransmission)Marshal.GetDelegateForFunctionPointer(GetProcAddress(hModule, "NP_StopDataTransmission"), typeof(dNP_StopDataTransmission));
                }
                else
                    npresult = NPRESULT.NP_ERR_DLL_NOT_FOUND;
            }
            else
                npresult = NPRESULT.NP_ERR_DLL_NOT_FOUND;
            return npresult;
        }

        private void GetDLLLocation(ref string dllPath)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\NaturalPoint\\NATURALPOINT\\NPClient Location", false);
            dllPath = registryKey.GetValue("Path").ToString();
            registryKey.Close();
        }

        private delegate NPRESULT PF_NOTIFYCALLBACK(ushort a, ushort b);

        private delegate NPRESULT dNP_GetSignatureDelegate(ref LPTRACKIRSIGNATUREDATA signature);

        private delegate NPRESULT dNP_RegisterWindowHandle(int hWnd);

        private delegate NPRESULT dNP_RegisterProgramProfileID(ushort wPPID);

        private delegate NPRESULT dNP_UnregisterWindowHandle();

        private delegate NPRESULT dNP_QueryVersion(ref ushort pwVersion);

        private delegate NPRESULT dNP_RequestData(ushort wDataReq);

        private delegate NPRESULT dNP_GetData(ref LPTRACKIRDATA pTID);

        private delegate NPRESULT dNP_RegisterNotify(PF_NOTIFYCALLBACK pfNotify);

        private delegate NPRESULT dNP_UnregisterNotify();

        private delegate NPRESULT dNP_StartCursor();

        private delegate NPRESULT dNP_StopCursor();

        private delegate NPRESULT dNP_ReCenter();

        private delegate NPRESULT dNP_StartDataTransmission();

        private delegate NPRESULT dNP_StopDataTransmission();

        public struct LPTRACKIRSIGNATUREDATA
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 200)]
            public string DllSignature;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 200)]
            public string AppSignature;
        }

        public enum NPSTATUS
        {
            NPSTATUS_REMOTEACTIVE,
            NPSTATUS_REMOTEDISABLED,
        }

        public enum NPRESULT
        {
            NP_OK,
            NP_ERR_DEVICE_NOT_PRESENT,
            NP_ERR_UNSUPPORTED_OS,
            NP_ERR_INVALID_ARG,
            NP_ERR_DLL_NOT_FOUND,
            NP_ERR_NO_DATA,
            NP_ERR_INTERNAL_DATA,
        }

        public struct LPTRACKIRDATA
        {
            public ushort wNPStatus;
            public ushort wPFrameSignature;
            public uint dwNPIOData;
            public float fNPRoll;
            public float fNPPitch;
            public float fNPYaw;
            public float fNPX;
            public float fNPY;
            public float fNPZ;
            public float fNPRawX;
            public float fNPRawY;
            public float fNPRawZ;
            public float fNPDeltaX;
            public float fNPDeltaY;
            public float fNPDeltaZ;
            public float fNPSmoothX;
            public float fNPSmoothY;
            public float fNPSmoothZ;
        }
    }
}