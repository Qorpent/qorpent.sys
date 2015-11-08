using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Qorpent;

namespace qorpent.IO {
    /// <summary>
    ///     Used for exclusive file locker for PID, deletes file after usage
    /// </summary>
    /// <example>
    ///     using (var lock = new FileLocker(".lock")) {
    ///     // if .lock file is not exists - FileLocker creates it  with PID as content with Write lock
    ///     if(lock.IsAquired()){  //require to check state of locked file - ctor of FileLocker never fire IO or lock
    ///     exceptions - if cannot lock IsAquired() will be false
    ///     }
    ///     } // if file was created be this FileLocker instance - it is deleted at end
    /// </example>
    /// <remarks>
    ///     Never use it against data files! - LockFile creates and deletes files that are just markers of locking
    ///     To prevent user data lost - LockFile uses <see cref="IsAllowedName" /> method to checkout if given file
    ///     is looks like locker file name
    ///     It checks if existed lock file is larger that created by LockFile and if it's name of existed Directory - it's
    ///     marks of possible data loss too
    /// </remarks>
    public class PidFile : IDisposable {
        public const int NOT_EXISTED_PROCESS = -1;
        public const int ERROR_ON_FILE_ACCESS = -2;
        public const int WAIT_TIMEOUT_STEP = 100;
        public const int WAIT_TIMEOUT_TOTAL = 10000;
        public const int MAX_PID_INT_SIZE = 8;
        public const string DEFAULT_PID_FILE = ".pid";

        public static readonly string[] ALLOWED_NAMES = {"tmp", "pid", "lock"};
        private readonly int _waittimeout;
        private FileStream str;


        /// <summary>
        ///     Creates new file locker
        /// </summary>
        /// <param name="filename">locker file name - if null or empty DEFAULT_PID_FILE in current directory will be used</param>
        /// <param name="mode">modes of LockFile</param>
        /// <param name="pid">PID - if Zero - will be PID of current process</param>
        /// <param name="waittimeout">total timeout to wait</param>
        public PidFile(string filename = DEFAULT_PID_FILE, PidFileMode mode = PidFileMode.Default, int pid = 0,
            int waittimeout = WAIT_TIMEOUT_TOTAL) {
            if (string.IsNullOrWhiteSpace(filename)) {
                filename = DEFAULT_PID_FILE;
            }
            if (!IsAllowedName(filename)) {
                throw new Exception("name of file is not save to use as lock");
            }
            Mode = mode;
            PidFilePath = EnvironmentInfo.ResolvePath(filename);

            ProcessId = pid;
            if (ProcessId == 0) {
                ProcessId = Process.GetCurrentProcess().Id;
            }
            Reset();
            if (!IsAquired && Mode.HasFlag(PidFileMode.Forced)) {
                ForseReset();
            }
            if (waittimeout <= WAIT_TIMEOUT_STEP) {
                waittimeout = WAIT_TIMEOUT_STEP*5;
            }
            _waittimeout = waittimeout;
            if (Mode.HasFlag(PidFileMode.Wait)) {
                Wait();
            }
            else if (!IsAquired && Mode.HasFlag(PidFileMode.ThrowNoLock)) {
                throw new Exception("cannot lock file");
            }
        }

        /// <summary>
        ///     Modes of LockFile given at creation
        /// </summary>
        public PidFileMode Mode { get; }

        /// <summary>
        ///     Full path to lock file
        /// </summary>
        public string PidFilePath { get; }

        /// <summary>
        ///     Used PID
        /// </summary>
        public int ProcessId { get; }

        /// <summary>
        ///     True if lock file successfully created and locked
        /// </summary>
        /// <returns></returns>
        public bool IsAquired { get; private set; }

        /// <summary>
        ///     At end of using frees opened locking stream (if opened) and if it was aquired before - deletes lock file
        /// </summary>
        public void Dispose() {
            str?.Close();
            if (!IsAquired) {
                return;
            }
            if (!File.Exists(PidFilePath)) {
                return;
            }
            try {
                File.Delete(PidFilePath);
            }
            catch {
                Thread.Sleep(WAIT_TIMEOUT_STEP);
                if (File.Exists(PidFilePath)) {
                    File.Delete(PidFilePath);
                }
            }
        }

        /// <summary>
        ///     Checkout if given file name can be used as LockFile
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>true if given path is valid to use as lock file name</returns>
        /// <remarks>
        ///     allowed names are: pid, tmp, lock, .pid, .tmp, .lock, *.pid, *.tmp, *.lock any case
        /// </remarks>
        public static bool IsAllowedName(string filename) {
            var name = Path.GetFileName(filename).ToLowerInvariant().Split('.').Last();
            return -1 != Array.IndexOf(ALLOWED_NAMES, name);
        }

        /// <summary>
        ///     Waits while locker file accpeted
        /// </summary>
        /// <param name="force">Forced check of current PID existence</param>
        /// <param name="waittimeout">Total wait timeout</param>
        /// <returns>True if file was Aquired prior timeout finish</returns>
        public bool Wait(bool force, int waittimeout) {
            if (waittimeout <= WAIT_TIMEOUT_STEP) {
                waittimeout = WAIT_TIMEOUT_STEP*5;
            }
            var limittime = DateTime.Now.AddMilliseconds(waittimeout);
            while (!IsAquired) {
                if (DateTime.Now > limittime) {
                    if (Mode.HasFlag(PidFileMode.ThrowNoLock)) {
                        throw new TimeoutException();
                    }
                    return false;
                }
                Thread.Sleep(WAIT_TIMEOUT_STEP);
                if (force) {
                    ForseReset();
                }
                else {
                    Reset();
                }
            }
            return true;
        }

        /// <summary>
        ///     Wait while lockfile could be Aquired with force, waittimeout given in ctor at creation
        /// </summary>
        /// <returns>True if file was Aquired prior timeout finish</returns>
        public bool Wait() {
            return Wait(Mode.HasFlag(PidFileMode.Forced), _waittimeout);
        }

        /// <summary>
        ///     Returns it's PID if IsAquired or reads saved PID if process exists
        /// </summary>
        /// <returns></returns>
        public int GetCurrentLockPid() {
            if (IsAquired) {
                return ProcessId;
            }
            if (!File.Exists(PidFilePath)) {
                return NOT_EXISTED_PROCESS;
            }


            using (var s = new FileStream(PidFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                //data loss protection
                if (s.Length > MAX_PID_INT_SIZE) {
                    throw new Exception("existed file too large for PID - user data loss possible");
                }
                using (var sr = new StreamReader(s)) {
                    var data = sr.ReadToEnd();
                    int pid;
                    if (int.TryParse(data, out pid)) {
                        return pid;
                    }
                    throw new Exception("data in locker file was not valid PID int - user data loss possible");
                }
            }
        }

        /// <summary>
        ///     If lock file not yet IsAquired and not exists - try to create and get lock over it
        /// </summary>
        public void Reset() {
            if (IsAquired) {
                return;
            }
            //data loss protection
            if (Directory.Exists(PidFilePath)) {
                throw new Exception("given lock file name is name of existed directory");
            }
            if (!File.Exists(PidFilePath)) {
                try {
                    Directory.CreateDirectory(Path.GetDirectoryName(PidFilePath));
                    str = new FileStream(PidFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.Read);

                    using (var sw = new StreamWriter(str, Encoding.ASCII, 8, true)) {
                        sw.Write(ProcessId.ToString());
                        sw.Flush();
                    }

                    IsAquired = true;
                }
                catch {
                }
            }
        }

        /// <summary>
        ///     If not IsAquired and file exists - additionally trys to check PID saved in file
        ///     - if it's really existed Process in system - if not - deletes file an Reset
        /// </summary>
        public void ForseReset() {
            if (IsAquired) {
                return;
            }
            if (!File.Exists(PidFilePath)) {
                Reset();
            }
            else {
                var pid = GetCurrentLockPid();
                try {
                    Process.GetProcessById(pid);
                }
                catch {
                    File.Delete(PidFilePath);
                    Reset();
                }
            }
        }
    }
}