using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HoneyCombTree
{
    public partial class FRM_main : Form
    {
        private readonly Color CURSORCOLOR = Color.Green;
        private readonly int CURSORRAD = 5, CURSORTHICKNESS = 2;
        private readonly int STALKWIDTH = 2;

        private List<Growth> growths;
        private Growth selected;
        private Point center;
        private List<Tuple<PointF, PointF, Color>> persistentLines;
        public FRM_main()
        {
            InitializeComponent();
        }

        private void FRM_main_Load(object sender, EventArgs e)
        {
            center = new Point(PB_main.Width / 2, PB_main.Height / 2);
            growths = new List<Growth>();
            //initial state
            growths.Add(new Growth(center, Growth.Direction.Up));
            growths.Add(new Growth(center, Growth.Direction.Down));
            selected = growths[1];
            persistentLines = new List<Tuple<PointF, PointF, Color>>();
        }

        private void TIM_main_Tick(object sender, EventArgs e)
        {
            foreach (Growth growth in growths)
            {
                growth.Step();
            }
            PB_main.Refresh();
        }

        private void FRM_main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                RotateSelection();
            }
            else if (e.KeyCode == Keys.Space)
            {
                Split();                
            }
        }

        private void Split()
        {
            growths.Add(new Growth(selected, true));
            growths.Add(new Growth(selected, false));
            persistentLines.Add(new Tuple<PointF, PointF, Color>(selected.Base, selected.Position, selected.Color));
            growths.Remove(selected);
            selected = growths[growths.Count - 1];
        }
        private void RotateSelection()
        {
            int index = -2;
            for (int i = 0; i < growths.Count; i++)
            {
                if (growths[i] == selected)
                {
                    index = i;
                }
            }
            index++;
            if (index == growths.Count)
            {
                index = 0;
            }
            selected = growths[index];
        }
        private void PB_main_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawEllipse(new Pen(new SolidBrush(CURSORCOLOR), CURSORTHICKNESS), 
                new Rectangle(Convert.ToInt32(selected.Position.X) - CURSORRAD, 
                Convert.ToInt32(selected.Position.Y) - CURSORRAD, 
                CURSORRAD * 2, CURSORRAD * 2));
            foreach (Growth growth in growths)
            {
                g.DrawLine(new Pen(new SolidBrush(growth.Color), STALKWIDTH), growth.Base, growth.Position);
            }
            foreach (Tuple<PointF, PointF, Color> line in persistentLines)
            {
                g.DrawLine(new Pen(new SolidBrush(line.Item3), STALKWIDTH), line.Item1, line.Item2);
            }
        }
    }
    public class Growth
    {
        public Direction MovementDirection;
        public PointF Position;
        public PointF Base;
        public Color Color;
        private readonly float GROWTHRATE = 0.1f;
        public enum Direction { Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft }
        public Growth(Growth parent, bool leftwards)
        {
            MovementDirection = NextDirection(parent.MovementDirection, leftwards);
            Position = parent.Position; Base = parent.Position;
            Color = Color.LimeGreen;
        }
        public Growth(Point _position, Direction _movementDirection)
        {
            Position = _position; MovementDirection = _movementDirection; Base = _position;
            Color = Color.LimeGreen;
        }
        private Direction NextDirection(Direction input, bool leftwards)
        {
            if (leftwards)
            {
                switch (input)
                {
                    case Direction.Up:
                        return Direction.UpLeft;
                    case Direction.UpLeft:
                        return Direction.Left;
                    case Direction.Left:
                        return Direction.DownLeft;
                    case Direction.DownLeft:
                        return Direction.Down;
                    case Direction.Down:
                        return Direction.DownRight;
                    case Direction.DownRight:
                        return Direction.Right;
                    case Direction.Right:
                        return Direction.UpRight;
                    case Direction.UpRight:
                        return Direction.Up;
                }
            }
            else
            {
                switch (input)
                {
                    case Direction.Up:
                        return Direction.UpRight;
                    case Direction.UpRight:
                        return Direction.Right;
                    case Direction.Right:
                        return Direction.DownRight;
                    case Direction.DownRight:
                        return Direction.Down;
                    case Direction.Down:
                        return Direction.DownLeft;
                    case Direction.DownLeft:
                        return Direction.Left;
                    case Direction.Left:
                        return Direction.UpLeft;
                    case Direction.UpLeft:
                        return Direction.Up;
                }
            }
            //this line should never be reached
            return Direction.Up;
        }
        public void Step()
        {
            switch (MovementDirection)
            {
                case Direction.Up:
                    Position.Y -= GROWTHRATE;
                    break;
                case Direction.UpRight:
                    Position.X += GROWTHRATE * Convert.ToSingle(Math.Pow(2, -0.5f));
                    Position.Y -= GROWTHRATE * Convert.ToSingle(Math.Pow(2, -0.5f));
                    break;
                case Direction.Right:
                    Position.X += GROWTHRATE;
                    break;
                case Direction.DownRight:
                    Position.X += GROWTHRATE * Convert.ToSingle(Math.Pow(2, -0.5f));
                    Position.Y += GROWTHRATE * Convert.ToSingle(Math.Pow(2, -0.5f));
                    break;
                case Direction.Down:
                    Position.Y += GROWTHRATE;
                    break;
                case Direction.DownLeft:
                    Position.X -= GROWTHRATE * Convert.ToSingle(Math.Pow(2, -0.5f));
                    Position.Y += GROWTHRATE * Convert.ToSingle(Math.Pow(2, -0.5f));
                    break;
                case Direction.Left:
                    Position.X -= GROWTHRATE;
                    break;
                case Direction.UpLeft:
                    Position.X -= GROWTHRATE * Convert.ToSingle(Math.Pow(2, -0.5f));
                    Position.Y -= GROWTHRATE * Convert.ToSingle(Math.Pow(2, -0.5f));
                    break;
            }
        }
    }
}
