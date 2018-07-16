using System;
using GTNTracker.Types;

namespace GTNTracker.EventArguments
{
    public class PositionChangedEventArgs : EventArgs
    {
        public Position Position { get; set; }
        public double Accuracy { get; set; }

        public PositionChangedEventArgs(Position pos, double accuracy)
        {
            Position = pos;
            Accuracy = accuracy;
        }
    }

    public class PositionInaccurateEventArgs : EventArgs
    {
        public Position Position { get; set; }
        public double Accuracy { get; set; }

        public PositionInaccurateEventArgs(Position pos, double accuracy)
        {
            Position = pos;
            Accuracy = accuracy;
        }
    }
}
