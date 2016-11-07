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
        int arcCorner,arcDiameter;
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
            arcCorner = (int)(0.3 * Width);
            arcDiameter = Height - 2 * arcCorner;
            //sets width and height equal
            SetSquare();
            // sets hight quality of drawing
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            // checks if there is an invalidated region. If so just update the needle
            if (region != null)
                DrawNeedle(e.Graphics, arcDiameter / 2 - Width / 56);

            // the outer circle posed in 0.1 of the side of this control
            DrawOuterCircle(e.Graphics);
            // draw thicker circle
            DrawThickCircle(e.Graphics);
            //arc
            DrawInnerArc(e.Graphics);
            // draw divisions
            DrawDivisions(e.Graphics);
            // print digits
            DrawDigits(e.Graphics);
            //draw the needle
            DrawNeedle(e.Graphics, arcDiameter / 2 );
        }
        //return the rectangle whose corner resides at distance of margin from the corner of the control
        private Rectangle GetRectangle(int margin)
        {
            return new Rectangle(margin, margin, Width - 2 * margin, Width - 2 * margin);
        }
       void DrawNeedle(Graphics g,int r)
        {
            int arcThickness = Width / 28;
            GraphicsPath path = new GraphicsPath();
            // draw semicircle
            int penWidth = Width / 112;
            int radius = Width / 28;
            //draws the needle's semi circle (its base). will be drawn in path parameter
            DrawArc(radius,90, 180,path);
            
            path.AddLine(Width / 2, Height / 2 - radius, (Width / 2) + r - arcThickness, Height / 2);
            path.AddLine(Width / 2, Height / 2 + radius, (Width / 2) + r - arcThickness, Height / 2);
            // making a region to invalidate
            GraphicsPath pth = new GraphicsPath();
            DrawArc( r, 0, 360,pth);
            region = new Region(pth);
            
            //converts current value to its equivalent in degree  
            float omega = GetCurrentValueDegree();
            Matrix W = new Matrix();
            //transporting the needle to point (0,0) the origin 
            W.Translate(-Width / 2, -Height / 2);
            //rotation with amount of omega
            W.Rotate(omega,MatrixOrder.Append);
            //returning the needle to the center of the control
            W.Translate(Width / 2, Height / 2, MatrixOrder.Append);
            //gradient brush
            Point center = new Point(Width / 2 - Width/28 - 4, Height / 2);
            Point extrem = new Point(Width / 2 + r, Height / 2);
            Point[] points = new Point[2] { center, extrem };
            W.TransformPoints(points);
            LinearGradientBrush lgb = new LinearGradientBrush(points[0], points[1], needleBorderColor1, needleBorderColor2);
            path.Transform(W);
            Pen pen = new Pen(lgb);
            pen.Alignment = PenAlignment.Inset;
            //region.Transform(W);
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
        void DrawOuterCircle(Graphics g)
        {
            int p = (int)(0.1 * Height);
            System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                new Point(p, p), new Point(Width - p, Height - p), outerCircleColor1,
                outerCircleColor2);
            Pen pen = new Pen(brush);
            //rectangle that  in which the circle will be drawn 
            Rectangle rect = GetRectangle(p);
            g.DrawEllipse(pen, rect);
            brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Point(p, p),
                new Point(Height - p, Height - p), biggestCircleFillColor1,
                biggestCircleFillColor2);
            g.FillEllipse(brush, rect);

        }
        void DrawDivisions(Graphics g)
        {
            //h is the diameter of the circle. diameter is calculated from the center of the gauge to the center of the arc
            Pen pen = new Pen(divisionColor, 1);
            // the first division is on +45 degrees. this is the base
            double teta = Math.PI / 4;
            // the offset depends on division count
            double delta = (-3 * Math.PI / 2) / DivisionCount;

            //polar point with angle teta and magnitude . Pen width is Width/28
            PolarPoint pp1 = new PolarPoint( arcDiameter/ 2 - Width / 56, teta);
            PolarPoint pp2 = new PolarPoint(arcDiameter / 2 + Width / 56, teta);
            for (int i = 0; i <= DivisionCount; i++)
            {
                DrawLine(pp1, pp2, new Point(Width/2, Height/2), g, pen);
                pp1.Rotate(delta);
                pp2.Rotate(delta);
            }

        }
        void DrawDigits(Graphics g)
        {
            int fontSize = (int)Width / 35;
            Font fnt = new Font("Tahoma", fontSize, FontStyle.Bold);
            PolarPoint pp = new PolarPoint();
            pp.Phase = Math.PI / 4;
            pp.Magnitude = arcDiameter/2 + (int)Width / 18;
            Size size = new Size(4 * fontSize, 2 * fontSize);
            // offset
            double delta = -(3 * Math.PI / 2) / DivisionCount;
            int interval = (MaxValue - MinValue) / DivisionCount;
            Brush brsh = new SolidBrush(digitColor);
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;

            for (int i = 0; i <= DivisionCount; i++)
            {
                int Digit = MaxValue - i * interval;
                g.DrawString(Digit.ToString(), fnt, brsh, CreateRectangle(pp, size), format);
                pp.Rotate(delta);
            }

        }
        void DrawThickCircle(Graphics g)
        {
            //the corner is on 0.12 of the corner of the control
            int a = (int)(0.12 * Width);
            Rectangle rect2 = GetRectangle(a);

            // setting gradiant colors between upper left corner and lower right corner of the thick circle
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                new Point(a, a), new Point(Height - a, Height - a), innerThickCircleColor1,
                innerThickCircleColor2);
            Pen pen = new Pen(brush);
            pen.Width = Width / 34;
            g.DrawArc(pen, rect2, 0, 360);

        }
        void DrawInnerArc(Graphics g)
        {
            //the corner of the arc
            int p = (int)(0.3 * Width);
            Pen pen = new Pen(innerArcColor);
            pen.Width = Width / 28;
          
            Rectangle rect = GetRectangle(p);
            // from 135 degree by amount of 270
            g.DrawArc(pen, rect, 135, 270);
            // draws two narrow arcs around the thick arc
            pen.Width = 1;
            pen.Color = Color.FromArgb(255, 175, 183, 231);
            int u = p - (int)Width / 56;
            rect = GetRectangle(u);
            g.DrawArc(pen, rect, 135, 270);
            u = p + (int)Width / 56;
            rect = GetRectangle(u);
            g.DrawArc(pen, rect, 135, 270);


        }
        Rectangle CreateRectangle(PolarPoint pp,Size size)
        {
            // create a rectangle with size and the center on pp
            // transform to cartesian the coordination of the center of the rectangle
            Point CartPoint = ToCartesian(pp);
            // Transport to the center of coordination i.e. Height/2 and Width/2
            // A , B shows the translate necessary to move CartPoint to center (Width/2,Height/2)

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
        // makes width and height equal
        private void SetSquare()
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
