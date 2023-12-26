using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerOfHanoi
{
    internal class Peg
    {
        public int Order;
        public Point Start, End;
        public Color Color;
        public Peg(int Order, Point Start, Point End,Color Color) {
            this.Order = Order;
            this.Start = Start;
            this.End = End;
            this.Color = Color;
        }
    }
}
