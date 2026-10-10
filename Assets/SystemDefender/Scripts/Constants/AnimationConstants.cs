namespace SystemDefender.Scripts.Constants
{
    public static class AnimationConstants
    {
        public static class Player
        {
            public enum MoveDirectionValues
            {
                Left = 1,
                Right = 2,
                Up = 3,
                Down = 4,
                LeftUp = 5,
                RightUp = 6,
                LeftDown = 7,
                RightDown = 8,
            }
            public const string MoveDirection = "MoveDirection";
            public const string Attack = "Attack";
            public const string Death = "Death";
            public const string Dash = "Dash";
        }   
    }
}


