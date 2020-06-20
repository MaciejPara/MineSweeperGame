using System;
using System.Collections.Generic;
using System.Text;

namespace MineSweeperGame.src
{

    /// <summary>
    /// Field state 
    /// </summary>
    public class State
    {
        /// <summary>
        /// State id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Number of bombs around 
        /// </summary>
        public int? Number { get; set; }
        /// <summary>
        /// Is bomb flag
        /// </summary>
        public bool IsBomb { get; set; }
        /// <summary>
        /// Is clicked flag
        /// </summary>
        public bool Clicked { get; set; }

        /// <summary>
        /// Initialize state with id, number and isBomb flag
        /// </summary>
        /// <param name="id">state id</param>
        /// <param name="number">number of bombs around</param>
        /// <param name="isBomb">flag</param>
        public State(int id, int? number, bool isBomb)
        {
            this.Id = id;
            this.Number = number;
            this.IsBomb = isBomb;
        }
    }
}
