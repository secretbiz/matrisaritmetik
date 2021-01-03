using System.Diagnostics.CodeAnalysis;
using MatrisAritmetik.Core.Models;

namespace MatrisAritmetik.Core
{
    /// <summary>
    /// Enumerated <see cref="MatrisBase{T}"/> limitations
    /// </summary>
    [SuppressMessage("Design", "CA1069:Enums values should not be duplicated", Justification = "<Pending>")]
    public enum MatrisLimits
    {
        /// <summary>
        /// Limit row dimension
        /// </summary>
        forRows = 64,

        /// <summary>
        /// Limit column dimension
        /// </summary>
        forCols = 64,

        /// <summary>
        /// Limit matrix size
        /// </summary>
        forSize = 64 * 64,

        /// <summary>
        /// Limit how many matrices can be stored in a dictionary
        /// </summary>
        forMatrisCount = 8,

        /// <summary>
        /// Character limit for a matrix name
        /// </summary>
        forName = 64
    };

    /// <summary>
    /// Enumerated <see cref="Dataframe"/> limitations
    /// </summary>
    [SuppressMessage("Design", "CA1069:Enums values should not be duplicated", Justification = "<Pending>")]
    public enum DataframeLimits
    {
        /// <summary>
        /// Maximum amount of dataframes per sessions
        /// </summary>
        forDataframeCount = 2,

        /// <summary>
        /// Maximum row count per dataframe
        /// </summary>
        forRows = 512,

        /// <summary>
        /// Maximum depth level of row labels (<see cref="Dataframe"/>.RowLabels.Count)
        /// </summary>
        forRowLabelLevels = 2,

        /// <summary>
        /// Maximum column count per dataframe
        /// </summary>
        forCols = 12,

        /// <summary>
        /// Maximum depth level of column labels (<see cref="Dataframe"/>.ColumnLabels.Count)
        /// </summary>
        forColLabelLevels = 3,

        /// <summary>
        /// Maximum element count
        /// </summary>
        forSize = 512 * 12,

        /// <summary>
        /// Maximum element count including labels
        /// </summary>
        forTotalSize = (512 * 12) + (512 * 2) + (32 * 3),

        /// <summary>
        /// Maximum character amount for a dataframe name 
        /// </summary>
        forName = 64
    }

    /// <summary>
    /// Enumerated limits for command history
    /// </summary>
    public enum CompilerLimits
    {
        /// <summary>
        /// Minimum amount of time in seconds to wait and accept command execution
        /// </summary>
        forCmdSendRateInSeconds = 3,

        /// <summary>
        /// Limit how many command can be shown each command history page
        /// </summary>
        forShowOldCommands = 16,

        /// <summary>
        /// Limit how many characters can be shown for each output string
        /// </summary>
        forOutputCharacterLength = 128 * 128
    };
}
