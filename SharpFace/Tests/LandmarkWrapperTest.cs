﻿using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFace.Tests
{
    public class LandmarkWrapperTest : TestBase
    {
        LandmarkDetectorWrap wrap = new LandmarkDetectorWrap();
        CascadeClassifier cascade = new CascadeClassifier();
        VideoCapture capture;

        public LandmarkWrapperTest()
        {
            cascade.Load(wrap.Parameters.face_detector_location);

            wrap.Load();
            wrap.InVideo = true;
        }

        public override void Dispose()
        {
            if (wrap != null)
            {
                wrap.Dispose();
                wrap = null;
            }
        }

        public override int Run()
        {
            capture = new VideoCapture(0);

            int ret = InternalRun();

            Cv2.DestroyAllWindows();
            capture.Dispose();

            return ret;
        }

        private void DrawInROI(Mat mat, Rect roi, List<Point2d> pt)
        {
            foreach (var p in pt)
            {
                mat.DrawMarker((int)(roi.X + p.X), (int)(roi.Y + p.Y), Scalar.Red, MarkerStyle.CircleAndCross, 10, LineTypes.AntiAlias);
            }
        }

        private void DrawInfo(Mat mat, Point2d pt)
        {
            var rot = wrap.EularRotation;
            var pos = wrap.Transform;
            var fmt = "0.000";
            mat.PutText($"R:{rot[0].ToString(fmt)},{rot[1].ToString(fmt)},{rot[2].ToString(fmt)}", new Point(pt.X, pt.Y), HersheyFonts.HersheyPlain, 1, Scalar.Lime, 1, LineTypes.AntiAlias);
            mat.PutText($"T:{pos[0].ToString(fmt)},{pos[1].ToString(fmt)},{pos[2].ToString(fmt)}", new Point(pt.X, pt.Y+20), HersheyFonts.HersheyPlain, 1, Scalar.Lime, 1, LineTypes.AntiAlias);
        }

        private int InternalRun()
        {
            if (!capture.IsOpened())
                return -1;

            bool onFace = true;
            Mat read = new Mat();
            while (capture.Read(read))
            {
                if (!read.Empty())
                {
                    if (onFace)
                    {
                        wrap.DetectImage(read);
                        wrap.Draw(read);
                        DrawInfo(read, new Point2d(10, 20));
                    }
                    else
                    {
                        var faces = cascade.DetectMultiScale(read, 1.4, 2, HaarDetectionType.ScaleImage, new Size(read.Width * 0.2, read.Height * 0.2), read.Size());
                        foreach (var face in faces)
                        {
                            wrap.DetectROI(read, face);
                            wrap.Draw(read);
                            DrawInfo(read, face.TopLeft);
                            read.Rectangle(face, Scalar.Blue, 2, LineTypes.AntiAlias);
                        }
                    }
                    Cv2.ImShow("test", read);
                }

                char c = (char)Cv2.WaitKey(1);
                switch (c)
                {
                    case 'r':
                        wrap.Reset();
                        break;
                    case 'i':
                        wrap.InVideo = !wrap.InVideo;
                        break;
                    case 'f':
                        onFace = !onFace;
                        break;
                    case 'q':
                        return 0;
                }
            }
            return 0;
        }
    }
}
