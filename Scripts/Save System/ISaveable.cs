/// <summary>
/// Interface for listener to save and load.
/// </summary>
public interface ISaveable
{
    /// <summary>
    /// If multiple instances of same type subscribe save manager, you need to identify them by ID.
    /// </summary>
    string ID { get; }

    /// <summary>
    /// The listener can save a data by returning it. You need to define "Serializable" attribute for the data type.
    /// </summary>
    /// <returns>a data to save</returns>
    object Save();

    /// <summary>
    /// The listener can load a data by getting from data parameter.
    /// </summary>
    /// <param name="data">a data loaded</param>

    void Load(object loaded);
}