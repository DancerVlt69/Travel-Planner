
using System.Runtime.CompilerServices;
using System.Windows.Documents;

namespace Schach
{    
    class Point
    {
        public Point(int x, int y)
        {
            this.x = x; this.y = y;
        }

        int x, y; // Spalte, Zeile 0..7, unten links Zeile 0, Spalte 0
        public int X { get { return x; } }
        public int Y { get { return y; } }
        public bool ComparePoints (Point point)
        {
            return X == point.X && X == point.Y;
        }
    }

    abstract class ShessPiece
    {
        protected Point point;
        public Point Position { get { return point; } }

        //protected int x, y;

        //public int getX { get { return x; } }

        //public int getY { get { return y; } }

        protected char form;
        public char getForm { get { return form; } }

        protected bool isWhite;

        public abstract bool isMovePossible(Point pTarget);     

        public ShessPiece(Point point, bool isWhite, char form)
        {
            this.point = point;
            this.isWhite = isWhite;
            this.form = form;
        }
        
        public bool moveTo(Point pTarget)
        {
            if(isMovePossible(pTarget))
            {
              point = pTarget; return true;
            }
            return false;
        }
    }

    class Rock : ShessPiece // Turm
    {
        public Rock(Point point, bool isWhite) :
            base(point, isWhite, isWhite ? '\u2656' : '\u265C')
        {
        
        }
        
        public override bool isMovePossible(Point pTarget)
        {

            return pTarget.X == point.X || pTarget.Y == point.Y;
            
        }
    }

    class Bishop : ShessPiece // Läufer
    {
        public Bishop(Point point, bool isWhite) :
            base(point, isWhite, isWhite ? '\u2657' : '\u265D')
        {

        }

        public override bool isMovePossible(Point pTarget)
        {

            return pTarget.Y == point.Y + point.X - pTarget.X || pTarget.Y == point.Y - point.X + pTarget.X;

        }
    }

    class Knight : ShessPiece // Springer
    {
        public Knight(Point point, bool isWhite) :
            base(point, isWhite, isWhite ? '\u2658' : '\u265E') // )
        {

        }

        public override bool isMovePossible(Point pTarget)
        {

            return pTarget.X == point.X || pTarget.Y == point.Y;

        }
    }

    class Pawn : ShessPiece // Bauer
    {
        public Pawn(Point point, bool isWhite) :
            base(point, isWhite, isWhite ? '\u2659' : '\u265F')
        {
        }

        public override bool isMovePossible(Point pTarget)
        {
            return pTarget.X == point.X || pTarget.Y == point.Y;

        }
    }

    class King : ShessPiece // König
    {
        public King(Point point, bool isWhite) :
            base(point, isWhite, isWhite ? '\u2654' : '\u265A') // )
        {

        }

        public override bool isMovePossible(Point pTarget)
        {

            return pTarget.X == point.X || pTarget.Y == point.Y;

        }
    }

    class Queen : ShessPiece // Königin
    {
        public Queen(Point point, bool isWhite) :
            base(point, isWhite, isWhite ? '\u2655' : '\u265B') // )
        {

        }

        public override bool isMovePossible(Point pTarget)
        {

            return pTarget.X == point.X || pTarget.Y == point.Y;

        }
    }

}
