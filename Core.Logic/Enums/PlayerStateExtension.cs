namespace BelarusChess.Core.Logic
{
    public static class PlayerStateExtension
    {
        public static string GetName(this PlayerState playerState)
        {
            switch (playerState)
            {
                case PlayerState.Regular:
                    return "";
                case PlayerState.Check:
                    return "Шах!";
                case PlayerState.Checkmate:
                    return "Мат";
                case PlayerState.Stalemate:
                    return "Пат";
                case PlayerState.Throne:
                    return "Трон!";
                case PlayerState.ThroneMine:
                    return "Престол мій";
                case PlayerState.Overthrowing:
                    return "Скидання короля";
                default:
                    return "";
            }
        }
    }
}