using BelarusChess.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketClient
{
    public class MoveDto
    {
        public Cell PieceCell { get; set; }
        public Cell PieceNewCell { get; set; } 
    }
}
