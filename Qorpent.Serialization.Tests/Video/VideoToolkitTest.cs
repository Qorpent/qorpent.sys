using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Utils;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Qorpent.Video;

namespace Qorpent.Serialization.Tests.Video
{
    [TestFixture]
    [Explicit]
    public class VideoToolkitTest
    {
        private string dir;

    

        [SetUp]
        public void Setup() {
            dir = Path.Combine(Path.GetTempPath(), "VideoToolkitTest");
            Directory.CreateDirectory(dir);
        }

        string GetPath(string file) {
            return Path.Combine(dir, file);
        }

        [Test]
        public void CanConvert() {
            FileSystemHelper.DeleteIfExists(GetPath("slideshow-converted.mp4"));
            FileSystemHelper.DeleteIfExists(GetPath("slideshow-converted.avi"));
            MakeSlideShow();
            var convertTask = new FFMpegConvertTask
            {
                SourceFile = GetPath("slideshow.mp4"),
                TargetFile = GetPath("slideshow-converted.avi")
            };
            FFMpeg.Convert(convertTask);
            convertTask = new FFMpegConvertTask {
                SourceFile = GetPath("slideshow-converted.avi"),
                TargetFile = GetPath("slideshow-converted.mp4")
            };
            FFMpeg.Convert(convertTask);
            Assert.True(File.Exists(convertTask.TargetFile));
            Assert.Greater(new FileInfo(convertTask.SourceFile).Length, new FileInfo(convertTask.TargetFile).Length);
        }

        [Test]
        public void CanSplit() {
            FileSystemHelper.DeleteIfExists(Path.Combine(dir, "basis_01.mp4"));
            FileSystemHelper.DeleteIfExists(Path.Combine(dir, "basis_02.mp4"));
            FileSystemHelper.DeleteIfExists(Path.Combine(dir, "basis_03.mp4"));
            FileSystemHelper.DeleteIfExists(Path.Combine(dir, "basis_04.mp4"));
            var slidetask = MakeSlideShow("long-slideshow.mp4",0.1m);
            var splittask = new FFMpegVideoSplitTask {SourceFile = slidetask.TargetFile, WorkingFolder = dir};
            splittask.Frames.Add(new FFMpegFrame{StartTime = TimeSpan.FromSeconds(1),Duration = TimeSpan.FromSeconds(1)});
            splittask.Frames.Add(new FFMpegFrame{StartTime = TimeSpan.FromSeconds(11),Duration = TimeSpan.FromSeconds(2)});
            splittask.Frames.Add(new FFMpegFrame{StartTime = TimeSpan.FromSeconds(1),Duration = TimeSpan.FromSeconds(0.5)});
            splittask.Frames.Add(new FFMpegFrame{StartTime = TimeSpan.FromSeconds(12),Duration = TimeSpan.FromSeconds(2)});
            FFMpeg.Split(splittask);
            Assert.True(File.Exists(GetPath("basis_01.mp4")));
            Assert.True(File.Exists(GetPath("basis_02.mp4")));
            Assert.True(File.Exists(GetPath("basis_03.mp4")));
            Assert.True(File.Exists(GetPath("basis_04.mp4")));
        }

        [Test]
        public void CanSplitImages()
        {
            FileSystemHelper.DeleteIfExists(Path.Combine(dir, "thumb_01.png"));
            FileSystemHelper.DeleteIfExists(Path.Combine(dir, "thumb_02.png"));
            FileSystemHelper.DeleteIfExists(Path.Combine(dir, "thumb_03.png"));
            FileSystemHelper.DeleteIfExists(Path.Combine(dir, "thumb_04.png"));
            var slidetask = MakeSlideShow("long-slideshow.mp4", 0.1m);
            var splittask = new FFMpegVideoSplitTask { SourceFile = slidetask.TargetFile, WorkingFolder = dir,FileNameBase = "thumb_"};
            splittask.Frames.Add(new FFMpegFrame { StartTime = TimeSpan.FromSeconds(1), Duration = TimeSpan.FromSeconds(1) });
            splittask.Frames.Add(new FFMpegFrame { StartTime = TimeSpan.FromSeconds(11), Duration = TimeSpan.FromSeconds(2) });
            splittask.Frames.Add(new FFMpegFrame { StartTime = TimeSpan.FromSeconds(1), Duration = TimeSpan.FromSeconds(0.5) });
            splittask.Frames.Add(new FFMpegFrame { StartTime = TimeSpan.FromSeconds(12), Duration = TimeSpan.FromSeconds(2) });
            splittask.Width = 100;
            splittask.Height = 100;
            FFMpeg.SplitImages(splittask);
            Assert.True(File.Exists(GetPath("thumb_01.png")));
            Assert.True(File.Exists(GetPath("thumb_02.png")));
            Assert.True(File.Exists(GetPath("thumb_03.png")));
            Assert.True(File.Exists(GetPath("thumb_04.png")));
        }

        [Test]
        public void CanJoin() {
            FileSystemHelper.DeleteIfExists(Path.Combine(dir, "joined.mp4"));
            FileSystemHelper.DeleteIfExists(Path.Combine(dir, "join-config.tmp"));
            CanSplit();
            var jointask = new FFMpegVideoJoinTask {
                JoinFileName = GetPath("join-config.tmp"),
                TargetFile = Path.Combine(dir, "joined.mp4"),
                WorkingFolder = dir
            };
            FFMpeg.Join(jointask);
            Assert.True(File.Exists(GetPath("joined.mp4")));
            Assert.True(File.Exists(GetPath("join-config.tmp")));
        }

        [Test]
        public void CanMakeMovie() {
           
            FileSystemHelper.DeleteIfExists(Path.Combine(dir, "dirty-movie.avi"));
            FileSystemHelper.DeleteIfExists(Path.Combine(dir, "dirty-movie.mp4"));
             FileSystemHelper.DeleteIfExists(Path.Combine(dir, "frame_01.mp4"));
            FileSystemHelper.DeleteIfExists(Path.Combine(dir, "frame_rawmovie.mp4"));
            FileSystemHelper.DeleteIfExists(Path.Combine(dir, "frame_02.mp4"));
            FileSystemHelper.DeleteIfExists(Path.Combine(dir, "frame_03.mp4"));
            FileSystemHelper.DeleteIfExists(Path.Combine(dir, "frame_04.mp4"));
            FileSystemHelper.DeleteIfExists(Path.Combine(dir, "movie.mp4"));
            FileSystemHelper.DeleteIfExists(Path.Combine(dir, "frame_movieset.tmp"));
            // we cannot make avi directly but want to show ability to make movie from avi
            MakeSlideShow("dirty-movie.mp4",0.1m);
            FFMpeg.Convert(new FFMpegConvertTask{SourceFile = GetPath("dirty-movie.mp4"),TargetFile = GetPath("dirty-movie.avi"),});



            var task = new FFMpegCreateMovieTask {
                WorkingFolder = dir,
                TmpFileBasis = "frame_",
                SourceFile = GetPath("dirty-movie.avi"),
                TargetFile = GetPath("movie.mp4"),

            };
            task.Frames.Add(new FFMpegFrame { StartTime = TimeSpan.FromSeconds(1), Duration = TimeSpan.FromSeconds(0.5) });
            task.Frames.Add(new FFMpegFrame { StartTime = TimeSpan.FromSeconds(11), Duration = TimeSpan.FromSeconds(0.5) });
            task.Frames.Add(new FFMpegFrame { StartTime = TimeSpan.FromSeconds(1), Duration = TimeSpan.FromSeconds(0.5) });
            task.Frames.Add(new FFMpegFrame { StartTime = TimeSpan.FromSeconds(12), Duration = TimeSpan.FromSeconds(0.5) });
            FFMpeg.CreateMovie(task);
            Assert.True(File.Exists(GetPath("movie.mp4")));
            Assert.True(File.Exists(GetPath("frame_movieset.tmp")));


        }

        [Test]
        public void CanCreateSlideShow()
        {
           
            var slidetask = MakeSlideShow();
            Assert.True(File.Exists(slidetask.TargetFile));
            Assert.Greater(8000, new FileInfo(slidetask.TargetFile).Length);

        }
       

        private FFMpegSlideShowTask MakeSlideShow(string file = "slideshow.mp4", decimal slidesPerSecond= 0.5m) {
            InitSlideFiles();
            FileSystemHelper.DeleteIfExists(Path.Combine(dir, file));

            var slidetask = new FFMpegSlideShowTask {
                TargetFile = Path.Combine(dir, file),
                WorkingFolder = dir,
                SlidesPerSecond = slidesPerSecond
            };
            FFMpeg.CreateSlideShow(slidetask);
            return slidetask;
        }

        private void InitSlideFiles() {
            FileSystemHelper.DeleteIfExists(Path.Combine(dir, "img_0001.png"));
            FileSystemHelper.DeleteIfExists(Path.Combine(dir, "img_0002.png"));
            var bmp = new Bitmap(40, 40);
            using (Graphics gfx = Graphics.FromImage(bmp)) {
                using (SolidBrush brush = new SolidBrush(Color.Red)) {
                    gfx.FillRectangle(brush, 0, 0, 40, 40);
                }
                bmp.Save(Path.Combine(dir, "img_0001.png"), ImageFormat.Png);
                using (SolidBrush brush = new SolidBrush(Color.Green)) {
                    gfx.FillRectangle(brush, 0, 0, 40, 40);
                }
                bmp.Save(Path.Combine(dir, "img_0002.png"), ImageFormat.Png);
            }
        }
    }
}
