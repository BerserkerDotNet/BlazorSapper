using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace BlazorSapper.Components
{
    public class TileBase : ComponentBase
    {
        private const int Mine = -1;

        [Parameter]
        public bool IsDebugMode { get; set; }

        [Parameter]
        public bool IsReadOnly { get; set; }

        [Parameter] 
        public TileModel Model { get; set; }

        [Parameter]
        public EventCallback<TileStateChangedEventArgs> OnTileStateChanged { get; set; }

        protected string GetTileClass()
        {
            if (IsDebugMode && Model.State != TileState.None && Model.IsMine)
            {
                return "danger";
            }

            if (Model.State == TileState.Open)
            {
                if (Model.IsMine)
                {
                    return "danger";
                }
                else if (Model.Rank == 1)
                {
                    return "success";
                }
                else if (Model.Rank == 2)
                {
                    return "warning";
                }
                else if (Model.Rank > 2)
                {
                    return "danger";
                }

                return "light";
            }
            else if (Model.State == TileState.Flagged)
            {
                return "warning";
            }

            return "primary";
        }

        protected string GetTileText()
        {
            return Model.Rank > 0 && Model.State == TileState.Open ? Model.Rank.ToString() : string.Empty;
        }

        protected string GetTileBackground()
        {
            if (Model.IsMine && Model.State == TileState.Open)
            {
                return "white";
            }
            else if (Model.State == TileState.None)
            {
                return "primary";
            }
            else if (Model.State == TileState.Flagged)
            {
                return "white";
            }
            else
            {
                return "light";
            }
        }

        protected async Task OnClick(MouseEventArgs args)
        {
            if (Model.State == TileState.Open)
            {
                return;
            }

            var old = Model.State;
            if (args.Button == 2) //Right click
            {
                Model.State = old == TileState.Flagged ? TileState.None : TileState.Flagged;
            }
            else 
            {
                Model.State = Model.IsMine ? TileState.Detonated : TileState.Open;
            }

            await OnTileStateChanged.InvokeAsync(new TileStateChangedEventArgs(Model, old));
        }
    }
}
