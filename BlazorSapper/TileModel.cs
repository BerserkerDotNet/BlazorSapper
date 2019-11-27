namespace BlazorSapper
{
    public class TileModel
    {
        public TileModel(int rank, TileState state, (int x, int y) position)
        {
            Rank = rank;
            State = state;
            Position = position;
        }

        public int Rank { get; set; }

        public TileState State { get; set; }

        public (int x, int y) Position { get; set; }

        public bool IsMine => Rank == -1;
    }
}
