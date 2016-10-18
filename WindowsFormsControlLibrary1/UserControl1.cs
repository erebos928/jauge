using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace WindowsFormsControlLibrary1
{
    public partial class CircularGauge_7: UserControl
    {
        static readonly Color outerCircleColor1 = Color.FromArgb(255, 114, 147, 159);
        static readonly Color outerCircleColor2 = Color.FromArgb(255, 178, 184, 196);
        static readonly Color biggestCircleFillColor1 = Color.FromArgb(255, 218, 226, 246);
        static readonly Color biggestCircleFillColor2 = Color.FromArgb(255, 178, 184, 196);
        static readonly Color innerThickCircleColor1 = Color.FromArgb(255, 117, 123, 137);
        static readonly Color innerThickCircleColor2 = Color.FromArgb(255, 195, 195, 195);
        static readonly Color innerArcColor = Color.FromArgb(255, 187, 195, 243);
        static readonly Color divisionColor = Color.FromArgb(200, 117, 123, 137);
        Color digitColor = Color.FromArgb(255, 121, 128, 138);
        Color needleColor1 = Color.FromArgb(255, 255, 127, 91);
        Color needleColor2 = Color.FromArgb(255, 253, 173, 149);
        Color needleBorderColor1 = Color.FromArgb(255, 184, 98, 74);
        Color needleBorderColor2 = Color.FromArgb(255, 253, 173, 149);
        static readonly public int MinDegree = 135;
        static readonly public int MaxDegree = 45;
        private int _divisionCount = 5;
        private int _maxValue = 100;
        private int _minValue = 0;
        public int MinValue { get {
                return _minValue;
            } set {
                if ((value < 0) || (value > _maxValue))
                    throw new ArgumentException("Invalid value");
                _minValue = value;
            } } 
        public int MaxValue { get {
                return _maxValue;
            } set {
                if ((value < 0) || (value <= _minValue))
                    throw new ArgumentException("Invalid value");
                _maxValue = value;
            } }
        
        public int DivisionCount {
            get
            {
                return _divisionCount;
            }
            set
            {
                if (value < 1)
                    throw new ArgumentException("Positive required");
                else
                _divisionCount = value;
            }
        }
        Region region;
        private double _cur;
        public double CurrentValue { get { return _cur; } set { _cur = value;
                if (region != null)
                Invalidate(region);
                Update();
            } }
        struct PolarPoint
        {

            public PolarPoint(double v1, double v2) : this()
            {
                Magnitude = v1;
                Phase = v2;
            }
           public void Rotate(double angle)
            {
                Phase = Phase + angle;
            }
            public double Magnitude { get; set; }
            public double Phase { get; set; }
        }
        public CircularGauge_7()
        {
            
            InitializeComponent();
            DoubleBuffered = true;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            int qq = (int)(0.3 * Width);
            int rr = Height - 2 * qq;
            if (region != null)
                DrawNeedle(e.Graphics, rr / 2 - Width / 56);
            SetPreferedSize();
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            // the outer circle posed in 0.1 of the side of this control
            int p = (int)(0.1 * Height);
            System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                new Point(p, p), new Point(Width - p , Height - p ), outerCircleColor1,
                outerCircleColor2);
            Pen pen = new Pen(brush);
            //rectangle that the circle in which will be drawn 
            Rectangle rect = GetRectangle(p);
            e.Graphics.DrawEllipse(pen, rect);
            brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Point(p, p), 
                new Point(Height - p, Height - p), biggestCircleFillColor1, 
                biggestCircleFillColor2);
            e.Graphics.FillEllipse(brush, rect);
            // the corner of thicker inner circle.
            int a = (int)(0.12 * Width);
            Rectangle rect2 = GetRectangle(a);

            // setting gradiant colors between upper left corner and lower right corner of the circle
            brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                new Point(a, a), new Point(Height - a, Height - a), innerThickCircleColor1,
                innerThickCircleColor2);
            pen = new Pen(brush);
            pen.Width = Width/34;
            e.Graphics.DrawArc(pen, rect2, 0, 360);
            //arc

            brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                new Point(a, a), new Point(Height - a, Height - a), innerArcColor,
                innerArcColor);
            pen = new Pen(brush);
            pen.Width = Width / 28;
            //the corner of the arc
            p = (int)(0.3 * Width);
            rect = GetRectangle(p);
            // from 135 degree by amount of 270
            e.Graphics.DrawArc(pen, rect, 135, 270);
            // draw divisions
            //h is the diameter of the circle
            int h = Height - 2 * p;
            pen.Color = divisionColor;
            pen.Width = 1;
            // the first division is on +45 degrees. this is the base
            double teta = Math.PI / 4;
            // the offset depends on division count
            double delta = (-3 * Math.PI / 2)/DivisionCount;
            
            //polar point with angle teta and magnitude . Pen width is Width/28
            PolarPoint pp1 = new PolarPoint(h/2 - Width/56, teta);
            PolarPoint pp2 = new PolarPoint(h/2 + Width/56, teta);
            for (int i = 0;i<= DivisionCount; i++)
            {
                DrawLine(pp1, pp2, new Point(p + h / 2, p + h / 2), e.Graphics, pen);
                pp1.Rotate(delta);
                pp2.Rotate(delta);
            }
            // draws two narrow arcs around the thick arc
            pen.Color = Color.FromArgb(255, 175, 183, 231);
            int u = p - (int)Width / 56 ;
            rect = GetRectangle(u);
            e.Graphics.DrawArc(pen, rect, 135, 270);
            u = p + (int)Width / 56;
            rect = GetRectangle(u);
            e.Graphics.DrawArc(pen, rect, 135, 270);
            // print digits
            int fontSize = (int)Width / 35;
            Font fnt = new Font("Tahoma", fontSize,FontStyle.Bold);
            pp2.Phase = Math.PI / 4;
            pp2.Magnitude += (int)Width / 18;
            Size size = new Size(4*fontSize, 2*fontSize);
            // offset
            delta = -(3 * Math.PI / 2) / DivisionCount;
            int interval = (MaxValue - MinValue) / DivisionCount;
            Brush brsh = new SolidBrush(digitColor);
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            
            for (int i = 0; i <= DivisionCount; i++)
            {
                int Digit = MaxValue - i * interval;
                e.Graphics.DrawString(Digit.ToString(), fnt, brsh, CreateRectangle(pp2, size),format);
                e.Graphics.DrawRectangle(pen, CreateRectangle(pp2, size));
                pp2.Rotate(delta);
            }


            DrawNeedle(e.Graphics, h / 2 - Width/56);
        }
        //return the rectangle whose corner resides at distance of margin from the corner of this control
        private Rectangle GetRectangle(int margin)
        {
            return new Rectangle(margin, margin, Width - 2 * margin, Width - 2 * margin);
        }
       void DrawNeedle(Graphics g,int r)
        {
            GraphicsPath path = new GraphicsPath();
            // draw semicircle
            int penWidth = Width / 112;
            int radius = Width / 28;
            int x = penWidth * r / radius;
            DrawArc(radius,90, 180,path);
            //int l = (int)Math.Sqrt( h * h - radius * radius);
            path.AddLine(Width / 2, Height / 2 - radius, (Width / 2) + r -x, Height / 2);
            path.AddLine(Width / 2, Height / 2 + radius, (Width / 2) + r - x, Height / 2);
            // making a region to invalidate
            GraphicsPath pth = new GraphicsPath();
            DrawArc( r, 0, 360,pth);
            region = new Region(pth);
            //g.DrawPath(Pens.Black, path);
            float omega = GetCurrentValueDegree();
            Matrix W = new Matrix();
            W.Translate(-Width / 2, -Height / 2);
            W.Rotate(omega,MatrixOrder.Append);
            W.Translate(Width / 2, Height / 2, MatrixOrder.Append);
            Point center = new Point(Width / 2 - Width/28 - 4, Height / 2);
            Point extrem = new Point(Width / 2 + r, Height / 2);
            Point[] points = new Point[2] { center, extrem };
            W.TransformPoints(points);
            LinearGradientBrush lgb = new LinearGradientBrush(points[0], points[1], needleBorderColor1, needleBorderColor2);
            path.Transform(W);
            Pen pen = new Pen(lgb);
            region.Transform(W);
            pen.Width = penWidth;
            g.DrawPath(pen,path);
            lgb = new LinearGradientBrush(points[0], points[1], needleColor1, needleColor2);
            g.FillPath(lgb, path);
           // g.FillRegion(Brushes.Black, region);
        }
        float GetCurrentValueDegree()
        {
            if (CurrentValue <= MinValue)
                return MinDegree;
            if (CurrentValue >= MaxValue)
                return MaxDegree;
            float ratio = (float)(CurrentValue - MinValue) / (MaxValue - MinValue);
            return 135 + ratio * 270;
        }
        Rectangle CreateRectangle(PolarPoint pp,Size size)
        {
            // create a rectangle with size and the center on pp
            // transform to cartesian the coordination of the center of the rectangle
            Point CartPoint = ToCartesian(pp);
            // Transport to the center of coordination i.e. Height/2 and Width/2
            // A , B shows the translate necessary to move CartPoint to center (Width/2,Height/2)

            /* int A = Width / 2 - CartPoint.X;
             int B = Height / 2 - CartPoint.Y;
             Point Corner = new Point();
             Corner.X = Width / 2 - size.Width/2;
             Corner.Y = Height / 2 + size.Height /2;
             // inverse translate
             Corner.X = Corner.X - A;
             Corner.Y = Corner.Y - B;*/
            CartPoint.X = CartPoint.X - size.Width/2;
            CartPoint.Y = CartPoint.Y - size.Height/2;
            return new Rectangle(CartPoint, size);
        }
        private Point Transport(Point point,PolarPoint v)
        {
            //cartesian vector
            Point cv = ToCartesian(v);
            Point result = new Point();
            //transport by cv
            result.X = point.X + cv.X;
            result.Y = point.Y + cv.Y;
            return result;
        }
        private Point ToCartesian(PolarPoint pp)
        {
            double x = pp.Magnitude * Math.Cos(pp.Phase) + Height / 2;
            double y = pp.Magnitude * Math.Sin(pp.Phase) + Width / 2;
            Point Cart_Point = new Point((int)x, (int)y);
            return Cart_Point;
        }
        private void DrawLine(PolarPoint p1, PolarPoint p2, Point center, Graphics g, Pen pen)
        {
            double x1 = p1.Magnitude * Math.Cos(p1.Phase) + center.X;
            double y1 = p1.Magnitude * Math.Sin(p1.Phase) + center.Y;
            double x2 = p2.Magnitude * Math.Cos(p2.Phase) + center.X;
            double y2 = p2.Magnitude * Math.Sin(p2.Phase) + center.Y;
            Point Cart_Point1 = new Point((int)x1, (int)y1);
            Point Cart_Point2 = new Point((int)x2, (int)y2);
            g.DrawLine(pen, Cart_Point1, Cart_Point2);
        }
        // all circles one center
        private void DrawCircle(int Radius,Graphics g,Pen pen)
        {
            
            int x = Height / 2 - Radius;
            int y = Width / 2 - Radius;
            g.DrawEllipse(pen, new Rectangle(x, y, Radius * 2, Radius * 2));
        }
        private void DrawArc(int Radius,float From, float To, Graphics g, Pen pen)
        {

            int x = Height / 2 - Radius;
            int y = Width / 2 - Radius;
            g.DrawArc(pen, new Rectangle(x, y, Radius * 2, Radius * 2), From, To);
        }
        private void DrawArc(int Radius, float From, float To, GraphicsPath path)
        {

            int x = Height / 2 - Radius;
            int y = Width / 2 - Radius;
            path.AddArc(new Rectangle(x, y, Radius * 2, Radius * 2), From, To);
            
        }
        // makes them equal
        private void SetPreferedSize()
        {
            if (Height != Width)
                if (Height > Width)
                    Width = Height;
                else
                    Height = Width;
             Width = Height;
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {

        }
    }
}
