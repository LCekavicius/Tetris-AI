using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisThing.AI
{
    public class Move
    {
        public int RotationState { get; set; }
        public int Transform { get; set; }
        public double Rating { get; set; }
        public bool HoldPiece { get; set; }

        public Move(int rotationState, int transform, double rating, bool holdPiece)
        {
            
            RotationState = rotationState;
            Transform = transform;
            Rating = rating;
            HoldPiece = holdPiece;
        }
    }
}
