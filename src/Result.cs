namespace MineSweeperGame.src
{
    /// <summary>
    /// Game win result
    /// </summary>
    class Result
    {
        public int id;
        public string time;

        /// <summary>
        /// Initialize single result with id and time
        /// </summary>
        /// <param name="id">result id</param>
        /// <param name="time">game finish time</param>
        public Result(int id, string time) {
            this.id = id;
            this.time = time;
        }
    }
}
