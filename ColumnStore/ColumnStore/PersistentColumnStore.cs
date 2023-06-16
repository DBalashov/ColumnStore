using System;
using FileContainer;

namespace ColumnStore;

public partial class PersistentColumnStore : IDisposable
{
    const string SECTION_SETTINGS        = "___settings___";
    const int    SECTION_SETTINGS_LENGTH = 128;

    public string                 Path      { get; }
    public PagedContainerAbstract Container { get; }

    /// <summary> partition size </summary>
    public CDTUnit Unit { get; private set; }

    /// <summary> gzip-compress each partition ? </summary>
    public bool Compressed { get; private set; }

    /// <summary> Access to Typed columns </summary>
    public IColumnStoreTyped Typed { get; }

    /// <summary> Access to columns with map to entity </summary>
    public IColumnStoreEntity Entity { get; }

    /// <summary> Access to Untyped columns </summary>
    public IColumnStoreUntyped Untyped { get; }

    /// <summary> ColumnStore container </summary>
    /// <param name="unit">
    /// Range function.
    /// All column data will be splitted to sections by range.
    /// Can be set ONLY for new commonPath data. Will read from container for existing data.
    /// </param>
    /// <param name="compressed">
    /// true for use gzip for compression
    /// Can be set ONLY for new commonPath data. Will read from container for existing data.
    /// </param>
    /// <param name="commonPath">prefix for ALL columns (writing/reading) in PersistentColumnStore instance. For example "/Cars/12312" </param>
    public PersistentColumnStore(PagedContainerAbstract container, CDTUnit unit = CDTUnit.Month, bool compressed = false, string? commonPath = null)
    {
        Container  = container ?? throw new ArgumentException("Can't be null", nameof(container));
        Unit       = unit;
        Compressed = compressed;
        Path       = string.IsNullOrEmpty(commonPath) ? "" : commonPath.Trim('/');

        initSettings(unit, compressed);

        Typed   = new ColumnStoreTyped(this);
        Entity  = new ColumnStoreEntity(this);
        Untyped = new ColumnStoreUntyped(this);
    }

    void initSettings(CDTUnit unit, bool compressed)
    {
        var settingsSectionName = string.Join(Path, SECTION_SETTINGS);
        var configData          = Container[settingsSectionName];
        if (configData == null)
        {
            configData                     = new byte[SECTION_SETTINGS_LENGTH];
            configData[0]                  = (byte) unit;
            configData[1]                  = (byte) (compressed ? 1 : 0);
            Container[settingsSectionName] = configData;
        }

        Unit       = (CDTUnit) configData[0];
        Compressed = configData[1] > 0;
    }

    public void Dispose() => Container.Dispose();
}