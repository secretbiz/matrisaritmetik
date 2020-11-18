namespace MatrisAritmetik.Core.Models
{
    /// <summary>
    /// Class to hold similar functions under a label
    /// </summary>
    public class CommandLabel
    {
        #region Public Fields
        public string Label = "Genel";

        public CommandInfo[] Functions;
        #endregion

        #region Constructors
        public CommandLabel()
        {
        }
        /// <summary>
        /// Creates an instance with the given label and a list of <see cref="CommandInfo"/ instances>
        /// </summary>
        /// <param name="label">Name of the label</param>
        /// <param name="cmds">Array of <see cref="CommandInfo"/> instances</param>
        public CommandLabel(string label, CommandInfo[] cmds)
        {
            Label = label;
            Functions = cmds;
        }
        #endregion
    }
}
