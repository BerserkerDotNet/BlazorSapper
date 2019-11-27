namespace BlazorSapper
{
    public class TileStateChangedEventArgs
    {
        public TileStateChangedEventArgs(TileModel tile, TileState old)
        {
            Tile = tile;
            Old = old;
        }

        public TileModel Tile { get; private set; }

        public TileState Old { get; private set; }
    }
}
