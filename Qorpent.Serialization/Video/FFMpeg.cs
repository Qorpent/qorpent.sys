using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Schema;
using Qorpent.Utils;

namespace Qorpent.Video
{

    public class FFMpegFrame {
        public TimeSpan StartTime { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public class FFMpegVideoJoinTask {
        public FFMpegVideoJoinTask() {
            FileNameBase = "basis_";
            DigitCount = 2;
        }
        public string WorkingFolder { get; set; }
        public string FileNameBase { get; set; }
        public int DigitCount { get; set; }
        public string TargetFile { get; set; }
        public string[] FileList { get; set; }
        public string JoinFileName { get; set; }
    }

    public class FFMpegSlideShowTask
    {
        public FFMpegSlideShowTask()
        {
            FileNameBase = "img_";
            DigitCount = 4;
            SlidesPerSecond = 0.2m;
            Height = 576;
            Width = 720;
            Fps = 25;
        }
        public string WorkingFolder { get; set; }
        public int Fps { get; set; }
        public string FileNameBase { get; set; }
        public int DigitCount { get; set; }
        public string TargetFile { get; set; }
        public decimal SlidesPerSecond { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }


    public class FFMpegVideoSplitTask
    {
        private IList<FFMpegFrame> _frames;

        public FFMpegVideoSplitTask()
        {
            FileNameBase = "basis_";
            DigitCount = 2;
            Height = 576;
            Width = 720;
        }
        public string WorkingFolder { get; set; }
        public string FileNameBase { get; set; }
        public int DigitCount { get; set; }
        public string SourceFile { get; set; }
        public IList<FFMpegFrame> Frames
        {
            get { return _frames ?? (_frames = new List<FFMpegFrame>()); }
            set { _frames = value; }
        }
        public int Height { get; set; }
        public int Width { get; set; }
    }

    public class FFMpegConvertTask {
        private IList<FFMpegFrame> _frames;

        public FFMpegConvertTask()
        {
            StartTime = TimeSpan.FromSeconds(0);
            Duration = TimeSpan.FromHours(5);
            Height = 576;
            Width = 720;
        }
        public string SourceFile { get; set; }
        public string TargetFile { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

    }

    public class FFMpegCreateMovieTask {
        private IList<FFMpegFrame> _frames;

        public FFMpegCreateMovieTask() {
            StartTime = TimeSpan.FromSeconds(0);
            Duration = TimeSpan.FromHours(5);
            Height = 576;
            Width = 720;
            TmpFileBasis = "basis_";
        }
        public string Id { get; set; }
        public string WorkingFolder { get; set; }
        public string SourceFile { get; set; }
        public string TargetFile { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string TmpFileBasis { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        public IList<FFMpegFrame> Frames {
            get { return _frames ??(_frames=new List<FFMpegFrame>()); }
            set { _frames = value; }
        }

        public void Normalize() {
            if (string.IsNullOrWhiteSpace(Id)) {
                Id = "FFC" + DateTime.Now.ToString("YYYYMMDDHHmmss");
            }
            if (string.IsNullOrWhiteSpace(WorkingFolder)) {
                WorkingFolder = Path.Combine(Path.GetTempPath(), Id);
                
            }
            if (Duration.TotalMilliseconds < 300) {
                Duration = TimeSpan.FromHours(5);
            }
            
        }

        public FFMpegCreateMovieTask Clone() {
            var result = (FFMpegCreateMovieTask) MemberwiseClone();
            result.Frames= new List<FFMpegFrame>();
            foreach (var ffMpegFrame in this.Frames) {
                result.Frames.Add(ffMpegFrame);
            }
            return result;

        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class FFMpeg
    {
        

        public static void CreateMovie(FFMpegCreateMovieTask task) {
            task.Normalize();
            Directory.CreateDirectory(task.WorkingFolder);
            var convert = new FFMpegConvertTask();
            convert.SourceFile = task.SourceFile;
            convert.Height = task.Height;
            convert.Width = task.Width;
            convert.StartTime = task.StartTime;
            convert.Duration = task.Duration;
            convert.TargetFile = Path.Combine(task.WorkingFolder, task.TmpFileBasis+"rawmovie.mp4");
            Convert(convert);
            var splitter = new FFMpegVideoSplitTask();
            splitter.SourceFile = convert.TargetFile;
            splitter.Frames = task.Frames;
            splitter.FileNameBase = task.TmpFileBasis;
            splitter.WorkingFolder = task.WorkingFolder;
            Split(splitter);
            var joiner = new FFMpegVideoJoinTask();
            joiner.TargetFile = task.TargetFile;
            joiner.FileNameBase = task.TmpFileBasis;
            joiner.JoinFileName = task.TmpFileBasis + "movieset.tmp";
            joiner.WorkingFolder = task.WorkingFolder;
            Join(joiner);
        }

        public static void CreateSlideShow(FFMpegSlideShowTask joiner) {
            var handler = new ConsoleApplicationHandler();
            handler.ExePath = EnvironmentInfo.GetExecutablePath("ffmpeg");
            handler.WorkingDirectory = joiner.WorkingFolder;
            var sb = new StringBuilder();
            int rate = (int) joiner.SlidesPerSecond;
            string ratestr = rate.ToString();
            if (joiner.SlidesPerSecond < 1) {
                rate = System.Convert.ToInt32(1/joiner.SlidesPerSecond);
                ratestr = "1/" + rate;
            }
            sb.Append(" -framerate ");
            sb.Append(ratestr);
            sb.Append(" -i ");
            sb.Append(joiner.FileNameBase);
            sb.Append("%0");
            sb.Append(joiner.DigitCount);
            sb.Append("d.png");
            sb.Append(" -c:v libx264 -y ");
            var fps = joiner.Fps.ToString();
            if (0 == joiner.Fps) fps = "25";
            sb.Append(" -r ");
            sb.Append(fps);
            sb.Append(" -pix_fmt yuv420p ");
            if (joiner.Height != 0 && joiner.Width != 0) {
                sb.Append(" -s ");
                sb.Append(joiner.Width);
                sb.Append("x");
                sb.Append(joiner.Height);
            }
            sb.Append(" \"");
            sb.Append(joiner.TargetFile);
            sb.Append("\"");
            handler.Arguments = sb.ToString();
            var result = handler.RunSync();
            if (!result.IsOK) {
                throw new Exception(result.Output+result.Error,result.Exception);
            }
        }

        public static void Join(FFMpegVideoJoinTask joiner) {

            var joinfile = CreateConcatFile(joiner);
            var handler = new ConsoleApplicationHandler();
            handler.ExePath = EnvironmentInfo.GetExecutablePath("ffmpeg");
            var sb = new StringBuilder();
            sb.Append(" -f concat");
            sb.Append(" -i \"" + joinfile + "\" ");
            sb.Append(" -c copy ");
            sb.Append(" \"");
            sb.Append(joiner.TargetFile);
            sb.Append("\"");
            handler.Arguments = sb.ToString();
            var result = handler.RunSync();
            if (!result.IsOK)
            {
                throw new Exception(result.Output+result.Error,result.Exception);
            }
        }

        private static string CreateConcatFile(FFMpegVideoJoinTask joiner) {
            var dir = joiner.WorkingFolder;
            var tmpfile = joiner.JoinFileName;
            if (string.IsNullOrWhiteSpace(tmpfile)) {
                if (!string.IsNullOrWhiteSpace(dir)) {
                    tmpfile = Path.Combine(dir, "join-config.tmp");
                    if (File.Exists(tmpfile)) {
                        tmpfile = Path.Combine(dir, "join-config-" + Environment.TickCount + ".tmp");
                    }
                }
                else {
                    tmpfile = Path.GetTempFileName();
                }
            }
            else {
                if (!Path.IsPathRooted(tmpfile)) {
                    if (string.IsNullOrWhiteSpace(dir)) {
                        dir = Environment.CurrentDirectory;
                    }
                    tmpfile = Path.Combine(dir, tmpfile);
                }
            }
            if (null != joiner.FileList && 0 != joiner.FileList.Length) {
                File.AppendAllLines(tmpfile, joiner.FileList.Select(_ => "file " + _.Replace("\\", "/")));
            }
            else {
                if (string.IsNullOrWhiteSpace(dir)) {
                    dir = Environment.CurrentDirectory;
                }
                var files = Directory.GetFiles(dir, joiner.FileNameBase + "*.mp4");
                files =
                    files.Where(
                        _ =>
                            Regex.IsMatch(Path.GetFileNameWithoutExtension(_),
                                joiner.FileNameBase + "\\d{" + joiner.DigitCount + "}")).ToArray();
                files = files.OrderBy(_ => Path.GetFileNameWithoutExtension(_)).ToArray();
                File.AppendAllLines(tmpfile,files.Select(_=>"file "+_.Replace("\\","/")));
            }
            return tmpfile;
        }

        public static void Split(FFMpegVideoSplitTask splitter) {
            if (string.IsNullOrWhiteSpace(splitter.WorkingFolder))
            {
                splitter.WorkingFolder = Path.GetDirectoryName(splitter.SourceFile);
            }
            Directory.CreateDirectory(splitter.WorkingFolder);
            for (var i = 0; i < splitter.Frames.Count; i++) {
                var frame = splitter.Frames[i];
                var num = (i + 1).ToString("00");
                var filename = splitter.FileNameBase + num + ".mp4";
                var convert = new FFMpegConvertTask {
                    SourceFile = splitter.SourceFile,
                    TargetFile = Path.Combine(splitter.WorkingFolder, filename),
                    StartTime = frame.StartTime,
                    Duration = frame.Duration,
                    Width = splitter.Width,
                    Height = splitter.Height
                };
                Convert(convert);
            }
        }


        public static void SplitImages(FFMpegVideoSplitTask splitter)
        {
            if (string.IsNullOrWhiteSpace(splitter.WorkingFolder)) {
                splitter.WorkingFolder = Path.GetDirectoryName(splitter.SourceFile);
            }
            Directory.CreateDirectory(splitter.WorkingFolder);
            for (var i = 0; i < splitter.Frames.Count; i++)
            {
                var frame = splitter.Frames[i];
                var num = (i + 1).ToString("00");
                var filename = splitter.FileNameBase + num + ".png";
                if (splitter.DigitCount == 0 && splitter.Frames.Count==1) {

                    filename = splitter.FileNameBase+ ".png";
                }
                filename = Path.Combine(splitter.WorkingFolder, filename);
                var handler = new ConsoleApplicationHandler();
                handler.ExePath = EnvironmentInfo.GetExecutablePath("ffmpeg");
                var sb = new StringBuilder();
                sb.Append(" -ss ");
                sb.Append(frame.StartTime);
                sb.Append(" -i ");
                sb.Append("\"");
                sb.Append(splitter.SourceFile);
                sb.Append("\" ");
                sb.Append(" -vframes 1 ");
                if (splitter.Height != 0 && splitter.Width != 0)
                {
                    sb.Append(" -s ");
                    sb.Append(splitter.Width);
                    sb.Append("x");
                    sb.Append(splitter.Height);
                }
                sb.Append(" -y -f image2 ");
                sb.Append("\"");
                sb.Append(filename);
                sb.Append("\"");
                handler.Arguments = sb.ToString();
                var result = handler.RunSync();
                if (!result.IsOK)
                {
                    throw new Exception(result.Output+result.Error, result.Exception);
                }
            }
        }

        public static void Convert(FFMpegConvertTask task)
        {
            var handler = new ConsoleApplicationHandler();
            handler.ExePath = EnvironmentInfo.GetExecutablePath("ffmpeg");

            var sb = new StringBuilder();
            sb.Append(" -i ");
            sb.Append(task.SourceFile);
            sb.Append(" -acodec aac -strict -2 -b:a 128k -vcodec libx264 -b:v 1200k -y ");
            if (task.Height != 0 && task.Width != 0)
            {
                sb.Append(" -s ");
                sb.Append(task.Width);
                sb.Append("x");
                sb.Append(task.Height);
            }
            sb.Append(" -ss ");
            sb.Append(task.StartTime);
            sb.Append(" -t ");
            if (task.Duration.TotalMilliseconds < 100) {
                sb.Append("05:00:00");
            }
            else {
                sb.Append(task.Duration);
            }
            sb.Append(" -flags +aic+mv4 ");
            sb.Append(" \"");
            sb.Append(task.TargetFile);
            sb.Append("\"");
            handler.Arguments = sb.ToString();
            var result = handler.RunSync();
            if (!result.IsOK)
            {
                throw new Exception(result.Output+result.Error, result.Exception);
            }
        }

        
    }
}
