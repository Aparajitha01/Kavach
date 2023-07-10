
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;





namespace DUMP_MASTER
{
    public partial class Form1 : Form
    {


        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, [In] ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("dbghelp.dll")]
        static extern bool MiniDumpWriteDump(IntPtr hProcess, uint processId, IntPtr hFile, int dumpType, IntPtr exceptionParam, IntPtr userStreamParam, IntPtr callbackParam);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct STARTUPINFO
        {
            public uint cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttribute;
            public uint dwFlags;
            public ushort wShowWindow;
            public ushort cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public uint dwProcessId;
            public uint dwThreadId;
        }


        public Form1()
        {
            InitializeComponent();
        }



        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                string dumpFileName = "memory_dump_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".dmp";
                int dumpType = 0x00000002; // MiniDumpWithFullMemory
                uint dwCreationFlags = 0x00000010; // CREATE_NEW_CONSOLE

                STARTUPINFO si = new STARTUPINFO();
                PROCESS_INFORMATION pi = new PROCESS_INFORMATION();

                if (CreateProcess(null, "cmd.exe", IntPtr.Zero, IntPtr.Zero, true, dwCreationFlags, IntPtr.Zero, null, ref si, out pi))
                {
                    using (FileStream fs = new FileStream(dumpFileName, FileMode.Create))
                    {
                        IntPtr hFile = fs.SafeFileHandle.DangerousGetHandle();

                        if (MiniDumpWriteDump(pi.hProcess, pi.dwProcessId, hFile, dumpType, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero))
                        {
                            MessageBox.Show("Memory dump saved to: " + dumpFileName);
                        }
                        else
                        {
                            MessageBox.Show("Failed to create memory dump.");
                        }
                    }


                    CloseHandle(pi.hProcess);
                    CloseHandle(pi.hThread);
                }
                else
                {
                    MessageBox.Show("Failed to create new process.");
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

    }
}
